using Domain.Entities.Seller;
using Infrastructure.Adapters.GetirAdapter;
using Infrastructure.Adapters.YemekSepetiAdapter;
using Newtonsoft.Json;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller._99_RestaurantTransferService;

public class RestaurantTransferServiceV2 : IRestaurantTransferServiceV2
{
    private readonly IGetirServiceAdapterV2 _getirServiceAdapter;
    private readonly IYemekSepetiAdapter _yemekSepetiAdapter;

    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryDetailRepository _categoryDetailRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IMenuOptionRepository _menuOptionRepository;
    private readonly IMenuOptionValueRepository _menuOptionValueRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductAttributeRepository _productAttributeRepository;
    private readonly IProductAttributeValueRepository _productAttributeValueRepository;

    public RestaurantTransferServiceV2(
        IGetirServiceAdapterV2 getirServiceAdapter,
        IYemekSepetiAdapter yemekSepetiAdapter,
        ICategoryRepository categoryRepository,
        ICategoryDetailRepository categoryDetailRepository,
        IMenuRepository menuRepository,
        IMenuOptionRepository menuOptionRepository,
        IMenuOptionValueRepository menuOptionValueRepository,
        IProductRepository productRepository,
        IProductAttributeRepository productAttributeRepository,
        IProductAttributeValueRepository productAttributeValueRepository)
    {
        _getirServiceAdapter = getirServiceAdapter;
        _yemekSepetiAdapter = yemekSepetiAdapter;

        _categoryRepository = categoryRepository;
        _categoryDetailRepository = categoryDetailRepository;
        _menuRepository = menuRepository;
        _menuOptionRepository = menuOptionRepository;
        _menuOptionValueRepository = menuOptionValueRepository;
        _productRepository = productRepository;
        _productAttributeRepository = productAttributeRepository;
        _productAttributeValueRepository = productAttributeValueRepository;
    }

