using Domain.Dto.Seller.Cuisine;
using Domain.Service;

namespace Application.Services.Seller.CuisineService;

public interface ICuisineService
{
    Task<ServiceCollectionResult<CuisineResponseDto>> GetList();
    Task<ServiceObjectResult<bool>> Add(AddCuisineDto requestDto);
    Task<ServiceObjectResult<bool>> Update(UpdateCuisineDto requestDto);
    Task<ServiceObjectResult<bool>> Delete(Guid id);
}