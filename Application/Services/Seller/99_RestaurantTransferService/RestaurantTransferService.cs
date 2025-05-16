using Domain.Entities.Seller;
using Infrastructure.Adapters.GetirAdapter;
using Newtonsoft.Json;
using Persistence.IRepositories;

namespace Application.Services.Seller._99_RestaurantTransferService;

public class RestaurantTransferService : IRestaurantTransferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGetirServiceAdapter _getirServiceAdapter;

    public RestaurantTransferService(IUnitOfWork unitOfWork, IGetirServiceAdapter getirServiceAdapter)
    {
        _unitOfWork = unitOfWork;
        _getirServiceAdapter = getirServiceAdapter;
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

            await _unitOfWork.BeginTransactionAsync();

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

                await _unitOfWork.ProductRepository.AddAsync(product);
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

                await _unitOfWork.CategoryRepository.AddAsync(category);
                getirCategory.Id = category.Id;

                foreach (var getirMenu in getirCategory.Menus)
                {
                    await _unitOfWork.CommitTransactionAsync();

                    var menu = new Menu
                    {
                        Id = Guid.NewGuid(),
                        RestaurantId = restaurantId,
                        Name = getirMenu.Name,
                        Description = getirMenu.Description,
                        Price = getirMenu.Price,
                        OrderIndex = getirCategory.Menus.IndexOf(getirMenu)
                    };

                    await _unitOfWork.MenuRepository.AddAsync(menu);
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

                        await _unitOfWork.MenuOptionRepository.AddAsync(menuOption);
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
                                Price = getirMenuOptionValues.Price,
                                OrderIndex = getirMenuOption.MenuOptionValues.IndexOf(getirMenuOptionValues),
                            };

                            if (menuOptionValues.ProductId == Guid.Empty) continue;

                            await _unitOfWork.MenuOptionValueRepository.AddAsync(menuOptionValues);
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

                                await _unitOfWork.MenuOptionValueOptionRepository.AddAsync(menuOptionValueOption);
                                getirMenuOptionValueOption.Id = menuOptionValueOption.Id;

                                foreach (var getirMenuOptionValueOptionValue in getirMenuOptionValueOption.MenuOptionValueOptionValues)
                                {
                                    var menuOptionValueOptionValueCommand = new MenuOptionValueOptionValue
                                    {
                                        Id = Guid.NewGuid(),
                                        MenuOptionValueOptionId = getirMenuOptionValueOption.Id,
                                        ProductId = lstProduct.FirstOrDefault(x => x.Hash == getirMenuOptionValueOptionValue.ProductReferenceId)?.Id ?? Guid.Empty,
                                        Price = getirMenuOptionValueOptionValue.Price,
                                        OrderIndex = getirMenuOptionValueOption.MenuOptionValueOptionValues.IndexOf(getirMenuOptionValueOptionValue)
                                    };

                                    if (menuOptionValueOptionValueCommand.ProductId == Guid.Empty) continue;

                                    await _unitOfWork.MenuOptionValueOptionValueRepository.AddAsync(menuOptionValueOptionValueCommand);
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

                    await _unitOfWork.CategoryDetailRepository.AddAsync(categoryDetail);
                    getirMenu.Id = categoryDetail.Id;
                }
            }

            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception e)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new Exception("Error occurred while saving data", e);
        }
    }
}