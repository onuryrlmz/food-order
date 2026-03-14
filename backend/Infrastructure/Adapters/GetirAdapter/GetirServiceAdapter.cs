using System.Collections.Concurrent;
using System.Net;
using Base.Helpers;
using Domain.Infrastructure.GetirService.Dto;
using Newtonsoft.Json;
using RestSharp;

namespace Infrastructure.Adapters.GetirAdapter;

public class GetirServiceAdapter : IGetirServiceAdapter
{
    private const string BaseUrl = "https://food-client-api-gateway.getirapi.com";
    private const int MaxParallelRequests = 5;

    private string _getirRestaurantId;
    private readonly ConcurrentDictionary<string, Product> _productByHash = new();
    private List<Category> _categories = [];

    public void TransferRestaurant(string getirRestaurantId)
    {
        _getirRestaurantId = getirRestaurantId;

        GetRestaurantDetails();
        if (_categories.Count == 0) return;
        GetProductDetailsParallel();

        File.WriteAllText($@"{_getirRestaurantId}-GetirProduct.json", JsonConvert.SerializeObject(_productByHash.Values.ToList(), Formatting.Indented));
        File.WriteAllText($@"{_getirRestaurantId}-GetirCategory.json", JsonConvert.SerializeObject(_categories, Formatting.Indented));
    }

    private RestClient CreateClient()
    {
        var options = new RestClientOptions(BaseUrl)
        {
            Timeout = Timeout.InfiniteTimeSpan,
            UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36"
        };
        return new RestClient(options);
    }

    private static void AddCommonHeaders(RestRequest request, string refererPath)
    {
        request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
        request.AddHeader("accept-language", "tr-TR,tr;q=0.6");
        request.AddHeader("cache-control", "max-age=0");
        request.AddHeader("priority", "u=0, i");
        request.AddHeader("sec-ch-ua", "\"Chromium\";v=\"124\", \"Brave\";v=\"124\", \"Not-A.Brand\";v=\"99\"");
        request.AddHeader("sec-ch-ua-mobile", "?0");
        request.AddHeader("sec-ch-ua-platform", "\"macOS\"");
        request.AddHeader("sec-fetch-dest", "document");
        request.AddHeader("sec-fetch-mode", "navigate");
        request.AddHeader("sec-fetch-site", "none");
        request.AddHeader("sec-fetch-user", "?1");
        request.AddHeader("sec-gpc", "1");
        request.AddHeader("upgrade-insecure-requests", "1");
        request.AddHeader("referer", $"{BaseUrl}{refererPath}");
    }

    public void GetRestaurantDetails()
    {
        try
        {
            var client = CreateClient();
            var request = new RestRequest("/restaurants/" + _getirRestaurantId);
            AddCommonHeaders(request, "/restaurants/" + _getirRestaurantId);
            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK) return;

            var restaurantDetail = JsonConvert.DeserializeObject<GetirRestaurantDetail.RootObject>(response.Content);

            var globalMenuByName = new Dictionary<string, Menu>();
            foreach (var getirCategory in restaurantDetail.data.productCategories)
            {
                var category = new Category
                {
                    ReferenceId = getirCategory.id,
                    Name = getirCategory.name,
                    Menus = []
                };

                foreach (var getirProduct in getirCategory.products)
                {
                    if (!globalMenuByName.TryGetValue(getirProduct.name, out var existingMenu))
                    {
                        existingMenu = new Menu
                        {
                            ReferenceId = getirProduct.id,
                            Name = getirProduct.name,
                            Description = getirProduct.description,
                            ImageUrl = getirProduct.fullScreenImageURL,
                            Price = getirProduct.price
                        };
                        globalMenuByName[getirProduct.name] = existingMenu;
                    }

                    category.Menus.Add(existingMenu);
                }

                _categories.Add(category);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[GetirAdapter] GetRestaurantDetails error: {e.Message}");
        }
    }

