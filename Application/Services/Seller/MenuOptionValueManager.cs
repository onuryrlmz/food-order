using AutoMapper;
using Domain.Dto.Seller.MenuOptionValue;
using Domain.Entities.Seller;
using Domain.Service;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller;

public class MenuOptionValueManager : IMenuOptionValueService
{
    private readonly IMapper _mapper;
    private readonly IMenuOptionRepository _menuOptionRepository;
    private readonly IMenuOptionValueRepository _menuOptionValueRepository;

    public MenuOptionValueManager(IMapper mapper, IMenuOptionRepository menuOptionRepository, IMenuOptionValueRepository menuOptionValueRepository)
    {
        _mapper = mapper;
        _menuOptionRepository = menuOptionRepository;
        _menuOptionValueRepository = menuOptionValueRepository;
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