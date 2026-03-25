using Base.Helpers;
using Domain.Infrastructure.GetirService.Dto;
using Domain.Infrastructure.YemekSepetiService.Dto;
using Newtonsoft.Json;
using RestSharp;

namespace Infrastructure.Adapters.YemekSepetiAdapter;

public class YemekSepetiAdapter : IYemekSepetiAdapter
{
    private string _ysRestaurantId;
    private YemekSepetiDto _yemekSepetiDto;
    private readonly Dictionary<string, Product> _productByName = new();
    private List<Category> _categories = [];

    public void TransferRestaurant(string ysRestaurantId)
    {
        try
        {
            _ysRestaurantId = ysRestaurantId;

            var options = new RestClientOptions($@"https://tr.fd-api.com")
            {
                Timeout = Timeout.InfiniteTimeSpan,
                UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36"
            };
            var client = new RestClient(options);
            var request = new RestRequest($"/api/v5/vendors/{_ysRestaurantId}?include=menus,bundles,multiple_discounts,payment_types&language_id=2&opening_type=delivery&basket_currency=TRY", Method.Get);
            request.AddHeader("accept", "application/json, text/plain, */*");
            request.AddHeader("accept-language", "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
            request.AddHeader("api-version", "7");
            request.AddHeader("origin", "https://www.yemeksepeti.com");
            request.AddHeader("perseus-client-id", "1");
            request.AddHeader("perseus-session-id", "1");
            request.AddHeader("priority", "u=1, i");
            request.AddHeader("referer", "https://www.yemeksepeti.com/");
            request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36");
            request.AddHeader("x-fp-api-key", "volo");
            request.AddHeader("x-pd-language-id", "2");
            var response = client.Execute(request);
            _yemekSepetiDto = JsonConvert.DeserializeObject<YemekSepetiDto>(response.Content);

            GetCategoriesAndProducts();

            File.WriteAllText($@"{_ysRestaurantId}-YemekSepetiProduct.json", JsonConvert.SerializeObject(_productByName.Values.ToList(), Formatting.Indented));
            File.WriteAllText($@"{_ysRestaurantId}-YemekSepetiCategory.json", JsonConvert.SerializeObject(_categories, Formatting.Indented));
        }
        catch (Exception e)
        {
            Console.WriteLine($"[YemekSepetiAdapter] TransferRestaurant error: {e.Message}");
        }
    }

    private Product GetOrAddProduct(string name, string referenceId, double price, int productType)
    {
        var cleanName = name.Replace("Promosyon", "").Replace("İstemiyorum", "").Trim();
        if (_productByName.TryGetValue(cleanName, out var existing))
        {
            if (existing.Price < price)
            {
                existing.Price = price;
                existing.Hash = ExtHelper.CreateMD5(JsonConvert.SerializeObject(new { existing.Name, existing.Price, existing.ProductType }));
            }

            return existing;
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            ReferenceId = referenceId,
            Name = cleanName,
            Price = price,
            ProductType = productType
        };
        product.Hash = ExtHelper.CreateMD5(JsonConvert.SerializeObject(new { product.Name, product.Price, product.ProductType }));
        _productByName[cleanName] = product;
        return product;
    }

    private void GetCategoriesAndProducts()
    {
        try
        {
            foreach (var ysCategory in _yemekSepetiDto.data.menus[0].menu_categories)
            {
                var category = new Category
                {
                    ReferenceId = ysCategory.id.ToString(),
                    Name = ysCategory.name
                };

                foreach (var ysMenu in ysCategory.products)
                {
                    var menu = new Menu
                    {
                        ReferenceId = ysMenu.id.ToString(),
                        Name = ysMenu.name,
                        Description = ysMenu.description,
                        Price = Convert.ToDouble(ysMenu.product_variations.FirstOrDefault()
                                                     ?.price ??
                                                 0),
                        OrderIndex = ysCategory.products.IndexOf(ysMenu),
                        ImageUrl = ysMenu.file_path
                    };

                    foreach (var ysMenuOption in ysMenu.product_variations[0].topping_ids)
                    {
                        var ysMenuOptionDto = _yemekSepetiDto.data.menus[0].toppings[ysMenuOption.ToString()];
                        var menuOption = new MenuOption
                        {
                            ReferenceId = ysMenuOptionDto.id.ToString(),
                            Name = ysMenuOptionDto.name,
                            Description = string.Empty,
                            MaxCount = ysMenuOptionDto.quantity_maximum,
                            MinCount = ysMenuOptionDto.name.Contains("Promosyon") ? 0 : ysMenuOptionDto.quantity_minimum,
                            OrderIndex = ysMenu.product_variations[0].topping_ids.IndexOf(ysMenuOption)
                        };

                        foreach (var ysMenuOptionValue in ysMenuOptionDto.options)
                        {
                            if (ysMenuOptionDto.options.IndexOf(ysMenuOptionValue) == 0 && ysMenuOptionDto.name.Contains("Promosyon")) continue; // Skip the first option if it is a promotion

                            var product = GetOrAddProduct(
                                ysMenuOptionValue.product.name,
                                ysMenuOptionValue.product_id.ToString(),
                                0,
                                1);

                            var menuOptionValue = new MenuOptionValue
                            {
                                ReferenceId = ysMenuOptionValue.id.ToString(),
                                Name = product.Name,
                                ProductReferenceId = product.Hash,
                                Price = Convert.ToDouble(ysMenuOptionValue?.product?.product_variations?.FirstOrDefault()?.price ?? 0),
                                OrderIndex = ysMenuOptionDto.options.IndexOf(ysMenuOptionValue)
                            };

                            // Nested options (option2 level) → MenuOptionValueOption
                            foreach (var ysNestedOption in ysMenuOptionValue.product.product_variations[0].topping_ids)
                            {
                                var ysNestedOptionDto = _yemekSepetiDto.data.menus[0].toppings[ysNestedOption.ToString()];

                                var menuOptionValueOption = new MenuOptionValueOption
                                {
                                    ReferenceId = ysNestedOptionDto.id.ToString(),
                                    Name = ysNestedOptionDto.name,
                                    Description = string.Empty,
                                    MinCount = ysNestedOptionDto.quantity_minimum,
                                    MaxCount = ysNestedOptionDto.quantity_maximum,
                                    OrderIndex = ysMenuOptionValue.product.product_variations[0].topping_ids.IndexOf(ysNestedOption)
                                };

                                foreach (var ysNestedOptionValue in ysNestedOptionDto.options)
                                {
                                    var subProduct = GetOrAddProduct(
                                        ysNestedOptionValue.name,
                                        ysNestedOptionValue.product_id.ToString(),
                                        ysNestedOptionValue.price,
                                        2);

                                    var menuOptionValueOptionValue = new MenuOptionValueOptionValue
                                    {
                                        ReferenceId = ysNestedOptionValue.id.ToString(),
                                        Name = subProduct.Name,
                                        ProductReferenceId = subProduct.Hash,
                                        Price = ysNestedOptionValue.price,
                                        OrderIndex = ysNestedOptionDto.options.IndexOf(ysNestedOptionValue)
                                    };
                                    menuOptionValueOption.MenuOptionValueOptionValues.Add(menuOptionValueOptionValue);
                                }

                                menuOptionValue.MenuOptionValueOptions.Add(menuOptionValueOption);
                            }

                            menuOption.MenuOptionValues.Add(menuOptionValue);
                        }

                        menu.MenuOptions.Add(menuOption);
                    }

                    category.Menus.Add(menu);
                }

                _categories.Add(category);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[YemekSepetiAdapter] GetCategoriesAndProducts error: {e.Message}");
        }
    }
}