    public async Task TransferDataFromGetir(string getirRestaurantId, Guid restaurantId)
    {
        try
        {
            _getirServiceAdapter.TransferRestaurant(getirRestaurantId);

            var getirProductFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{getirRestaurantId}-GetirProduct.json");
            var getirCategoryFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{getirRestaurantId}-GetirCategory.json");
            if (!File.Exists(getirProductFilePath) || !File.Exists(getirCategoryFilePath))
            {
                throw new FileNotFoundException("Getir product or category file not found.");
            }

            var lstProduct = JsonConvert.DeserializeObject<List<Domain.Infrastructure.GetirService.DtoV2.Product>>(File.ReadAllText($@"{getirRestaurantId}-GetirProduct.json"));
            var lstCategory = JsonConvert.DeserializeObject<List<Domain.Infrastructure.GetirService.DtoV2.Category>>(File.ReadAllText($@"{getirRestaurantId}-GetirCategory.json"));

            foreach (var getirProduct in lstProduct)
            {
                var product = new Product
                {
                    Id = getirProduct.Id,
                    RestaurantId = restaurantId,
                    Name = getirProduct.Name,
                    ProductType = getirProduct.ProductType,
                    OrderIndex = 0,
                    Price = (decimal)getirProduct.Price,
                };

                await _productRepository.AddAsync(product);
            }

            foreach (var getirProduct in lstProduct)
            {
                foreach (var getirProductAttribute in getirProduct.ProductAttributes)
                {
                    var productAttribute = new ProductAttribute
                    {
                        Id = getirProductAttribute.Id,
                        ProductId = getirProduct.Id,
                        Name = getirProductAttribute.Name,
                        Type = getirProductAttribute.Type,
                        Description = string.Empty,
                        MinCount = getirProductAttribute.MinCount,
                        MaxCount = getirProductAttribute.MaxCount
                    };

                    await _productAttributeRepository.AddAsync(productAttribute);

                    foreach (var getirProductAttributeValue in getirProductAttribute.ProductAttributeValues)
                    {
                        if (lstProduct.FirstOrDefault(x => x.Id == getirProductAttributeValue.SubProductId) == null)
                        {
                            continue; // Skip if the master product does not exist
                        }

                        var productAttributeValue = new ProductAttributeValue
                        {
                            Id = getirProductAttributeValue.Id,
                            ProductAttributeId = productAttribute.Id,
                            ProductId = lstProduct.FirstOrDefault(x => x.Id == getirProductAttributeValue.SubProductId).Id
                        };

                        await _productAttributeValueRepository.AddAsync(productAttributeValue);
                    }
                }
            }

            foreach (var getirCategory in lstCategory)
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    Name = getirCategory.Name,
                    OrderIndex = lstCategory.IndexOf(getirCategory)
                };

                await _categoryRepository.AddAsync(category);
                getirCategory.Id = category.Id;

                foreach (var getirMenu in getirCategory.Menus)
                {
                    var menu = new Menu
                    {
                        Id = Guid.NewGuid(),
                        RestaurantId = restaurantId,
                        Name = getirMenu.Name,
                        Description = string.Empty,
                        Price = (decimal)getirMenu.Price,
                        OrderIndex = getirCategory.Menus.IndexOf(getirMenu)
                    };

                    await _menuRepository.AddAsync(menu);
                    getirMenu.Id = menu.Id;

                    foreach (var getirMenuOption in getirMenu.MenuOptions)
                    {
                        var menuOption = new MenuOption
                        {
                            Id = Guid.NewGuid(),
                            MenuId = getirMenu.Id,
                            Name = getirMenuOption.Name,
                            Description = string.Empty,
                            MinCount = getirMenuOption.MinCount,
                            MaxCount = getirMenuOption.MaxCount,
                            OrderIndex = getirMenu.MenuOptions.IndexOf(getirMenuOption)
                        };

                        await _menuOptionRepository.AddAsync(menuOption);
                        getirMenuOption.Id = menuOption.Id;

                        foreach (var getirMenuOptionValues in getirMenuOption.MenuOptionValues)
                        {
                            var menuOptionValues = new MenuOptionValue
                            {
                                Id = Guid.NewGuid(),
                                MenuOptionId = getirMenuOption.Id,
                                ProductId = lstProduct.FirstOrDefault(x => x.Hash == getirMenuOptionValues.ProductReferenceId)
                                                ?.Id ??
                                            Guid.Empty,
                                Price = (decimal)getirMenuOptionValues.Price,
                                OrderIndex = getirMenuOption.MenuOptionValues.IndexOf(getirMenuOptionValues),
                            };

                            if (menuOptionValues.ProductId == Guid.Empty) continue;

                            await _menuOptionValueRepository.AddAsync(menuOptionValues);
                            getirMenuOptionValues.Id = menuOptionValues.Id;
                        }
                    }

                    var categoryDetail = new CategoryDetail
                    {
                        Id = Guid.NewGuid(),
                        CategoryId = category.Id,
                        MenuId = menu.Id,
                        OrderIndex = getirCategory.Menus.IndexOf(getirMenu)
                    };

                    await _categoryDetailRepository.AddAsync(categoryDetail);
                    getirMenu.Id = categoryDetail.Id;
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error occurred while saving data", e);
        }
    }

