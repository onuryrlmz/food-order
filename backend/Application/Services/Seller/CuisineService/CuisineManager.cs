using Domain.Dto.Seller.Cuisine;
using Domain.Entities.Common;
using Domain.Service;
using Persistence.IRepositories.Common;

namespace Application.Services.Seller.CuisineService;

public class CuisineManager : ICuisineService
{
    private readonly ICuisineRepository _cuisineRepository;

    public CuisineManager(ICuisineRepository cuisineRepository)
    {
        _cuisineRepository = cuisineRepository;
    }

    public async Task<ServiceCollectionResult<CuisineResponseDto>> GetList()
    {
        var response = new ServiceCollectionResult<CuisineResponseDto>();
        try
        {
            var list = await _cuisineRepository.GetListAsync(enableTracking: false, size: 999,
                orderBy: q => q.OrderBy(x => x.OrderIndex));
            response.SetData(list.Items.Select(x => new CuisineResponseDto
            {
                Id = x.Id, Name = x.Name, OrderIndex = x.OrderIndex
            }).ToList());
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Add(AddCuisineDto requestDto)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            await _cuisineRepository.AddAsync(new Cuisine
            {
                Id = Guid.NewGuid(), Name = requestDto.Name, OrderIndex = requestDto.OrderIndex
            });
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Update(UpdateCuisineDto requestDto)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var cuisine = await _cuisineRepository.GetAsync(x => x.Id == requestDto.Id, enableTracking: true);
            if (cuisine == null)
            {
                response.Fail("Mutfak türü bulunamadı.");
                return response;
            }

            cuisine.Name = requestDto.Name;
            cuisine.OrderIndex = requestDto.OrderIndex;
            await _cuisineRepository.UpdateAsync(cuisine);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Delete(Guid id)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var cuisine = await _cuisineRepository.GetAsync(x => x.Id == id, enableTracking: true);
            if (cuisine == null)
            {
                response.Fail("Mutfak türü bulunamadı.");
                return response;
            }

            await _cuisineRepository.DeleteAsync(cuisine);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}