using AutoMapper;
using Domain.Dto.Seller.MenuOption;
using Domain.Entities.Seller;
using Domain.Service;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller;

public class MenuOptionManager : IMenuOptionService
{
    private readonly IMapper _mapper;
    private readonly IMenuOptionRepository _menuOptionRepository;
    private readonly IMenuRepository _menuRepository;

    public MenuOptionManager(IMapper mapper, IMenuRepository menuRepository, IMenuOptionRepository menuOptionRepository)
    {
        _mapper = mapper;
        _menuRepository = menuRepository;
        _menuOptionRepository = menuOptionRepository;
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

            var menuOption = _mapper.Map<MenuOption>(request);
            menuOption.Id = Guid.NewGuid();
            menuOption.OrderIndex = (await _menuOptionRepository.GetListAsync(x => x.MenuId == request.MenuId)).Items.Count;
            await _menuOptionRepository.AddAsync(menuOption);
            response.SetData(menuOption.Id);
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