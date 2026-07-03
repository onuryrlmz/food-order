using Application.Services.Common.TokenService;
using Application.Services.Seller.ProductService;
using Application.Services.Seller.MenuOptionValueService;
using Application.Services.Seller.MenuOptionService;
using Application.Services.Seller.MenuOptionValueOptionService;
using Application.Services.Seller.MenuOptionValueOptionValueService;
using AutoMapper;
using Base.Enums;
using Microsoft.EntityFrameworkCore;
using Domain.Dto.Seller.Menu;
using Domain.Dto.Seller.MenuOption;
using Domain.Dto.Seller.MenuOptionValue;
using Domain.Dto.Seller.MenuOptionValueOption;
using Domain.Dto.Seller.MenuOptionValueOptionValue;
using Domain.Dto.Seller.Product;
using Domain.Entities.Seller;
using Domain.Service;
using Infrastructure.Adapters.AwsS3;
using Newtonsoft.Json;
using Persistence.IRepositories.Seller;
using RestSharp;

namespace Application.Services.Seller.MenuService;

public class MenuManager : IMenuService
{
    private readonly IMapper _mapper;
    private readonly IMenuOptionService _menuOptionService;
    private readonly IMenuOptionValueOptionService _menuOptionValueOptionService;
    private readonly IMenuOptionValueOptionValueService _menuOptionValueOptionValueService;
    private readonly IMenuOptionValueService _menuOptionValueService;
    private readonly IMenuRepository _menuRepository;
    private readonly IProductService _productService;
    private readonly IAwsS3ServiceAdapter _awsS3ServiceAdapter;
    private readonly ITokenAccessor _tokenAccessor;

    public MenuManager(IMapper mapper, IMenuRepository menuRepository, IMenuOptionService menuOptionService, IMenuOptionValueService menuOptionValueService, IMenuOptionValueOptionService menuOptionValueOptionService, IMenuOptionValueOptionValueService menuOptionValueOptionValueService, IProductService productService, IAwsS3ServiceAdapter awsS3ServiceAdapter, ITokenAccessor tokenAccessor)
    {
        _mapper = mapper;
        _menuRepository = menuRepository;
        _menuOptionService = menuOptionService;
        _menuOptionValueService = menuOptionValueService;
        _menuOptionValueOptionService = menuOptionValueOptionService;
        _menuOptionValueOptionValueService = menuOptionValueOptionValueService;
        _productService = productService;
        _awsS3ServiceAdapter = awsS3ServiceAdapter;
        _tokenAccessor = tokenAccessor;
    }

    // Restoran sahiplik kontrolü (IDOR koruması). Admin muaftır.
    private bool TryAuthorizeRestaurant(Guid restaurantId, out string? error)
    {
        error = null;
        var token = _tokenAccessor.GetToken();
        if (token == null) { error = "Kimlik doğrulama hatası."; return false; }
        if (token.Role == UserRoleEnums.Admin) return true;
        if (token.RestaurantIds == null || !token.RestaurantIds.Contains(restaurantId))
        {
            error = "Bu işlem için yetkiniz yok.";
            return false;
        }
        return true;
    }

