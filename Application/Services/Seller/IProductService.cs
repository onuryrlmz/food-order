using Domain.Dto.Seller;
using Domain.Service;

namespace Application.Services.Seller;

public interface IProductService
{
    Task<ServiceCollectionResult<ProductResponseDto>> GetProducts(GetProductsRequestDto requestDto);
    Task<ServiceObjectResult<Guid>> CreateProduct(CreateProductDto request);
    Task<ServiceObjectResult<bool>> UpdateProduct(UpdateProductDto request);
    Task<ServiceObjectResult<bool>> DeleteProduct(DeleteProductDto request);
}