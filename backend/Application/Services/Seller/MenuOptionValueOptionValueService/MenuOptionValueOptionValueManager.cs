using Application.Services.Common.TokenService;
using AutoMapper;
using Base.Enums;
using Domain.Dto.Seller.MenuOptionValueOptionValue;
using Domain.Entities.Seller;
using Domain.Service;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller.MenuOptionValueOptionValueService;

public class MenuOptionValueOptionValueManager : IMenuOptionValueOptionValueService
{
    private readonly IMapper _mapper;
    private readonly IMenuRepository _menuRepository;
    private readonly IMenuOptionRepository _menuOptionRepository;
    private readonly IMenuOptionValueRepository _menuOptionValueRepository;
    private readonly IMenuOptionValueOptionRepository _menuOptionValueOptionRepository;
    private readonly IMenuOptionValueOptionValueRepository _menuOptionValueOptionValueRepository;
    private readonly ITokenAccessor _tokenAccessor;

    public MenuOptionValueOptionValueManager(IMapper mapper, IMenuRepository menuRepository, IMenuOptionRepository menuOptionRepository, IMenuOptionValueRepository menuOptionValueRepository, IMenuOptionValueOptionRepository menuOptionValueOptionRepository, IMenuOptionValueOptionValueRepository menuOptionValueOptionValueRepository, ITokenAccessor tokenAccessor)
    {
        _mapper = mapper;
        _menuRepository = menuRepository;
        _menuOptionRepository = menuOptionRepository;
        _menuOptionValueRepository = menuOptionValueRepository;
        _menuOptionValueOptionRepository = menuOptionValueOptionRepository;
        _menuOptionValueOptionValueRepository = menuOptionValueOptionValueRepository;
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

    // MenuOptionValueOptionValue → MenuOptionValueOption → MenuOptionValue → MenuOption → Menu üzerinden restoranı çözer.
    private async Task<Guid?> ResolveRestaurantIdByMenuOptionValueOptionId(Guid menuOptionValueOptionId)
    {
        var movo = await _menuOptionValueOptionRepository.GetAsync(x => x.Id == menuOptionValueOptionId);
        if (movo == null) return null;
        var mov = await _menuOptionValueRepository.GetAsync(x => x.Id == movo.MenuOptionValueId);
        if (mov == null) return null;
        var mo = await _menuOptionRepository.GetAsync(x => x.Id == mov.MenuOptionId);
        if (mo == null) return null;
        var menu = await _menuRepository.GetAsync(x => x.Id == mo.MenuId);
        return menu?.RestaurantId;
    }

    public async Task<ServiceCollectionResult<MenuOptionValueOptionValueResponseDto>> GetMenuOptionValueOptionValueByMenuOptionValueOptionId(GetMenuOptionValueOptionValueRequestDto request)
    {
        var result = new ServiceCollectionResult<MenuOptionValueOptionValueResponseDto>();
        try
        {
            var menuOptionValueOptionValues = await _menuOptionValueOptionValueRepository.GetListAsync(x => x.MenuOptionValueOptionId == request.MenuOptionValueOptionId);
            result.SetData(menuOptionValueOptionValues.Items.Select(x => _mapper.Map<MenuOptionValueOptionValueResponseDto>(x)).OrderBy(x => x.OrderIndex).ToList());
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<Guid>> Create(CreateMenuOptionValueOptionValueRequestDto request)
    {
        var response = new ServiceObjectResult<Guid>();
        try
        {
            var menuOptionValueOption = await _menuOptionValueOptionRepository.GetAsync(x => x.Id == request.MenuOptionValueOptionId);
            if (menuOptionValueOption == null)
            {
                response.Fail("MenuOptionValueOption not found");
                return response;
            }

            var addRestaurantId = await ResolveRestaurantIdByMenuOptionValueOptionId(menuOptionValueOption.Id);
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

            var menuOptionValueOptionValue = _mapper.Map<MenuOptionValueOptionValue>(request);
            menuOptionValueOptionValue.Id = Guid.NewGuid();
            menuOptionValueOptionValue.OrderIndex = (await _menuOptionValueOptionValueRepository.GetListAsync(x => x.MenuOptionValueOptionId == request.MenuOptionValueOptionId)).Items.Count + 1;

            await _menuOptionValueOptionValueRepository.AddAsync(menuOptionValueOptionValue);
            response.SetData(menuOptionValueOptionValue.Id);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Update(UpdateMenuOptionValueOptionValueRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var menuOptionValueOptionValue = await _menuOptionValueOptionValueRepository.GetAsync(x => x.Id == request.Id);
            if (menuOptionValueOptionValue == null)
            {
                response.Fail("MenuOptionValueOptionValue not found");
                return response;
            }

            var updRestaurantId = await ResolveRestaurantIdByMenuOptionValueOptionId(menuOptionValueOptionValue.MenuOptionValueOptionId);
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

            if (menuOptionValueOptionValue.OrderIndex != request.OrderIndex)
            {
                var oldOrderIndex = menuOptionValueOptionValue.OrderIndex;
                var newOrderIndex = request.OrderIndex;

                var menuOptionValueOptionsValues = await _menuOptionValueOptionValueRepository.GetListAsync(x => x.MenuOptionValueOptionId == menuOptionValueOptionValue.MenuOptionValueOptionId);
                var sortedMenuOptionValueOptionsValues = menuOptionValueOptionsValues.Items.OrderBy(x => x.OrderIndex).ToList();

                if (oldOrderIndex < newOrderIndex)
                    for (var i = oldOrderIndex + 1; i <= newOrderIndex; i++)
                    {
                        sortedMenuOptionValueOptionsValues[i].OrderIndex--;
                        await _menuOptionValueOptionValueRepository.UpdateAsync(sortedMenuOptionValueOptionsValues[i]);
                    }
                else
                    for (var i = newOrderIndex; i < oldOrderIndex; i++)
                    {
                        sortedMenuOptionValueOptionsValues[i].OrderIndex++;
                        await _menuOptionValueOptionValueRepository.UpdateAsync(sortedMenuOptionValueOptionsValues[i]);
                    }

                menuOptionValueOptionValue = await _menuOptionValueOptionValueRepository.GetAsync(x => x.Id == request.Id);
            }

            menuOptionValueOptionValue.Price = request.Price;

            await _menuOptionValueOptionValueRepository.UpdateAsync(menuOptionValueOptionValue);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Delete(DeleteMenuOptionValueOptionValueRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var menuOptionValueOptionValue = await _menuOptionValueOptionValueRepository.GetAsync(x => x.Id == request.Id);
            if (menuOptionValueOptionValue == null)
            {
                response.Fail("MenuOptionValueOptionValue not found");
                return response;
            }

            var delRestaurantId = await ResolveRestaurantIdByMenuOptionValueOptionId(menuOptionValueOptionValue.MenuOptionValueOptionId);
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

            await _menuOptionValueOptionValueRepository.DeleteAsync(menuOptionValueOptionValue);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}