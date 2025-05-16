using AutoMapper;
using Base.Enums;
using Domain.Dto.Seller;
using Domain.Entities.Seller;
using Domain.Service;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller._3_ProductService;

public class ProductManager : IProductService
{
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public ProductManager(IMapper mapper, IRestaurantRepository restaurantRepository, IProductRepository productRepository)
    {
        _mapper = mapper;
        _restaurantRepository = restaurantRepository;
        _productRepository = productRepository;
    }

    public async Task<ServiceCollectionResult<ProductResponseDto>> GetProducts(GetProductsRequestDto requestDto)
    {
        var result = new ServiceCollectionResult<ProductResponseDto>();
        try
        {
            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == requestDto.RestaurantId);
            if (restaurant == null)
            {
                result.Fail(new Exception("Restaurant not found"));
                return result;
            }

            var pgProduct = await _productRepository.GetListAsync(x => x.RestaurantId == requestDto.RestaurantId);
            var products = pgProduct.Items.OrderBy(x => x.OrderIndex).ToList();

            if (requestDto.ProductIds != null && requestDto.ProductIds.Count != 0)
                products = products.Where(x => requestDto.ProductIds.Contains(x.Id)).ToList();

            var productDtos = products.Select(product => new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ProductType = product.ProductType
            }).ToList();

            result.SetData(productDtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<Guid>> CreateProduct(CreateProductDto request)
    {
        var response = new ServiceObjectResult<Guid>();
        try
        {
            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == request.RestaurantId);
            if (restaurant == null)
            {
                response.AddErrorMessage("Restaurant not found");
                return response;
            }

            var product = _mapper.Map<Product>(request);
            product.Id = Guid.NewGuid();
            product.OrderIndex = request.OrderIndex;

            var productType = (FoodCatalogServiceEnums.ProductTypeEnums)product.ProductType;
            if (!Enum.IsDefined(typeof(FoodCatalogServiceEnums.ProductTypeEnums), productType))
            {
                response.AddErrorMessage("Invalid product type");
                return response;
            }

            /*var cuisine = await _cuisineRepository.GetAsync($"Cuisines", product.CuisineId);
            if (cuisine == null)
            {
                response.AddErrorMessage("Cuisine not found");
                return response;
            }*/

            await _productRepository.AddAsync(product);
            response.SetData(product.Id);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> UpdateProduct(UpdateProductDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var productRequest = _mapper.Map<Product>(request);

            var product = await _productRepository.GetAsync(x => x.Id == productRequest.Id);
            if (product == null)
            {
                response.Fail("Product not found");
                return response;
            }

            product.Name = productRequest.Name;
            product.Description = productRequest.Description;
            product.OrderIndex = productRequest.OrderIndex;

            await _productRepository.UpdateAsync(product);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> DeleteProduct(DeleteProductDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var product = await _productRepository.GetAsync(x => x.Id == request.Id);
            if (product == null)
            {
                response.Fail("Product not found");
                return response;
            }

            await _productRepository.DeleteAsync(product);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}