    public async Task TransferDataFromYemekSepeti(string ysRestaurantId, Guid restaurantId)
    {
        try
        {
            _yemekSepetiAdapter.TransferRestaurant(ysRestaurantId);

            var ysProductFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{ysRestaurantId}-YemekSepetiProduct.json");
            var ysCategoryFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{ysRestaurantId}-YemekSepetiCategory.json");
            if (!File.Exists(ysProductFilePath) || !File.Exists(ysCategoryFilePath))
            {
                throw new FileNotFoundException("Getir product or category file not found.");
            }

            var lstProduct = JsonConvert.DeserializeObject<List<Domain.Infrastructure.GetirService.DtoV2.Product>>(File.ReadAllText($"{ysRestaurantId}-YemekSepetiProduct.json"));
            var lstCategory = JsonConvert.DeserializeObject<List<Domain.Infrastructure.GetirService.DtoV2.Category>>(File.ReadAllText($"{ysRestaurantId}-YemekSepetiCategory.json"));

            foreach (var ysProduct in lstProduct)
            {
                var product = new Product
                {
                    Id = ysProduct.Id,
                    RestaurantId = restaurantId,
                    Name = ysProduct.Name,
                    ProductType = ysProduct.ProductType,
                    OrderIndex = 0,
                    Price = (decimal)ysProduct.Price,
                };

                await _productRepository.AddAsync(product);
            }

            foreach (var ysProduct in lstProduct)
            {
                foreach (var ysProductAttribute in ysProduct.ProductAttributes)
                {
                    var productAttribute = new ProductAttribute
                    {
                        Id = ysProductAttribute.Id,
                        ProductId = ysProduct.Id,
                        Name = ysProductAttribute.Name,
                        Type = ysProductAttribute.Type,
                        Description = string.Empty,
                        MinCount = ysProductAttribute.MinCount,
                        MaxCount = ysProductAttribute.MaxCount
                    };

                    await _productAttributeRepository.AddAsync(productAttribute);

                    foreach (var ysProductAttributeValue in ysProductAttribute.ProductAttributeValues)
                    {
                        if (lstProduct.FirstOrDefault(x => x.Id == ysProductAttributeValue.SubProductId) == null)
                        {
                            continue; // Skip if the master product does not exist
                        }

                        var productAttributeValue = new ProductAttributeValue
                        {
                            Id = ysProductAttributeValue.Id,
                            ProductAttributeId = productAttribute.Id,
                            ProductId = lstProduct.FirstOrDefault(x => x.Id == ysProductAttributeValue.SubProductId).Id
                        };

                        await _productAttributeValueRepository.AddAsync(productAttributeValue);
                    }
                }
            }

            foreach (var ysCategory in lstCategory)
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    Name = ysCategory.Name,
                    OrderIndex = lstCategory.IndexOf(ysCategory)
                };

                await _categoryRepository.AddAsync(category);
                ysCategory.Id = category.Id;

                foreach (var ysMenu in ysCategory.Menus)
                {
                    var menu = new Menu
                    {
                        Id = Guid.NewGuid(),
                        RestaurantId = restaurantId,
                        Name = ysMenu.Name,
                        Description = string.Empty,
                        Price = (decimal)ysMenu.Price,
                        OrderIndex = ysCategory.Menus.IndexOf(ysMenu)
                    };

                    await _menuRepository.AddAsync(menu);
                    ysMenu.Id = menu.Id;

                    foreach (var ysMenuOption in ysMenu.MenuOptions)
                    {
                        var menuOption = new MenuOption
                        {
                            Id = Guid.NewGuid(),
                            MenuId = ysMenu.Id,
                            Name = ysMenuOption.Name,
                            Description = string.Empty,
                            MinCount = ysMenuOption.MinCount,
                            MaxCount = ysMenuOption.MaxCount,
                            OrderIndex = ysMenu.MenuOptions.IndexOf(ysMenuOption)
                        };

                        await _menuOptionRepository.AddAsync(menuOption);
                        ysMenuOption.Id = menuOption.Id;

                        foreach (var ysMenuOptionValues in ysMenuOption.MenuOptionValues)
                        {
                            var menuOptionValues = new MenuOptionValue
                            {
                                Id = Guid.NewGuid(),
                                MenuOptionId = ysMenuOption.Id,
                                ProductId = lstProduct.FirstOrDefault(x => x.ReferenceId == ysMenuOptionValues.ProductReferenceId)?.Id ?? Guid.Empty,
                                Price = (decimal)ysMenuOptionValues.Price,
                                OrderIndex = ysMenuOption.MenuOptionValues.IndexOf(ysMenuOptionValues),
                            };

                            if (menuOptionValues.ProductId == Guid.Empty) continue;

                            await _menuOptionValueRepository.AddAsync(menuOptionValues);
                            ysMenuOptionValues.Id = menuOptionValues.Id;
                        }
                    }

                    var categoryDetail = new CategoryDetail
                    {
                        Id = Guid.NewGuid(),
                        CategoryId = category.Id,
                        MenuId = menu.Id,
                        OrderIndex = ysCategory.Menus.IndexOf(ysMenu)
                    };

                    await _categoryDetailRepository.AddAsync(categoryDetail);
                    ysMenu.Id = categoryDetail.Id;
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error occurred while saving data", e);
        }
    }
}