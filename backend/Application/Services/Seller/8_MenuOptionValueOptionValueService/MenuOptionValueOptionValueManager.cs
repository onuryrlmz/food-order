using AutoMapper;
using Domain.Dto.Seller.MenuOptionValueOptionValue;
using Domain.Entities.Seller;
using Domain.Service;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller._8_MenuOptionValueOptionValueService;

public class MenuOptionValueOptionValueManager : IMenuOptionValueOptionValueService
{
    private readonly IMapper _mapper;
    private readonly IMenuOptionValueOptionRepository _menuOptionValueOptionRepository;
    private readonly IMenuOptionValueOptionValueRepository _menuOptionValueOptionValueRepository;

    public MenuOptionValueOptionValueManager(IMapper mapper, IMenuOptionValueOptionRepository menuOptionValueOptionRepository, IMenuOptionValueOptionValueRepository menuOptionValueOptionValueRepository)
    {
        _mapper = mapper;
        _menuOptionValueOptionRepository = menuOptionValueOptionRepository;
        _menuOptionValueOptionValueRepository = menuOptionValueOptionValueRepository;
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