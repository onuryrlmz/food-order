using Domain.Infrastructure.GetirService.DtoV2;
using Domain.Infrastructure.YemekSepetiService.Dto;
using Newtonsoft.Json;
using RestSharp;

namespace Infrastructure.Adapters.YemekSepetiAdapter;

public class YemekSepetiAdapter : IYemekSepetiAdapter
{
    private string _ysRestaurantId;
    private YemekSepetiDto _yemekSepetiDto;
    private List<Product>? lstProduct = [];
    private List<Category>? lstCategory = [];

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

            File.WriteAllText($@"{_ysRestaurantId}-YemekSepetiProduct.json", JsonConvert.SerializeObject(lstProduct, Formatting.Indented));
            File.WriteAllText($@"{_ysRestaurantId}-YemekSepetiCategory.json", JsonConvert.SerializeObject(lstCategory, Formatting.Indented));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void GetCategoriesAndProducts()
    {
        try
        {
            foreach (var ysCategory in _yemekSepetiDto.data.menus[0].menu_categories)
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    ReferenceId = ysCategory.id.ToString(),
                    Name = ysCategory.name,
                };

                foreach (var ysMenu in ysCategory.products)
                {
                    var menu = new Menu
                    {
                        Id = Guid.NewGuid(),
                        ReferenceId = ysMenu.id.ToString(),
                        Name = ysMenu.name,
                        Description = ysMenu.description,
                        Price = Convert.ToDouble(ysMenu.product_variations.FirstOrDefault()
                                                     ?.price ??
                                                 0),
                        OrderIndex = ysCategory.products.IndexOf(ysMenu),
                        ImageUrl = ysMenu.file_path,
                    };

                    foreach (var ysMenuOption in ysMenu.product_variations[0].topping_ids)
                    {
                        var ysMenuOptionDto = _yemekSepetiDto.data.menus[0].toppings[ysMenuOption.ToString()];
                        var menuOption = new MenuOption
                        {
                            Id = Guid.NewGuid(),
                            ReferenceId = ysMenuOptionDto.id.ToString(),
                            Name = ysMenuOptionDto.name,
                            Description = string.Empty,
                            MaxCount = ysMenuOptionDto.quantity_maximum,
                            MinCount = ysMenuOptionDto.name.Contains("Promosyon") ? 0 : ysMenuOptionDto.quantity_minimum,
                            OrderIndex = ysMenu.product_variations[0].topping_ids.IndexOf(ysMenuOption),
                        };

                        foreach (var ysMenuOptionValue in ysMenuOptionDto.options)
                        {
                            if (ysMenuOptionDto.options.IndexOf(ysMenuOptionValue) == 0 && ysMenuOptionDto.name.Contains("Promosyon"))
                            {
                                continue; // Skip the first option if it is a promotion
                            }

                            var menuOptionValue = new MenuOptionValue
                            {
                                Id = Guid.NewGuid(),
                                ReferenceId = ysMenuOptionValue.id.ToString(),
                                ProductReferenceId = ysMenuOptionValue.product_id.ToString(),
                                Price = Convert.ToDouble(ysMenuOptionValue?.product?.product_variations?.FirstOrDefault()?.price ?? 0),
                                OrderIndex = ysMenuOptionDto.options.IndexOf(ysMenuOptionValue),
                            };

                            var product = new Product
                            {
                                Id = Guid.NewGuid(),
                                ReferenceId = ysMenuOptionValue.product_id.ToString(),
                                Name = ysMenuOptionValue.product.name.Replace("Promosyon", "").Replace("İstemiyorum", "").Trim(),
                                Price = 0,
                                ProductType = 1,
                                Hash = string.Empty
                            };

                            foreach (var ysProductAttribute in ysMenuOptionValue.product.product_variations[0].topping_ids)
                            {
                                var ysProductAttributeDto = _yemekSepetiDto.data.menus[0].toppings[ysProductAttribute.ToString()];

                                var type = -1;
                                var includePrice = ysProductAttributeDto.options.Any(x => x.price > 0);

                                if (ysProductAttributeDto.quantity_minimum > 0 && includePrice) type = 1;
                                else if (ysProductAttributeDto.quantity_minimum > 0 && !includePrice) type = 2;
                                else if (ysProductAttributeDto.quantity_minimum == 0 && includePrice) type = 3;
                                else if (ysProductAttributeDto.quantity_minimum == 0 && !includePrice) type = 4;

                                var productAttribute = new ProductAttribute
                                {
                                    Id = Guid.NewGuid(),
                                    MasterProductId = product.Id,
                                    Name = ysProductAttributeDto.name,
                                    Type = type,
                                    Description = null,
                                    MinCount = ysProductAttributeDto.quantity_minimum,
                                    MaxCount = ysProductAttributeDto.quantity_maximum,
                                };

                                foreach (var ysProductAttributeValue in ysProductAttributeDto.options)
                                {
                                    var subProduct = new Product
                                    {
                                        Id = Guid.NewGuid(),
                                        ReferenceId = ysProductAttributeValue.product_id.ToString(),
                                        Name = ysProductAttributeValue.name.Replace("Promosyon", "").Replace("İstemiyorum", "").Trim(),
                                        Price = ysProductAttributeValue.price,
                                        ProductType = 2,
                                        Hash = string.Empty
                                    };

                                    var addedSubProduct = lstProduct.FirstOrDefault(x => x.Name == subProduct.Name);
                                    if (addedSubProduct == null)
                                    {
                                        lstProduct.Add(subProduct);
                                    }
                                    else
                                    {
                                        if (addedSubProduct.Price < subProduct.Price)
                                        {
                                            lstProduct.Remove(addedSubProduct);
                                            subProduct.Id = addedSubProduct.Id;
                                            lstProduct.Add(subProduct);
                                        }
                                        else
                                        {
                                            subProduct = addedSubProduct;
                                        }
                                    }

                                    var productAttributeValue = new ProductAttributeValue
                                    {
                                        Id = Guid.NewGuid(),
                                        MasterProductId = product.Id,
                                        ProductAttributeId = productAttribute.Id,
                                        SubProductId = subProduct.Id,
                                    };

                                    productAttribute.ProductAttributeValues.Add(productAttributeValue);
                                }

                                product.ProductAttributes.Add(productAttribute);
                            }

                            menuOption.MenuOptionValues.Add(menuOptionValue);

                            if (lstProduct.FirstOrDefault(x => x.ReferenceId == product.ReferenceId) == null) lstProduct.Add(product);
                        }

                        menu.MenuOptions.Add(menuOption);
                    }

                    category.Menus.Add(menu);


                    break;
                }

                lstCategory.Add(category);
                break;
            }
        }
        catch (Exception e)
        {
        }
    }
}