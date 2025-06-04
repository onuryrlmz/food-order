using Domain.Dto.Seller.Category;
using Domain.Service;

namespace Application.Services.Seller._9_CategoryService;

public interface ICategoryService
{
    public Task<ServiceCollectionResult<CategoryResponse>> GetCategoriesByRestaurantId(GetCategoriesByRestaurantIdRequestDto request);
    public Task<ServiceObjectResult<Guid>> CreateCategory(CreateCategoryRequestDto request);
    public Task<ServiceObjectResult<bool>> UpdateCategory(UpdateCategoryRequestDto request);
    public Task<ServiceObjectResult<bool>> DeleteCategory(DeleteCategoryRequestDto request);
}