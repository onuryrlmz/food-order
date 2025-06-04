using Domain.Dto.Seller.Product;
using Domain.Service;

namespace Application.Services.Seller._3_ProductService;

public interface IProductService
{
    Task<ServiceCollectionResult<ProductResponseDto>> GetProducts(GetProductsRequestDto requestDto);
    Task<ServiceObjectResult<Guid>> CreateProduct(AddProductDto request);
    Task<ServiceObjectResult<bool>> UpdateProduct(UpdateProductDto request);
    Task<ServiceObjectResult<bool>> DeleteProduct(DeleteProductDto request);
}