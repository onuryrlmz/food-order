using Domain.Dto.Seller.Menu;
using Domain.Service;

namespace Application.Services.Seller.MenuService;

public interface IMenuService
{
    Task<ServiceObjectResult<MenuResponseDto>> GetMenuById(GetMenuRequestDto request);
    Task<ServiceCollectionResult<MenuResponseDto>> GetMenusWithProductsByRestaurantId(GetMenusWithProductsByRestaurantIdRequestDto request);
    Task<ServiceObjectResult<MenuResponseDto>> GetMenuInformationJsonFileByMenuId(GetMenuInformationByMenuIdRequestDto request);
    Task<ServiceObjectResult<Guid>> Create(CreateMenuRequestDto request);
    Task<ServiceObjectResult<bool>> Update(UpdateMenuRequestDto request);
    Task<ServiceObjectResult<bool>> Delete(DeleteMenuRequestDto request);
    Task<ServiceCollectionResult<MenuResponseDto>> GetMenusByRestaurantId(GetMenusByRestaurantIdRequestDto request);
    Task<ServiceCollectionResult<MenuResponseDto>> GetMenusByRestaurantIdWithOptions(GetMenusByRestaurantIdRequestDto request);
    Task<ServiceObjectResult<MenuResponseDto>> GetMenuWithProductsById(GetMenuRequestDto request);
    Task<ServiceObjectResult<bool>> CreateMenuInformationJsonFile(GetMenuRequestDto request);
}