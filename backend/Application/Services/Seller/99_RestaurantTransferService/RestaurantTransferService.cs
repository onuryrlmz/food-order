using Domain.Entities.Seller;
using Infrastructure.Adapters.GetirAdapter;
using Infrastructure.Adapters.YemekSepetiAdapter;
using Newtonsoft.Json;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller._99_RestaurantTransferService;

public class RestaurantTransferService : IRestaurantTransferService
{
    private readonly IGetirServiceAdapter _getirServiceAdapter;
    private readonly IYemekSepetiAdapter _yemekSepetiAdapter;
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
        IYemekSepetiAdapter yemekSepetiAdapter,
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
        _yemekSepetiAdapter = yemekSepetiAdapter;
        _categoryRepository = categoryRepository;
        _categoryDetailRepository = categoryDetailRepository;
        _menuRepository = menuRepository;
        _menuOptionRepository = menuOptionRepository;
        _menuOptionValueRepository = menuOptionValueRepository;
        _menuOptionValueOptionRepository = menuOptionValueOptionRepository;
        _menuOptionValueOptionValueRepository = menuOptionValueOptionValueRepository;
        _productRepository = productRepository;
    }

    public async Task TransferDataFromGetir(string getirRestaurantId, Guid restaurantId)
    {
        try
        {
            var productFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{getirRestaurantId}-GetirProduct.json");
            var categoryFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{getirRestaurantId}-GetirCategory.json");
            if (!File.Exists(productFilePath) || !File.Exists(categoryFilePath))
            {
                _getirServiceAdapter.TransferRestaurant(getirRestaurantId);
            }

            if (!File.Exists(productFilePath) || !File.Exists(categoryFilePath))
            {
                throw new FileNotFoundException("Getir product or category file not found after transfer attempt.");
            }

            var lstProduct = JsonConvert.DeserializeObject<List<Domain.Infrastructure.GetirService.Dto.Product>>(File.ReadAllText(productFilePath));
            var lstCategory = JsonConvert.DeserializeObject<List<Domain.Infrastructure.GetirService.Dto.Category>>(File.ReadAllText(categoryFilePath));

            await SaveEntities(lstProduct, lstCategory, restaurantId);
        }
        catch (Exception e)
        {
            throw new Exception("Error occurred while saving Getir data", e);
        }
    }

    public async Task TransferDataFromYemekSepeti(string ysRestaurantId, Guid restaurantId)
    {
        try
        {
            _yemekSepetiAdapter.TransferRestaurant(ysRestaurantId);

            var productFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{ysRestaurantId}-YemekSepetiProduct.json");
            var categoryFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"{ysRestaurantId}-YemekSepetiCategory.json");
            if (!File.Exists(productFilePath) || !File.Exists(categoryFilePath))
            {
                throw new FileNotFoundException("YemekSepeti product or category file not found.");
            }

            var lstProduct = JsonConvert.DeserializeObject<List<Domain.Infrastructure.GetirService.Dto.Product>>(File.ReadAllText(productFilePath));
            var lstCategory = JsonConvert.DeserializeObject<List<Domain.Infrastructure.GetirService.Dto.Category>>(File.ReadAllText(categoryFilePath));

            await SaveEntities(lstProduct, lstCategory, restaurantId);
        }
        catch (Exception e)
        {
            throw new Exception("Error occurred while saving YemekSepeti data", e);
        }
    }

    private async Task SaveEntities(
        List<Domain.Infrastructure.GetirService.Dto.Product> lstProduct,
        List<Domain.Infrastructure.GetirService.Dto.Category> lstCategory,
        Guid restaurantId)
    {
        // --- Dictionary lookup: O(n) → O(1) for product resolution ---
        var productByHash = lstProduct
            .Where(x => !string.IsNullOrEmpty(x.Hash))
            .GroupBy(x => x.Hash)
            .ToDictionary(g => g.Key, g => g.First());

        // --- 1. Batch insert: Products ---
        var products = lstProduct.Select(p =>
        {
            if (p.Id == Guid.Empty) p.Id = Guid.NewGuid();
            return new Product
            {
                Id = p.Id,
                RestaurantId = restaurantId,
                Name = p.Name,
                ProductType = p.ProductType,
                OrderIndex = 0,
                Price = (decimal)p.Price,
            };
        }).ToList();

        await _productRepository.AddRangeAsync(products);

        // --- 2. Collect all entities in memory, then batch insert ---
        var allCategories = new List<Category>();
        var allMenus = new List<Menu>();
        var allMenuOptions = new List<MenuOption>();
        var allMenuOptionValues = new List<MenuOptionValue>();
        var allMenuOptionValueOptions = new List<MenuOptionValueOption>();
        var allMenuOptionValueOptionValues = new List<MenuOptionValueOptionValue>();
        var allCategoryDetails = new List<CategoryDetail>();

        for (var catIdx = 0; catIdx < lstCategory.Count; catIdx++)
        {
            var srcCategory = lstCategory[catIdx];
            var category = new Category
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                Name = srcCategory.Name,
                OrderIndex = catIdx
            };
            allCategories.Add(category);
            srcCategory.Id = category.Id;

            for (var menuIdx = 0; menuIdx < srcCategory.Menus.Count; menuIdx++)
            {
                var srcMenu = srcCategory.Menus[menuIdx];
                var menu = new Menu
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    Name = srcMenu.Name,
                    Description = srcMenu.Description ?? string.Empty,
                    Price = (decimal)srcMenu.Price,
                    OrderIndex = menuIdx
                };
                allMenus.Add(menu);
                srcMenu.Id = menu.Id;

                for (var optIdx = 0; optIdx < srcMenu.MenuOptions.Count; optIdx++)
                {
                    var srcOpt = srcMenu.MenuOptions[optIdx];
                    var menuOption = new MenuOption
                    {
                        Id = Guid.NewGuid(),
                        MenuId = menu.Id,
                        Name = srcOpt.Name,
                        Description = string.Empty,
                        MinCount = srcOpt.MinCount,
                        MaxCount = srcOpt.MaxCount,
                        OrderIndex = optIdx
                    };
                    allMenuOptions.Add(menuOption);
                    srcOpt.Id = menuOption.Id;

                    for (var valIdx = 0; valIdx < srcOpt.MenuOptionValues.Count; valIdx++)
                    {
                        var srcVal = srcOpt.MenuOptionValues[valIdx];
                        var resolvedProductId = ResolveProductId(srcVal.ProductReferenceId, productByHash);
                        if (resolvedProductId == Guid.Empty) continue;

                        var menuOptionValue = new MenuOptionValue
                        {
                            Id = Guid.NewGuid(),
                            MenuOptionId = menuOption.Id,
                            ProductId = resolvedProductId,
                            Price = (decimal)srcVal.Price,
                            OrderIndex = valIdx,
                        };
                        allMenuOptionValues.Add(menuOptionValue);
                        srcVal.Id = menuOptionValue.Id;

                        for (var vooIdx = 0; vooIdx < srcVal.MenuOptionValueOptions.Count; vooIdx++)
                        {
                            var srcVoo = srcVal.MenuOptionValueOptions[vooIdx];
                            var menuOptionValueOption = new MenuOptionValueOption
                            {
                                Id = Guid.NewGuid(),
                                MenuOptionValueId = menuOptionValue.Id,
                                Name = srcVoo.Name,
                                Description = string.Empty,
                                MinCount = srcVoo.MinCount,
                                MaxCount = srcVoo.MaxCount,
                                OrderIndex = vooIdx
                            };
                            allMenuOptionValueOptions.Add(menuOptionValueOption);
                            srcVoo.Id = menuOptionValueOption.Id;

                            for (var voovIdx = 0; voovIdx < srcVoo.MenuOptionValueOptionValues.Count; voovIdx++)
                            {
                                var srcVoov = srcVoo.MenuOptionValueOptionValues[voovIdx];
                                var resolvedProductId2 = ResolveProductId(srcVoov.ProductReferenceId, productByHash);
                                if (resolvedProductId2 == Guid.Empty) continue;

                                var menuOptionValueOptionValue = new MenuOptionValueOptionValue
                                {
                                    Id = Guid.NewGuid(),
                                    MenuOptionValueOptionId = menuOptionValueOption.Id,
                                    ProductId = resolvedProductId2,
                                    Price = (decimal)srcVoov.Price,
                                    OrderIndex = voovIdx
                                };
                                allMenuOptionValueOptionValues.Add(menuOptionValueOptionValue);
                                srcVoov.Id = menuOptionValueOptionValue.Id;
                            }
                        }
                    }
                }

                allCategoryDetails.Add(new CategoryDetail
                {
                    Id = Guid.NewGuid(),
                    CategoryId = category.Id,
                    MenuId = menu.Id,
                    OrderIndex = menuIdx
                });
            }
        }

        // --- 3. Batch insert all entities ---
        await _categoryRepository.AddRangeAsync(allCategories);
        await _menuRepository.AddRangeAsync(allMenus);
        await _menuOptionRepository.AddRangeAsync(allMenuOptions);
        await _menuOptionValueRepository.AddRangeAsync(allMenuOptionValues);
        await _menuOptionValueOptionRepository.AddRangeAsync(allMenuOptionValueOptions);
        await _menuOptionValueOptionValueRepository.AddRangeAsync(allMenuOptionValueOptionValues);
        await _categoryDetailRepository.AddRangeAsync(allCategoryDetails);
    }

    private static Guid ResolveProductId(
        string productReferenceId,
        Dictionary<string, Domain.Infrastructure.GetirService.Dto.Product> byHash)
    {
        if (string.IsNullOrEmpty(productReferenceId)) return Guid.Empty;

        if (byHash.TryGetValue(productReferenceId, out var match))
            return match.Id;

        return Guid.Empty;
    }
}