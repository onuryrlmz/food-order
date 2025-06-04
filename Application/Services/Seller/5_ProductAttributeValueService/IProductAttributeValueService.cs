using Domain.Dto.Seller.ProductAttributeValue;
using Domain.Service;

namespace Application.Services.Seller._5_ProductAttributeValueService;

public interface IProductAttributeValueService
{
    Task<ServiceCollectionResult<GetProductAttributeValueResponseDto>> GetProductAttributeValues(GetProductAttributeValuesRequestDto request);
    Task<ServiceObjectResult<Guid>> Add(AddProductAttributeValueDto request);
    Task<ServiceObjectResult<bool>> Delete(DeleteProductAttributeValueDto request);
}