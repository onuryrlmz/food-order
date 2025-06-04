using Application.Services.Common.TokenService;
using Base.Enums;
using Domain.Dto.Seller.ProductAttributeValue;
using Domain.Entities.Seller;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller._5_ProductAttributeValueService;

public class ProductAttributeValueManager : IProductAttributeValueService
{
    private readonly ITokenAccessor _tokenAccessor;
    private readonly IProductAttributeValueRepository _productAttributeValueRepository;
    private readonly IProductRepository _productRepository;

    public ProductAttributeValueManager(
        ITokenAccessor tokenAccessor,
        IProductAttributeValueRepository productAttributeValueRepository, IProductRepository productRepository)
    {
        _tokenAccessor = tokenAccessor;
        _productAttributeValueRepository = productAttributeValueRepository;
        _productRepository = productRepository;
    }

    public async Task<ServiceCollectionResult<GetProductAttributeValueResponseDto>> GetProductAttributeValues(GetProductAttributeValuesRequestDto request)
    {
        var response = new ServiceCollectionResult<GetProductAttributeValueResponseDto>();
        try
        {
            if (_tokenAccessor.GetToken() == null)
            {
                response.Fail("Unauthorized");
                return response;
            }

            var product = await _productRepository.GetAsync(x => x.Id == request.ProductId, include: x => x.Include(y => y.Restaurant));
            if (product == null)
            {
                response.Fail("Product not found");
                return response;
            }

            if (_tokenAccessor.GetToken()?.RestaurantIds?.Contains(product.RestaurantId) != true)
            {
                response.Fail("Unauthorized access to this product");
                return response;
            }

            if (product.ProductType != (int)FoodCatalogServiceEnums.ProductTypeEnums.Master)
            {
                response.Fail("Product type must be 'Master' to get product attribute values");
                return response;
            }

            var productAttributeValues = await _productAttributeValueRepository.GetListAsync(x => x.ProductAttributeId == request.ProductAttributeId);

            var productAttributeValueDtos = productAttributeValues.Items.Select(x => new GetProductAttributeValueResponseDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductAttributeId = x.ProductAttributeId,
            }).ToList();

            response.SetData(productAttributeValueDtos);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<Guid>> Add(AddProductAttributeValueDto request)
    {
        var response = new ServiceObjectResult<Guid>();
        try
        {
            if (_tokenAccessor.GetToken() == null)
            {
                response.Fail("Unauthorized");
                return response;
            }

            var product = await _productRepository.GetAsync(x => x.Id == request.ProductId, include: x => x.Include(y => y.Restaurant));
            if (product == null)
            {
                response.Fail("Product not found");
                return response;
            }

            if (_tokenAccessor.GetToken()?.RestaurantIds?.Contains(product.RestaurantId) != true)
            {
                response.Fail("Unauthorized access to this product");
                return response;
            }

            if (product.ProductType != (int)FoodCatalogServiceEnums.ProductTypeEnums.Sub)
            {
                response.Fail("Product type must be 'Sub' to add product attribute value");
                return response;
            }

            var productAttributeValue = new ProductAttributeValue
            {
                Id = Guid.NewGuid(),
                ProductAttributeId = request.ProductAttributeId,
                ProductId = request.ProductId,
            };

            await _productAttributeValueRepository.AddAsync(productAttributeValue);
            response.SetData(productAttributeValue.Id);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Delete(DeleteProductAttributeValueDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var productAttributeValue = await _productAttributeValueRepository.GetAsync(x => x.Id == request.Id, include: x => x.Include(y => y.ProductAttribute).ThenInclude(y => y.Product));
            if (productAttributeValue == null)
            {
                response.Fail("Product attribute value not found");
                return response;
            }

            if (_tokenAccessor.GetToken()?.RestaurantIds?.Contains(productAttributeValue.ProductAttribute.Product.RestaurantId) != true)
            {
                response.Fail("Unauthorized access to this product attribute value");
                return response;
            }

            await _productAttributeValueRepository.DeleteAsync(productAttributeValue);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}