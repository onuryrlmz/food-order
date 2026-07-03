using Application.Services.Common.TokenService;
using AutoMapper;
using Base.Enums;
using Domain.Dto.Seller.MenuOptionValue;
using Domain.Entities.Seller;
using Domain.Service;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller.MenuOptionValueService;

public class MenuOptionValueManager : IMenuOptionValueService
{
    private readonly IMapper _mapper;
    private readonly IMenuRepository _menuRepository;
    private readonly IMenuOptionRepository _menuOptionRepository;
    private readonly IMenuOptionValueRepository _menuOptionValueRepository;
    private readonly ITokenAccessor _tokenAccessor;

    public MenuOptionValueManager(IMapper mapper, IMenuRepository menuRepository, IMenuOptionRepository menuOptionRepository, IMenuOptionValueRepository menuOptionValueRepository, ITokenAccessor tokenAccessor)
    {
        _mapper = mapper;
        _menuRepository = menuRepository;
        _menuOptionRepository = menuOptionRepository;
        _menuOptionValueRepository = menuOptionValueRepository;
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

    // MenuOptionValue → MenuOption → Menu üzerinden restoranı çözer.
    private async Task<Guid?> ResolveRestaurantIdByMenuOptionId(Guid menuOptionId)
    {
        var menuOption = await _menuOptionRepository.GetAsync(x => x.Id == menuOptionId);
        if (menuOption == null) return null;
        var menu = await _menuRepository.GetAsync(x => x.Id == menuOption.MenuId);
        return menu?.RestaurantId;
    }

    public async Task<ServiceCollectionResult<MenuOptionValueResponseDto>> GetMenuOptionValuesByMenuOptionId(GetMenuOptionValueRequestDto request)
    {
        var result = new ServiceCollectionResult<MenuOptionValueResponseDto>();
        try
        {
            var menuProducts = await _menuOptionValueRepository.GetListAsync(x => x.MenuOptionId == request.MenuOptionId);
            result.SetData(menuProducts.Items.Select(x => _mapper.Map<MenuOptionValueResponseDto>(x)).OrderBy(x => x.OrderIndex).ToList());
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<Guid>> Add(CreateMenuOptionValueRequestDto request)
    {
        var response = new ServiceObjectResult<Guid>();
        try
        {
            var menuOption = await _menuOptionRepository.GetAsync(x => x.Id == request.MenuOptionId);
            if (menuOption == null)
            {
                response.AddErrorMessage("Menu Detail not found");
                return response;
            }

            var addRestaurantId = await ResolveRestaurantIdByMenuOptionId(menuOption.Id);
            if (addRestaurantId == null)
            {
                response.Fail("Menu not found");
                return response;
            }
            if (!TryAuthorizeRestaurant(addRestaurantId.Value, out var addAuthError))
            {
                response.Fail(addAuthError!);
                return response;
            }

            var menuOptionValue = _mapper.Map<MenuOptionValue>(request);
            menuOptionValue.Id = Guid.NewGuid();
            menuOptionValue.OrderIndex = (await _menuOptionRepository.GetListAsync(x => x.Id == request.MenuOptionId)).Count + 1;

            await _menuOptionValueRepository.AddAsync(menuOptionValue);
            response.SetData(menuOptionValue.Id);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Update(UpdateMenuOptionValueRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var menuOptionValue = await _menuOptionValueRepository.GetAsync(x => x.Id == request.Id);
            if (menuOptionValue == null)
            {
                response.Fail("Menu Option Value not found");
                return response;
            }

            var updRestaurantId = await ResolveRestaurantIdByMenuOptionId(menuOptionValue.MenuOptionId);
            if (updRestaurantId == null)
            {
                response.Fail("Menu not found");
                return response;
            }
            if (!TryAuthorizeRestaurant(updRestaurantId.Value, out var updAuthError))
            {
                response.Fail(updAuthError!);
                return response;
            }

            if (menuOptionValue.OrderIndex != request.OrderIndex)
            {
                var oldOrderIndex = menuOptionValue.OrderIndex;
                var newOrderIndex = request.OrderIndex;

                var updateMenuProducts = await _menuOptionValueRepository.GetListAsync(x => x.MenuOptionId == menuOptionValue.MenuOptionId);
                var sortedMenuProducts = updateMenuProducts.Items.OrderBy(x => x.OrderIndex).ToList();

                if (oldOrderIndex < newOrderIndex)
                    for (var i = oldOrderIndex + 1; i <= newOrderIndex; i++)
                    {
                        sortedMenuProducts[i].OrderIndex--;
                        await _menuOptionValueRepository.UpdateAsync(sortedMenuProducts[i]);
                    }
                else
                    for (var i = newOrderIndex; i < oldOrderIndex; i++)
                    {
                        sortedMenuProducts[i].OrderIndex++;
                        await _menuOptionValueRepository.UpdateAsync(sortedMenuProducts[i]);
                    }

                menuOptionValue = await _menuOptionValueRepository.GetAsync(x => x.Id == request.Id);
            }

            menuOptionValue.Price = request.Price;

            await _menuOptionValueRepository.UpdateAsync(menuOptionValue);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Delete(DeleteMenuOptionValueRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var menuOptionValue = await _menuOptionValueRepository.GetAsync(x => x.Id == request.Id);
            if (menuOptionValue == null)
            {
                response.Fail("Menu Option Value not found");
                return response;
            }

            var delRestaurantId = await ResolveRestaurantIdByMenuOptionId(menuOptionValue.MenuOptionId);
            if (delRestaurantId == null)
            {
                response.Fail("Menu not found");
                return response;
            }
            if (!TryAuthorizeRestaurant(delRestaurantId.Value, out var delAuthError))
            {
                response.Fail(delAuthError!);
                return response;
            }

            await _menuOptionValueRepository.DeleteAsync(menuOptionValue);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}