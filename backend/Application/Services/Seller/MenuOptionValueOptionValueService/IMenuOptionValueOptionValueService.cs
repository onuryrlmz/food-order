using Domain.Dto.Seller.MenuOptionValueOptionValue;
using Domain.Service;

namespace Application.Services.Seller.MenuOptionValueOptionValueService;

public interface IMenuOptionValueOptionValueService
{
    Task<ServiceCollectionResult<MenuOptionValueOptionValueResponseDto>> GetMenuOptionValueOptionValueByMenuOptionValueOptionId(GetMenuOptionValueOptionValueRequestDto request);
    Task<ServiceObjectResult<Guid>> Create(CreateMenuOptionValueOptionValueRequestDto request);
    Task<ServiceObjectResult<bool>> Update(UpdateMenuOptionValueOptionValueRequestDto request);
    Task<ServiceObjectResult<bool>> Delete(DeleteMenuOptionValueOptionValueRequestDto request);
}