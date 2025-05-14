using Domain.Dto.Seller.MenuOptionValue;
using Domain.Service;

namespace Application.Services.Seller;

public interface IMenuOptionValueService
{
    Task<ServiceCollectionResult<MenuOptionValueResponseDto>> GetMenuOptionValuesByMenuOptionId(GetMenuOptionValueRequestDto request);
    Task<ServiceObjectResult<Guid>> Add(CreateMenuOptionValueRequestDto request);
    Task<ServiceObjectResult<bool>> Update(UpdateMenuOptionValueRequestDto request);
    Task<ServiceObjectResult<bool>> Delete(DeleteMenuOptionValueRequestDto request);
}