    private void GetProductDetailsParallel()
    {
        try
        {
            var uniqueMenus = _categories
                .SelectMany(c => c.Menus)
                .GroupBy(m => m.ReferenceId)
                .Select(g => g.First())
                .ToList();

            var client = CreateClient();
            var semaphore = new SemaphoreSlim(MaxParallelRequests);

            var tasks = uniqueMenus.Select(async menu =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var productDetail = await FetchProductDetailAsync(client, menu.ReferenceId);
                    if (productDetail == null) return;

                    ProcessProductDetail(menu, productDetail);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[GetirAdapter] GetProductDetails error for {menu.ReferenceId}: {e.Message}");
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToArray();

            Task.WhenAll(tasks).GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            Console.WriteLine($"[GetirAdapter] GetProductDetailsParallel error: {e.Message}");
        }
    }

    private async Task<GetirProductDetail.RootObject?> FetchProductDetailAsync(RestClient client, string productReferenceId)
    {
        var request = new RestRequest("/restaurants/products/" + productReferenceId);
        AddCommonHeaders(request, "/restaurants/products/" + productReferenceId);
        var response = await client.ExecuteAsync(request);

        if (response.StatusCode != HttpStatusCode.OK) return null;

        return JsonConvert.DeserializeObject<GetirProductDetail.RootObject>(response.Content);
    }

    private void ProcessProductDetail(Menu menu, GetirProductDetail.RootObject productDetail)
    {
        if (productDetail.data.product.optionCategories.Length == 0)
        {
            ProcessSimpleProduct(menu, productDetail);
        }
        else
        {
            ProcessProductWithOptions(menu, productDetail);
        }
    }

    private void ProcessSimpleProduct(Menu menu, GetirProductDetail.RootObject productDetail)
    {
        var menuDetail = new MenuOption
        {
            ReferenceId = productDetail.data.product.id,
            Name = productDetail.data.product.name,
            Description = string.Empty,
            MaxCount = 1,
            MinCount = 1,
            OrderIndex = 1
        };

        var product = new Product
        {
            Name = productDetail.data.product.name,
            Price = 0,
            ProductType = 1,
            ReferenceId = productDetail.data.product.id
        };
        product.Hash = ExtHelper.CreateMD5(JsonConvert.SerializeObject(product));
        _productByHash.TryAdd(product.Hash, product);

        var menuOptionValue = new MenuOptionValue
        {
            ReferenceId = productDetail.data.product.id,
            Name = productDetail.data.product.name,
            ProductReferenceId = product.Hash,
            Price = productDetail.data.product.price,
            OrderIndex = 1
        };
        menuDetail.MenuOptionValues.Add(menuOptionValue);

        lock (menu.MenuOptions)
        {
            menu.MenuOptions.Add(menuDetail);
        }
    }

    private void ProcessProductWithOptions(Menu menu, GetirProductDetail.RootObject productDetail)
    {
        var optionCategories = productDetail.data.product.optionCategories;
        for (var optIdx = 0; optIdx < optionCategories.Length; optIdx++)
        {
            var option1 = optionCategories[optIdx];
            var menuDetail = new MenuOption
            {
                ReferenceId = option1.id,
                Name = option1.name,
                Description = string.Empty,
                MaxCount = option1.maxCount,
                MinCount = option1.minCount,
                OrderIndex = (optIdx + 1) * 1000
            };

            for (var valIdx = 0; valIdx < option1.options.Length; valIdx++)
            {
                var option1Value = option1.options[valIdx];

                var product = new Product
                {
                    Name = option1Value.name,
                    Price = 0,
                    ProductType = option1Value.type == 2 ? 1 : 2,
                    ReferenceId = option1Value.id
                };
                product.Hash = ExtHelper.CreateMD5(JsonConvert.SerializeObject(product));
                _productByHash.TryAdd(product.Hash, product);

                var menuOptionValue = new MenuOptionValue
                {
                    ReferenceId = option1Value.id,
                    Name = option1Value.name,
                    ProductReferenceId = product.Hash,
                    Price = option1Value.price,
                    OrderIndex = (valIdx + 1) * 1000
                };

                foreach (var option2 in option1Value.optionCategories)
                {
                    var menuOptionValueOption = new MenuOptionValueOption
                    {
                        ReferenceId = option2.id,
                        Name = option2.name,
                        Description = string.Empty,
                        MaxCount = option2.maxCount,
                        MinCount = option2.minCount,
                        OrderIndex = menuDetail.OrderIndex + option1Value.optionCategories.ToList().IndexOf(option2) + 1
                    };

                    foreach (var option2Value in option2.options)
                    {
                        var product2 = new Product
                        {
                            Name = option2Value.name,
                            Price = 0,
                            ProductType = option2Value.type == 2 ? 1 : 2,
                            ReferenceId = option2Value.id
                        };
                        product2.Hash = ExtHelper.CreateMD5(JsonConvert.SerializeObject(product2));
                        _productByHash.TryAdd(product2.Hash, product2);

                        var menuOptionValueOptionValue = new MenuOptionValueOptionValue
                        {
                            ReferenceId = option2Value.id,
                            Name = option2Value.name,
                            ProductReferenceId = product2.Hash,
                            Price = option2Value.price,
                            OrderIndex = menuOptionValue.OrderIndex + option2.options.ToList().IndexOf(option2Value) + 1
                        };
                        menuOptionValueOption.MenuOptionValueOptionValues.Add(menuOptionValueOptionValue);
                    }

                    menuOptionValue.MenuOptionValueOptions.Add(menuOptionValueOption);
                }

                menuDetail.MenuOptionValues.Add(menuOptionValue);
            }

            lock (menu.MenuOptions)
            {
                menu.MenuOptions.Add(menuDetail);
            }
        }
    }
}