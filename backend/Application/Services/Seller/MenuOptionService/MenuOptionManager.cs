using Application.Services.Common.TokenService;
using AutoMapper;
using Base.Enums;
using Domain.Dto.Seller.MenuOption;
using Domain.Entities.Seller;
using Domain.Service;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller.MenuOptionService;

public class MenuOptionManager : IMenuOptionService
{
    private readonly IMapper _mapper;
    private readonly IMenuOptionRepository _menuOptionRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IOptionTemplateRepository _optionTemplateRepository;
    private readonly IOptionTemplateValueRepository _optionTemplateValueRepository;
    private readonly IOptionTemplateValueOptionRepository _optionTemplateValueOptionRepository;
    private readonly IOptionTemplateValueOptionValueRepository _optionTemplateValueOptionValueRepository;
    private readonly IMenuOptionValueRepository _menuOptionValueRepository;
    private readonly IMenuOptionValueOptionRepository _menuOptionValueOptionRepository;
    private readonly IMenuOptionValueOptionValueRepository _menuOptionValueOptionValueRepository;
    private readonly ITokenAccessor _tokenAccessor;

    public MenuOptionManager(IMapper mapper, IMenuRepository menuRepository, IMenuOptionRepository menuOptionRepository,
        IOptionTemplateRepository optionTemplateRepository,
        IOptionTemplateValueRepository optionTemplateValueRepository,
        IOptionTemplateValueOptionRepository optionTemplateValueOptionRepository,
        IOptionTemplateValueOptionValueRepository optionTemplateValueOptionValueRepository,
        IMenuOptionValueRepository menuOptionValueRepository,
        IMenuOptionValueOptionRepository menuOptionValueOptionRepository,
        IMenuOptionValueOptionValueRepository menuOptionValueOptionValueRepository,
        ITokenAccessor tokenAccessor)
    {
        _mapper = mapper;
        _menuRepository = menuRepository;
        _menuOptionRepository = menuOptionRepository;
        _optionTemplateRepository = optionTemplateRepository;
        _optionTemplateValueRepository = optionTemplateValueRepository;
        _optionTemplateValueOptionRepository = optionTemplateValueOptionRepository;
        _optionTemplateValueOptionValueRepository = optionTemplateValueOptionValueRepository;
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

    // MenuOption → Menu üzerinden restoranı çözer.
    private async Task<Guid?> ResolveRestaurantIdByMenuId(Guid menuId)
    {
        var menu = await _menuRepository.GetAsync(x => x.Id == menuId);
        return menu?.RestaurantId;
    }

    public async Task<ServiceCollectionResult<MenuOptionResponseDto>> GetMenuOptionsByMenuId(GetMenuOptionsRequestDto request)
    {
        var result = new ServiceCollectionResult<MenuOptionResponseDto>();
        try
        {
            var menuOptions = await _menuOptionRepository.GetListAsync(x => x.MenuId == request.MenuId);
            result.SetData(menuOptions.Items.Select(x => _mapper.Map<MenuOptionResponseDto>(x)).OrderBy(x => x.OrderIndex).ToList());
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<Guid>> Add(CreateMenuOptionRequestDto request)
    {
        var response = new ServiceObjectResult<Guid>();
        try
        {
            var menu = await _menuRepository.GetAsync(x => x.Id == request.MenuId);
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

            var orderIndex = (await _menuOptionRepository.GetListAsync(x => x.MenuId == request.MenuId)).Items.Count;

            // Template-based creation
            if (request.OptionTemplateId.HasValue)
            {
                var template = await _optionTemplateRepository.GetAsync(x => x.Id == request.OptionTemplateId.Value);
                if (template == null)
                {
                    response.Fail("Option template not found");
                    return response;
                }

                var menuOption = new MenuOption
                {
                    Id = Guid.NewGuid(),
                    MenuId = request.MenuId,
                    OptionTemplateId = template.Id,
                    Name = template.Name,
                    Description = template.Description,
                    MinCount = template.MinCount,
                    MaxCount = template.MaxCount,
                    OrderIndex = orderIndex
                };
                await _menuOptionRepository.AddAsync(menuOption);

                // Create derived MenuOptionValues from template
                var templateValues = await _optionTemplateValueRepository.GetListAsync(x => x.OptionTemplateId == template.Id, size: 999);
                foreach (var tv in templateValues.Items.OrderBy(x => x.OrderIndex))
                {
                    var menuOptionValue = new MenuOptionValue
                    {
                        Id = Guid.NewGuid(),
                        MenuOptionId = menuOption.Id,
                        ProductId = tv.ProductId,
                        Price = tv.Price,
                        OptionTemplateValueId = tv.Id,
                        OrderIndex = tv.OrderIndex
                    };
                    await _menuOptionValueRepository.AddAsync(menuOptionValue);

                    // Create derived MenuOptionValueOptions
                    var templateValueOptions = await _optionTemplateValueOptionRepository.GetListAsync(x => x.OptionTemplateValueId == tv.Id, size: 999);
                    foreach (var tvo in templateValueOptions.Items.OrderBy(x => x.OrderIndex))
                    {
                        var menuOptionValueOption = new MenuOptionValueOption
                        {
                            Id = Guid.NewGuid(),
                            MenuOptionValueId = menuOptionValue.Id,
                            OptionTemplateValueOptionId = tvo.Id,
                            Name = tvo.Name,
                            Description = tvo.Description,
                            MinCount = tvo.MinCount,
                            MaxCount = tvo.MaxCount,
                            OrderIndex = tvo.OrderIndex
                        };
                        await _menuOptionValueOptionRepository.AddAsync(menuOptionValueOption);

                        // Create derived MenuOptionValueOptionValues
                        var templateValueOptionValues = await _optionTemplateValueOptionValueRepository.GetListAsync(x => x.OptionTemplateValueOptionId == tvo.Id, size: 999);
                        foreach (var tvov in templateValueOptionValues.Items.OrderBy(x => x.OrderIndex))
                            await _menuOptionValueOptionValueRepository.AddAsync(new MenuOptionValueOptionValue
                            {
                                Id = Guid.NewGuid(),
                                MenuOptionValueOptionId = menuOptionValueOption.Id,
                                ProductId = tvov.ProductId,
                                Price = tvov.Price,
                                OptionTemplateValueOptionValueId = tvov.Id,
                                OrderIndex = tvov.OrderIndex
                            });
                    }
                }

                response.SetData(menuOption.Id);
                return response;
            }

            // Custom creation (existing behavior)
            var customMenuOption = _mapper.Map<MenuOption>(request);
            customMenuOption.Id = Guid.NewGuid();
            customMenuOption.OrderIndex = orderIndex;
            await _menuOptionRepository.AddAsync(customMenuOption);
            response.SetData(customMenuOption.Id);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Update(UpdateMenuOptionRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var menuOption = await _menuOptionRepository.GetAsync(x => x.Id == request.Id);
            if (menuOption == null)
            {
                response.Fail("Menu option not found");
                return response;
            }

            var updRestaurantId = await ResolveRestaurantIdByMenuId(menuOption.MenuId);
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

            if (menuOption.OrderIndex != request.OrderIndex)
            {
                var oldOrderIndex = menuOption.OrderIndex;
                var newOrderIndex = request.OrderIndex;

                var menuDetails = await _menuOptionRepository.GetListAsync(x => x.MenuId == menuOption.MenuId);
                var sortedMenuDetails = menuDetails.Items.OrderBy(x => x.OrderIndex).ToList();

                if (oldOrderIndex < newOrderIndex)
                    for (var i = oldOrderIndex + 1; i <= newOrderIndex; i++)
                    {
                        sortedMenuDetails[i].OrderIndex--;
                        await _menuOptionRepository.UpdateAsync(sortedMenuDetails[i]);
                    }
                else
                    for (var i = newOrderIndex; i < oldOrderIndex; i++)
                    {
                        sortedMenuDetails[i].OrderIndex++;
                        await _menuOptionRepository.UpdateAsync(sortedMenuDetails[i]);
                    }

                menuOption = await _menuOptionRepository.GetAsync(x => x.Id == request.Id && x.MenuId == menuOption.MenuId);
            }

            menuOption.Name = request.Name;
            menuOption.Description = request.Description;
            menuOption.MaxCount = request.MaxCount;
            menuOption.MinCount = request.MinCount;
            menuOption.OrderIndex = request.OrderIndex;

            await _menuOptionRepository.UpdateAsync(menuOption);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Delete(DeleteMenuOptionRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var menuOption = await _menuOptionRepository.GetAsync(x => x.Id == request.Id);
            if (menuOption == null)
            {
                response.Fail("Menu option not found");
                return response;
            }

            var delRestaurantId = await ResolveRestaurantIdByMenuId(menuOption.MenuId);
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

            await _menuOptionRepository.DeleteAsync(menuOption);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}