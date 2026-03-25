using Domain.Dto.Seller.MenuOption;
using Domain.Service;

namespace Application.Services.Seller.MenuOptionService;

public interface IMenuOptionService
{
    Task<ServiceCollectionResult<MenuOptionResponseDto>> GetMenuOptionsByMenuId(GetMenuOptionsRequestDto request);
    Task<ServiceObjectResult<Guid>> Add(CreateMenuOptionRequestDto request);
    Task<ServiceObjectResult<bool>> Update(UpdateMenuOptionRequestDto request);
    Task<ServiceObjectResult<bool>> Delete(DeleteMenuOptionRequestDto request);
}