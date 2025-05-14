using AutoMapper;
using Domain.Dto.Seller.MenuOptionValueOption;
using Domain.Entities.Seller;
using Domain.Service;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller;

public class MenuOptionValueOptionManager : IMenuOptionValueOptionService
{
    private readonly IMapper _mapper;
    private readonly IMenuOptionValueOptionRepository _menuOptionValueOptionRepository;
    private readonly IMenuOptionValueRepository _menuOptionValueRepository;

    public MenuOptionValueOptionManager(IMapper mapper, IMenuOptionValueOptionRepository menuOptionValueOptionRepository, IMenuOptionValueRepository menuOptionValueRepository)
    {
        _mapper = mapper;
        _menuOptionValueOptionRepository = menuOptionValueOptionRepository;
        _menuOptionValueRepository = menuOptionValueRepository;
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