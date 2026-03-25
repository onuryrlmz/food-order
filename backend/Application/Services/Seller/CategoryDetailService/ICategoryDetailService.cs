using Domain.Dto.Seller.CategoryDetail;
using Domain.Service;

namespace Application.Services.Seller.CategoryDetailService;

public interface ICategoryDetailService
{
    public Task<ServiceCollectionResult<CategoryDetailResponse>> GetCategoryDetailsByCategoryId(GetCategoryDetailsByCategoryIdRequestDto request);
    public Task<ServiceObjectResult<Guid>> CreateCategoryDetail(CreateCategoryDetailRequestDto request);
    public Task<ServiceObjectResult<bool>> DeleteCategoryDetail(DeleteCategoryDetailRequestDto request);
}