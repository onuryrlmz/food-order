using Domain.Dto.Seller.MenuOptionValueOption;
using Domain.Service;

namespace Application.Services.Seller;

public interface IMenuOptionValueOptionService
{
    Task<ServiceCollectionResult<MenuOptionValueOptionResponseDto>> GetMenuOptionValueOptionsByMenuOptionValueId(GetMenuOptionValueOptionRequestDto request);
    Task<ServiceObjectResult<Guid>> Add(CreateMenuOptionValueOptionRequestDto request);
    Task<ServiceObjectResult<bool>> Update(UpdateMenuOptionValueOptionRequestDto request);
    Task<ServiceObjectResult<bool>> Delete(DeleteMenuOptionValueOptionRequestDto request);
}