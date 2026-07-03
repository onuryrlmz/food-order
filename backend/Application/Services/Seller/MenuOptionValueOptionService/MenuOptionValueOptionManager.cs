using Application.Services.Common.TokenService;
using AutoMapper;
using Base.Enums;
using Domain.Dto.Seller.MenuOptionValueOption;
using Domain.Entities.Seller;
using Domain.Service;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller.MenuOptionValueOptionService;

public class MenuOptionValueOptionManager : IMenuOptionValueOptionService
{
    private readonly IMapper _mapper;
    private readonly IMenuRepository _menuRepository;
    private readonly IMenuOptionRepository _menuOptionRepository;
    private readonly IMenuOptionValueOptionRepository _menuOptionValueOptionRepository;
    private readonly IMenuOptionValueRepository _menuOptionValueRepository;
    private readonly ITokenAccessor _tokenAccessor;

    public MenuOptionValueOptionManager(IMapper mapper, IMenuRepository menuRepository, IMenuOptionRepository menuOptionRepository, IMenuOptionValueOptionRepository menuOptionValueOptionRepository, IMenuOptionValueRepository menuOptionValueRepository, ITokenAccessor tokenAccessor)
    {
        _mapper = mapper;
        _menuRepository = menuRepository;
        _menuOptionRepository = menuOptionRepository;
        _menuOptionValueOptionRepository = menuOptionValueOptionRepository;
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

    // MenuOptionValueOption → MenuOptionValue → MenuOption → Menu üzerinden restoranı çözer.
    private async Task<Guid?> ResolveRestaurantIdByMenuOptionValueId(Guid menuOptionValueId)
    {
        var mov = await _menuOptionValueRepository.GetAsync(x => x.Id == menuOptionValueId);
        if (mov == null) return null;
        var mo = await _menuOptionRepository.GetAsync(x => x.Id == mov.MenuOptionId);
        if (mo == null) return null;
        var menu = await _menuRepository.GetAsync(x => x.Id == mo.MenuId);
        return menu?.RestaurantId;
    }

    public async Task<ServiceCollectionResult<MenuOptionValueOptionResponseDto>> GetMenuOptionValueOptionsByMenuOptionValueId(GetMenuOptionValueOptionRequestDto request)
    {
        var result = new ServiceCollectionResult<MenuOptionValueOptionResponseDto>();
        try
        {
            var menuOptionValueOptions = await _menuOptionValueOptionRepository.GetListAsync(x =>
                x.MenuOptionValueId == request.MenuOptionValueId);
            result.SetData(menuOptionValueOptions.Items.Select(x => _mapper.Map<MenuOptionValueOptionResponseDto>(x)).OrderBy(x => x.OrderIndex).ToList());
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<Guid>> Add(CreateMenuOptionValueOptionRequestDto request)
    {
        var response = new ServiceObjectResult<Guid>();
        try
        {
            var menuOptionValue = await _menuOptionValueRepository.GetAsync(x => x.Id == request.MenuOptionValueId);
            if (menuOptionValue == null)
            {
                response.Fail("MenuOptionValueOption not found");
                return response;
            }

            var addRestaurantId = await ResolveRestaurantIdByMenuOptionValueId(menuOptionValue.Id);
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

            var menuOptionValueOption = _mapper.Map<MenuOptionValueOption>(request);
            menuOptionValueOption.Id = Guid.NewGuid();
            menuOptionValueOption.OrderIndex = (await _menuOptionValueOptionRepository.GetListAsync(x => x.MenuOptionValueId == request.MenuOptionValueId)).Count + 1;
            await _menuOptionValueOptionRepository.AddAsync(menuOptionValueOption);
            response.SetData(menuOptionValueOption.Id);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Update(UpdateMenuOptionValueOptionRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var menuOptionValueOption = await _menuOptionValueOptionRepository.GetAsync(x => x.Id == request.Id);
            if (menuOptionValueOption == null)
            {
                response.Fail("MenuOptionValueOption not found");
                return response;
            }

            var updRestaurantId = await ResolveRestaurantIdByMenuOptionValueId(menuOptionValueOption.MenuOptionValueId);
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

            if (menuOptionValueOption.OrderIndex != request.OrderIndex)
            {
                var oldOrderIndex = menuOptionValueOption.OrderIndex;
                var newOrderIndex = request.OrderIndex;

                var menuOptionValueOptions = await _menuOptionValueOptionRepository.GetListAsync(x => x.MenuOptionValueId == menuOptionValueOption.MenuOptionValueId);
                var sortedMenuOptionValueOptions = menuOptionValueOptions.Items.OrderBy(x => x.OrderIndex).ToList();

                if (oldOrderIndex < newOrderIndex)
                    for (var i = oldOrderIndex + 1; i <= newOrderIndex; i++)
                    {
                        sortedMenuOptionValueOptions[i].OrderIndex--;
                        await _menuOptionValueOptionRepository.UpdateAsync(sortedMenuOptionValueOptions[i]);
                    }
                else
                    for (var i = newOrderIndex; i < oldOrderIndex; i++)
                    {
                        sortedMenuOptionValueOptions[i].OrderIndex++;
                        await _menuOptionValueOptionRepository.UpdateAsync(sortedMenuOptionValueOptions[i]);
                    }

                menuOptionValueOption = await _menuOptionValueOptionRepository.GetAsync(x => x.Id == request.Id);
            }

            menuOptionValueOption.Name = request.Name;
            menuOptionValueOption.Description = request.Description;
            menuOptionValueOption.MaxCount = request.MaxCount;
            menuOptionValueOption.MinCount = request.MinCount;
            menuOptionValueOption.OrderIndex = request.OrderIndex;

            await _menuOptionValueOptionRepository.UpdateAsync(menuOptionValueOption);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Delete(DeleteMenuOptionValueOptionRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var menuOptionValueOption = await _menuOptionValueOptionRepository.GetAsync(x => x.Id == request.Id);
            if (menuOptionValueOption == null)
            {
                response.Fail("MenuOptionValueOption not found");
                return response;
            }

            var delRestaurantId = await ResolveRestaurantIdByMenuOptionValueId(menuOptionValueOption.MenuOptionValueId);
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

            await _menuOptionValueOptionRepository.DeleteAsync(menuOptionValueOption);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}