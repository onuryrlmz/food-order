using Application.Services.Common.TokenService;
using Domain.Dto.Seller.ProductAttribute;
using Domain.Entities.Seller;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller._4_ProductAttributeService;

public class ProductAttributeManager : IProductAttributeService
{
    private readonly ITokenAccessor _tokenAccessor;
    private readonly IProductAttributeRepository _productAttributeRepository;
    private readonly IProductRepository _productRepository;

    public ProductAttributeManager(
        ITokenAccessor tokenAccessor,
        IProductAttributeRepository productAttributeRepository,
        IProductRepository productRepository)
    {
        _tokenAccessor = tokenAccessor;
        _productAttributeRepository = productAttributeRepository;
        _productRepository = productRepository;
    }

    public async Task<ServiceCollectionResult<GetProductAttributeResponseDto>> GetProductAttributes(GetProductAttributesRequestDto requestDto)
    {
        var result = new ServiceCollectionResult<GetProductAttributeResponseDto>();
        try
        {
            var product = await _productRepository.GetAsync(x => x.Id == requestDto.ProductId, include: x => x.Include(p => p.Restaurant));
            if (product == null)
            {
                result.Fail("Product not found");
                return result;
            }

            if (_tokenAccessor.GetToken()?.RestaurantIds?.Contains(product.RestaurantId) != true)
            {
                result.Fail("Unauthorized access to this product");
                return result;
            }

            var dataAsync = await _productAttributeRepository.GetListAsync(x => x.ProductId == requestDto.ProductId, size: int.MaxValue);
            var productAttributes = dataAsync.Items;

            var productAttributeDtos = productAttributes.Select(pa => new GetProductAttributeResponseDto
            {
                Id = pa.Id,
                ProductId = pa.ProductId,
                Name = pa.Name,
                Description = pa.Description,
                Type = pa.Type,
                MinCount = pa.MinCount,
                MaxCount = pa.MaxCount
            }).ToList();

            result.SetData(productAttributeDtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<Guid>> Add(AddProductAttributeDto request)
    {
        var response = new ServiceObjectResult<Guid>();
        try
        {
            if (_tokenAccessor.GetToken() == null)
            {
                response.Fail("Unauthorized");
                return response;
            }

            var productAttribute = new ProductAttribute
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Name = request.Name,
                Description = request.Description,
                Type = request.Type,
                MinCount = request.MinCount,
                MaxCount = request.MaxCount,
            };

            await _productAttributeRepository.AddAsync(productAttribute);
            response.SetData(productAttribute.Id);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Update(UpdateProductAttributeDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var productAttribute = await _productAttributeRepository.GetAsync(x => x.Id == request.Id, include: x => x.Include(pa => pa.Product));
            if (productAttribute == null)
            {
                response.Fail("Product attribute not found");
                return response;
            }

            if (_tokenAccessor.GetToken()?.RestaurantIds?.Contains(productAttribute.Product.RestaurantId) != true)
            {
                response.Fail("Unauthorized access to this product attribute");
                return response;
            }

            productAttribute.Name = request.Name;
            productAttribute.Description = request.Description;
            //productAttribute.Type = request.Type;
            //TODO: Product Attibute Type değerine göre minCount ve maxCount değerlerini kontrol et
            productAttribute.MinCount = request.MinCount;
            productAttribute.MaxCount = request.MaxCount;
            await _productAttributeRepository.UpdateAsync(productAttribute);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Delete(DeleteProductAttributeDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var productAttribute = await _productAttributeRepository.GetAsync(x => x.Id == request.Id, include: x => x.Include(pa => pa.Product));
            if (productAttribute == null)
            {
                response.Fail("Product attribute not found");
                return response;
            }

            if (_tokenAccessor.GetToken()?.RestaurantIds?.Contains(productAttribute.Product.RestaurantId) != true)
            {
                response.Fail("Unauthorized access to this product attribute");
                return response;
            }

            await _productAttributeRepository.DeleteAsync(productAttribute);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}