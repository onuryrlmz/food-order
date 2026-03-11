using System.Net;
using Base.Helpers;
using Domain.Infrastructure.GetirService.Dto;
using Newtonsoft.Json;
using RestSharp;

namespace Infrastructure.Adapters.GetirAdapter;

public class GetirServiceAdapter : IGetirServiceAdapter
{
    private string _getirRestaurantId;
    private List<Product>? lstProduct = [];
    private List<Category>? lstCategory = [];

    public void TransferRestaurant(string getirRestaurantId)
    {
        _getirRestaurantId = getirRestaurantId;

        GetRestaurantDetails();
        if (lstCategory.Count == 0) return;
        GetProductDetails();

        File.WriteAllText($@"{_getirRestaurantId}-GetirProduct.json", JsonConvert.SerializeObject(lstProduct, Formatting.Indented));
        File.WriteAllText($@"{_getirRestaurantId}-GetirCategory.json", JsonConvert.SerializeObject(lstCategory, Formatting.Indented));
    }

    public void GetRestaurantDetails()
    {
        try
        {
            var jsonData = string.Empty;

            var options = new RestClientOptions("https://food-client-api-gateway.getirapi.com")
            {
                Timeout = Timeout.InfiniteTimeSpan,
                UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36"
            };
            var client = new RestClient(options);
            var request = new RestRequest("/restaurants/" + _getirRestaurantId);
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
            request.AddHeader("referer", "https://food-client-api-gateway.getirapi.com/restaurants/" + _getirRestaurantId);
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK) jsonData = response.Content;
            else return;

            var restaurantDetail = JsonConvert.DeserializeObject<GetirRestaurantDetail.RootObject>(jsonData);

            var lstGlobalMenu = new List<Menu>();
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
                    if (lstGlobalMenu.FirstOrDefault(x => x.Name == getirProduct.name) == null)
                        lstGlobalMenu.Add(new Menu
                        {
                            ReferenceId = getirProduct.id,
                            Name = getirProduct.name,
                            Description = getirProduct.description,
                            ImageUrl = getirProduct.fullScreenImageURL,
                            Price = getirProduct.price
                        });

                    category.Menus.Add(lstGlobalMenu.FirstOrDefault(x => x.Name == getirProduct.name));
                }

