using Domain.Entities.Seller;
using Infrastructure.Adapters.GetirAdapter;
using Newtonsoft.Json;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller._99_RestaurantTransferService;

public class RestaurantTransferService : IRestaurantTransferService
{
    private readonly IGetirServiceAdapter _getirServiceAdapter;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryDetailRepository _categoryDetailRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IMenuOptionRepository _menuOptionRepository;
    private readonly IMenuOptionValueRepository _menuOptionValueRepository;
    private readonly IMenuOptionValueOptionRepository _menuOptionValueOptionRepository;
    private readonly IMenuOptionValueOptionValueRepository _menuOptionValueOptionValueRepository;
    private readonly IProductRepository _productRepository;

    public RestaurantTransferService(
        IGetirServiceAdapter getirServiceAdapter,
        ICategoryRepository categoryRepository,
        ICategoryDetailRepository categoryDetailRepository,
        IMenuRepository menuRepository,
        IMenuOptionRepository menuOptionRepository,
        IMenuOptionValueRepository menuOptionValueRepository,
        IMenuOptionValueOptionRepository menuOptionValueOptionRepository,
        IMenuOptionValueOptionValueRepository menuOptionValueOptionValueRepository,
        IProductRepository productRepository)
    {
        _getirServiceAdapter = getirServiceAdapter;
        _categoryRepository = categoryRepository;
        _categoryDetailRepository = categoryDetailRepository;
        _menuRepository = menuRepository;
        _menuOptionRepository = menuOptionRepository;
        _menuOptionValueRepository = menuOptionValueRepository;
        _menuOptionValueOptionRepository = menuOptionValueOptionRepository;
        _menuOptionValueOptionValueRepository = menuOptionValueOptionValueRepository;
        _productRepository = productRepository;
    }

    public async Task SaveData(string getirRestaurantId, Guid restaurantId)
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

            var lstProduct = JsonConvert.DeserializeObject<List<Domain.Infrastructure.GetirService.Dto.Product>>(File.ReadAllText($@"{getirRestaurantId}-GetirProduct.json"));
            var lstCategory = JsonConvert.DeserializeObject<List<Domain.Infrastructure.GetirService.Dto.Category>>(File.ReadAllText($@"{getirRestaurantId}-GetirCategory.json"));

            foreach (var getirProduct in lstProduct)
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    Name = getirProduct.Name,
                    ProductType = getirProduct.ProductType,
                    OrderIndex = 0
                };

                await _productRepository.AddAsync(product);
                getirProduct.Id = product.Id;
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
                        Description = getirMenu.Description,
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

                            foreach (var getirMenuOptionValueOption in getirMenuOptionValues.MenuOptionValueOptions)
                            {
                                var menuOptionValueOption = new MenuOptionValueOption
                                {
                                    Id = Guid.NewGuid(),
                                    MenuOptionValueId = getirMenuOptionValues.Id,
                                    Name = getirMenuOptionValueOption.Name,
                                    Description = string.Empty,
                                    MinCount = getirMenuOptionValueOption.MinCount,
                                    MaxCount = getirMenuOptionValueOption.MaxCount,
                                    OrderIndex = getirMenuOption.MenuOptionValues.IndexOf(getirMenuOptionValues)
                                };

                                await _menuOptionValueOptionRepository.AddAsync(menuOptionValueOption);
                                getirMenuOptionValueOption.Id = menuOptionValueOption.Id;

                                foreach (var getirMenuOptionValueOptionValue in getirMenuOptionValueOption.MenuOptionValueOptionValues)
                                {
                                    var menuOptionValueOptionValueCommand = new MenuOptionValueOptionValue
                                    {
                                        Id = Guid.NewGuid(),
                                        MenuOptionValueOptionId = getirMenuOptionValueOption.Id,
                                        ProductId = lstProduct.FirstOrDefault(x => x.Hash == getirMenuOptionValueOptionValue.ProductReferenceId)?.Id ?? Guid.Empty,
                                        Price = (decimal)getirMenuOptionValueOptionValue.Price,
                                        OrderIndex = getirMenuOptionValueOption.MenuOptionValueOptionValues.IndexOf(getirMenuOptionValueOptionValue)
                                    };

                                    if (menuOptionValueOptionValueCommand.ProductId == Guid.Empty) continue;

                                    await _menuOptionValueOptionValueRepository.AddAsync(menuOptionValueOptionValueCommand);
                                    getirMenuOptionValueOptionValue.Id = menuOptionValueOptionValueCommand.Id;
                                }
                            }
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
}