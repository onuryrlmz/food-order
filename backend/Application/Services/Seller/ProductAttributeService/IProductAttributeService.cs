using Domain.Dto.Seller.ProductAttribute;
using Domain.Service;

namespace Application.Services.Seller.ProductAttributeService;

public interface IProductAttributeService
{
    Task<ServiceCollectionResult<GetProductAttributeResponseDto>> GetProductAttributes(GetProductAttributesRequestDto requestDto);
    Task<ServiceObjectResult<Guid>> Add(AddProductAttributeDto request);
    Task<ServiceObjectResult<bool>> Update(UpdateProductAttributeDto request);
    Task<ServiceObjectResult<bool>> Delete(DeleteProductAttributeDto request);
}