                lstCategory.Add(category);
            }
        }
        catch (Exception e)
        {
        }
    }

    public void GetProductDetails()
    {
        try
        {
            foreach (var category in lstCategory)
            foreach (var menu in category.Menus)
            {
                var jsonData = string.Empty;
                var options = new RestClientOptions("https://food-client-api-gateway.getirapi.com")
                {
                    Timeout = Timeout.InfiniteTimeSpan,
                    UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36"
                };
                var client = new RestClient(options);
                var request = new RestRequest("/restaurants/products/" + menu.ReferenceId);
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
                request.AddHeader("referer", "https://food-client-api-gateway.getirapi.com/restaurants/products/" + menu.ReferenceId);
                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK) jsonData = response.Content;
                else return;

                var productDetail = JsonConvert.DeserializeObject<GetirProductDetail.RootObject>(jsonData);

                if (productDetail.data.product.optionCategories.Length == 0)
                {
                    var menuDetail = new MenuOption();
                    menuDetail.ReferenceId = productDetail.data.product.id;
                    menuDetail.Name = productDetail.data.product.name;
                    menuDetail.Description = string.Empty;
                    menuDetail.MaxCount = 1;
                    menuDetail.MinCount = 1;
                    menuDetail.OrderIndex = 1;

                    var product = new Product();
                    product.Name = productDetail.data.product.name;
                    product.Price = 0;
                    product.ProductType = 1;
                    product.Hash = ExtHelper.CreateMD5(JsonConvert.SerializeObject(product));
                    product.ReferenceId = productDetail.data.product.id;
                    if (lstProduct.FirstOrDefault(x => x.Hash == product.Hash) == null) lstProduct.Add(product);

                    var menuOptionValue = new MenuOptionValue();
                    menuOptionValue.ReferenceId = productDetail.data.product.id;
                    menuOptionValue.Name = productDetail.data.product.name;
                    menuOptionValue.ProductReferenceId = product.Hash;
                    menuOptionValue.Price = productDetail.data.product.price;
                    menuOptionValue.OrderIndex = 1;
                    menuDetail.MenuOptionValues.Add(menuOptionValue);

                    menu.MenuOptions.Add(menuDetail);
                }
                else
                {
                    foreach (var option1 in productDetail.data.product.optionCategories)
                    {
                        var menuDetail = new MenuOption();
                        menuDetail.ReferenceId = option1.id;
                        menuDetail.Name = option1.name;
                        menuDetail.Description = string.Empty;
                        menuDetail.MaxCount = option1.maxCount;
                        menuDetail.MinCount = option1.minCount;
                        menuDetail.OrderIndex = (productDetail.data.product.optionCategories.ToList().IndexOf(option1) + 1) * 1000;

                        foreach (var option1Value in option1.options)
                        {
                            var product = new Product();
                            product.Name = option1Value.name;
                            product.Price = 0;
                            product.ProductType = option1Value.type == 2 ? 1 : 2;
                            product.Hash = ExtHelper.CreateMD5(JsonConvert.SerializeObject(product));
                            product.ReferenceId = option1Value.id;
                            if (lstProduct.FirstOrDefault(x => x.Hash == product.Hash) == null) lstProduct.Add(product);

                            var menuOptionValue = new MenuOptionValue();
                            menuOptionValue.ReferenceId = option1Value.id;
                            menuOptionValue.Name = option1Value.name;
                            menuOptionValue.ProductReferenceId = product.Hash;
                            menuOptionValue.Price = option1Value.price;
                            menuOptionValue.OrderIndex = (option1.options.ToList().IndexOf(option1Value) + 1) * 1000;
                            menuDetail.MenuOptionValues.Add(menuOptionValue);

                            foreach (var option2 in option1Value.optionCategories)
                            {
                                var menuOptionValueAttribute = new MenuOptionValueOption();
                                menuOptionValueAttribute.ReferenceId = option2.id;
                                menuOptionValueAttribute.Name = option2.name;
                                menuOptionValueAttribute.Description = string.Empty;
                                menuOptionValueAttribute.MaxCount = option2.maxCount;
                                menuOptionValueAttribute.MinCount = option2.minCount;
                                menuOptionValueAttribute.OrderIndex = menuDetail.OrderIndex + option1Value.optionCategories.ToList().IndexOf(option2) + 1;

                                foreach (var option2Value in option2.options)
                                {
                                    var product2 = new Product();
                                    product2.Name = option2Value.name;
                                    product2.Price = 0;
                                    product2.ProductType = option2Value.type == 2 ? 1 : 2;
                                    product2.Hash = ExtHelper.CreateMD5(JsonConvert.SerializeObject(product2));
                                    product2.ReferenceId = option2Value.id;
                                    if (lstProduct.FirstOrDefault(x => x.Hash == product2.Hash) == null) lstProduct.Add(product2);

                                    var menuOptionValueAttributeValue = new MenuOptionValueOptionValue();
                                    menuOptionValueAttributeValue.ReferenceId = option2Value.id;
                                    menuOptionValueAttributeValue.Name = option2Value.name;
                                    menuOptionValueAttributeValue.ProductReferenceId = product2.Hash;
                                    menuOptionValueAttributeValue.Price = option2Value.price;
                                    menuOptionValueAttributeValue.OrderIndex = menuOptionValue.OrderIndex + option2.options.ToList().IndexOf(option2Value) + 1;
                                    menuOptionValueAttribute.MenuOptionValueOptionValues.Add(menuOptionValueAttributeValue);
                                }

                                menuOptionValue.MenuOptionValueOptions.Add(menuOptionValueAttribute);
                            }
                        }

                        menu.MenuOptions.Add(menuDetail);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}