    public async Task<ServiceObjectResult<MenuResponseDto>> GetMenuById(GetMenuRequestDto request)
    {
        var result = new ServiceObjectResult<MenuResponseDto>();
        try
        {
            var menu = await _menuRepository.GetAsync(x => x.RestaurantId == request.RestaurantId && x.Id == request.MenuId);
            result.SetData(_mapper.Map<MenuResponseDto>(menu));
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<MenuResponseDto>> GetMenusWithProductsByRestaurantId(GetMenusWithProductsByRestaurantIdRequestDto request)
    {
        var result = new ServiceCollectionResult<MenuResponseDto>();
        try
        {
            var responseMenus = await GetMenusByRestaurantId(new GetMenusByRestaurantIdRequestDto { RestaurantId = request.RestaurantId });
            var menus = responseMenus.Data;

            foreach (var menu in menus)
            {
                var responseMenuOptions = await _menuOptionService.GetMenuOptionsByMenuId(new GetMenuOptionsRequestDto
                {
                    RestaurantId = request.RestaurantId,
                    MenuId = menu.Id
                });
                if (responseMenuOptions.Data.Count == 0)
                {
                    result.Fail("MenuOptions not found");
                    return result;
                }

                var menuOptions = responseMenuOptions.Data;
                foreach (var menuOption in menuOptions)
                {
                    var responseMenuOptionValues = await _menuOptionValueService.GetMenuOptionValuesByMenuOptionId(new GetMenuOptionValueRequestDto
                    {
                        MenuOptionId = menuOption.Id
                    });
                    var menuOptionValues = responseMenuOptionValues.Data;

                    var responseProducts = await _productService.GetProducts(new GetProductsRequestDto
                    {
                        RestaurantId = request.RestaurantId,
                        GetDetails = true,
                        ProductIds = menuOptionValues.Select(x => x.ProductId).ToList()
                    });
                    if (responseProducts.Data.Count == 0)
                    {
                        result.Fail(new Exception("Products not found"));
                        return result;
                    }

                    foreach (var menuOptionValue in menuOptionValues)
                    {
                        var product = responseProducts.Data.FirstOrDefault(x => x.Id == menuOptionValue.ProductId);
                        if (product == null)
                        {
                            result.Fail(new Exception("Product not found"));
                            return result;
                        }

                        menuOptionValue.Product = product;

                        var responseMenuOptionValueOptions = await _menuOptionValueOptionService.GetMenuOptionValueOptionsByMenuOptionValueId(new GetMenuOptionValueOptionRequestDto
                        {
                            MenuOptionValueId = menuOptionValue.Id
                        });
                        if (responseMenuOptionValueOptions.Data.Count == 0)
                        {
                            result.Fail("MenuOptionValueOptions not found");
                            return result;
                        }

                        var menuOptionValueOptions = responseMenuOptionValueOptions.Data;

                        foreach (var menuOptionValueOption in menuOptionValueOptions)
                        {
                            var responseMenuOptionValueOptionValues = await _menuOptionValueOptionValueService.GetMenuOptionValueOptionValueByMenuOptionValueOptionId(new GetMenuOptionValueOptionValueRequestDto
                            {
                                MenuOptionValueOptionId = menuOptionValueOption.Id
                            });
                            var menuOptionValueOptionValues = responseMenuOptionValueOptionValues.Data;

                            foreach (var menuOptionValueOptionValue in menuOptionValueOptionValues) menuOptionValueOption.MenuOptionValueOptionValues.Add(menuOptionValueOptionValue);
                        }
                    }

                    menuOption.MenuOptionValues = menuOptionValues;
                }

                menu.MenuOptions = menuOptions;
            }

            result.SetData(menus);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<MenuResponseDto>> GetMenuInformationJsonFileByMenuId(GetMenuInformationByMenuIdRequestDto request)
    {
        Retry:
        var result = new ServiceObjectResult<MenuResponseDto>();
        try
        {
            var options = new RestClientOptions($"https://cdn.yrlmzteknoloji.com/restaurant/{request.RestaurantId}/menu/{request.MenuId}.json")
            {
                Timeout = TimeSpan.FromMilliseconds(5000)
            };
            var restClient = new RestClient(options);
            var restRequest = new RestRequest("");
            var response = restClient.Execute(restRequest);
            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                var menu = JsonConvert.DeserializeObject<MenuResponseDto>(response.Content);
                result.SetData(menu);
            }
            else
            {
                /*var responseCreate = await CreateMenuInformationJsonFile(client, new MenuRequest
                {
                    RestaurantId = request.RestaurantId,
                    MenuId = request.MenuId
                });

                if (responseCreate.Data) goto Retry;*/
            }
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<Guid>> Create(CreateMenuRequestDto request)
    {
        var response = new ServiceObjectResult<Guid>();
        try
        {
            if (!TryAuthorizeRestaurant(request.RestaurantId, out var authError))
            {
                response.Fail(authError!);
                return response;
            }

            var menu = _mapper.Map<Menu>(request);
            menu.Id = Guid.NewGuid();
            menu.OrderIndex = (await _menuRepository.GetListAsync(x => x.RestaurantId == request.RestaurantId)).Count + 1;

            await _menuRepository.AddAsync(menu);
            response.SetData(menu.Id);
            _ = CreateMenuInformationJsonFile(new GetMenuRequestDto
            {
                RestaurantId = request.RestaurantId,
                MenuId = menu.Id
            });
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Update(UpdateMenuRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var menu = await _menuRepository.GetAsync(x => x.Id == request.Id && x.RestaurantId == request.RestaurantId);
            if (menu == null)
            {
                response.Fail("Menu not found");
                return response;
            }

            if (!TryAuthorizeRestaurant(menu.RestaurantId, out var authError))
            {
                response.Fail(authError!);
                return response;
            }

            menu.Name = request.Name;
            menu.Description = request.Description;
            menu.Price = request.Price;
            menu.OrderIndex = request.OrderIndex;

            await _menuRepository.UpdateAsync(menu);
            response.SetData(true);

            await CreateMenuInformationJsonFile(new GetMenuRequestDto
            {
                RestaurantId = request.RestaurantId,
                MenuId = menu.Id
            });
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Delete(DeleteMenuRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var menu = await _menuRepository.GetAsync(x => x.Id == request.Id);
            if (menu == null)
            {
                response.Fail("Menu not found");
                return response;
            }

            if (!TryAuthorizeRestaurant(menu.RestaurantId, out var authError))
            {
                response.Fail(authError!);
                return response;
            }

            await _menuRepository.DeleteAsync(menu);
            response.SetData(true);

            await CreateMenuInformationJsonFile(new GetMenuRequestDto
            {
                RestaurantId = request.RestaurantId,
                MenuId = menu.Id
            });
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceCollectionResult<MenuResponseDto>> GetMenusByRestaurantId(GetMenusByRestaurantIdRequestDto request)
    {
        var result = new ServiceCollectionResult<MenuResponseDto>();
        try
        {
            var menus = await _menuRepository.GetListAsync(x => x.RestaurantId == request.RestaurantId, size: 999);
            result.SetData(menus.Items.Select(x => _mapper.Map<MenuResponseDto>(x)).OrderBy(x => x.OrderIndex).ToList());
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<MenuResponseDto>> GetMenusByRestaurantIdWithOptions(GetMenusByRestaurantIdRequestDto request)
    {
        var result = new ServiceCollectionResult<MenuResponseDto>();
        try
        {
            var menus = await _menuRepository.GetListAsync(
                x => x.RestaurantId == request.RestaurantId,
                include: x => x
                    .Include(m => m.MenuOptions)
                    .ThenInclude(o => o.MenuOptionValues)
                    .ThenInclude(v => v.Product)
                    .Include(m => m.MenuOptions)
                    .ThenInclude(o => o.MenuOptionValues)
                    .ThenInclude(v => v.MenuOptionValueOptions)
                    .ThenInclude(vo => vo.MenuOptionValueOptionValues)
                    .ThenInclude(vov => vov.Product),
                size: 999);

            var dtos = menus.Items
                .OrderBy(m => m.OrderIndex)
                .Select(m => _mapper.Map<MenuResponseDto>(m))
                .ToList();

            result.SetData(dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<MenuResponseDto>> GetMenuWithProductsById(GetMenuRequestDto request)
    {
        var result = new ServiceObjectResult<MenuResponseDto>();
        try
        {
            var responseMenus = await GetMenusByRestaurantId(new GetMenusByRestaurantIdRequestDto
            {
                RestaurantId = request.RestaurantId
            });

            var menu = responseMenus.Data.FirstOrDefault(x => x.Id == request.MenuId);
            if (menu == null)
            {
                result.Fail(new Exception("Menu not found"));
                return result;
            }

            var responseMenuOptions = await _menuOptionService.GetMenuOptionsByMenuId(new GetMenuOptionsRequestDto
            {
                RestaurantId = request.RestaurantId,
                MenuId = menu.Id
            });

            if (responseMenuOptions.Data.Count == 0)
            {
                result.Fail("MenuOptions not found");
                return result;
            }

            var responseProducts = await _productService.GetProducts(new GetProductsRequestDto
            {
                RestaurantId = request.RestaurantId,
                GetDetails = true
            });

            var menuOptions = responseMenuOptions.Data;
            foreach (var menuOption in menuOptions)
            {
                var responseMenuOptionValues = await _menuOptionValueService.GetMenuOptionValuesByMenuOptionId(new GetMenuOptionValueRequestDto
                {
                    MenuOptionId = menuOption.Id
                });
                var menuOptionValues = responseMenuOptionValues.Data;

                if (responseProducts.Data.Count == 0)
                {
                    result.Fail(new Exception("Products not found"));
                    return result;
                }

                foreach (var menuOptionValue in menuOptionValues)
                {
                    var product = responseProducts.Data.FirstOrDefault(x => x.Id == menuOptionValue.ProductId);
                    if (product == null)
                    {
                        result.Fail(new Exception("Product not found"));
                        return result;
                    }

                    menuOptionValue.Product = product;

                    var responseMenuOptionValueOptions = await _menuOptionValueOptionService.GetMenuOptionValueOptionsByMenuOptionValueId(new GetMenuOptionValueOptionRequestDto
                    {
                        MenuOptionValueId = menuOptionValue.Id
                    });
                    if (responseMenuOptionValueOptions.Data.Count == 0) continue;

                    var menuOptionValueOptions = responseMenuOptionValueOptions.Data;

                    foreach (var menuOptionValueOption in menuOptionValueOptions)
                    {
                        var responseMenuOptionValueOptionValues = await _menuOptionValueOptionValueService.GetMenuOptionValueOptionValueByMenuOptionValueOptionId(new GetMenuOptionValueOptionValueRequestDto
                        {
                            MenuOptionValueOptionId = menuOptionValueOption.Id
                        });
                        var menuOptionValueOptionValues = responseMenuOptionValueOptionValues.Data;

                        foreach (var menuOptionValueOptionValue in menuOptionValueOptionValues)
                        {
                            var productOptionValue = responseProducts.Data.FirstOrDefault(x => x.Id == menuOptionValueOptionValue.ProductId);
                            if (productOptionValue == null)
                            {
                                result.Fail(new Exception("Product not found"));
                                return result;
                            }

                            menuOptionValueOptionValue.Product = productOptionValue;

                            menuOptionValueOption.MenuOptionValueOptionValues.Add(menuOptionValueOptionValue);
                        }
                    }

                    menuOptionValue.MenuOptionValueOptions = menuOptionValueOptions;
                }

                menuOption.MenuOptionValues = menuOptionValues;
            }

            menu.MenuOptions = menuOptions;
            result.SetData(menu);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> CreateMenuInformationJsonFile(GetMenuRequestDto request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var menu = await _menuRepository.GetAsync(X => X.Id == request.MenuId && X.RestaurantId == request.RestaurantId);
            if (menu == null)
            {
                result.Fail("Menu not found");
                return result;
            }

            var mdl = await GetMenuWithProductsById(new GetMenuRequestDto
            {
                RestaurantId = request.RestaurantId,
                MenuId = request.MenuId
            });

            if (mdl.Data == null)
            {
                result.Fail("Menu not found");
                return result;
            }

            var json = JsonConvert.SerializeObject(mdl.Data);
            var pathFolder = Path.Combine("wwwroot", "restaurants", "menus");
            var pathFile = Path.Combine(pathFolder, $"{menu.Id}.json");

            if (!Directory.Exists(pathFolder)) Directory.CreateDirectory(pathFolder);
            if (File.Exists(pathFile)) File.Delete(pathFile);
            await File.WriteAllTextAsync(pathFile, json);

            var url = _awsS3ServiceAdapter.UploadFile(pathFile, $@"restaurant/{request.RestaurantId}/menu");
            if (url == null)
            {
                result.Fail("File not uploaded");
                return result;
            }

            if (File.Exists(pathFile)) File.Delete(pathFile);
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}