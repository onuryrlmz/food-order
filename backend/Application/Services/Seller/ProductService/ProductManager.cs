using Application.Services.Common.TokenService;
using AutoMapper;
using Base.Enums;
using Domain.Dto.Seller.Product;
using Domain.Entities.Seller;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller.ProductService;

public class ProductManager : IProductService
{
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly ITokenAccessor _tokenAccessor;

    public ProductManager(IMapper mapper, IRestaurantRepository restaurantRepository, IProductRepository productRepository, ITokenAccessor tokenAccessor)
    {
        _mapper = mapper;
        _restaurantRepository = restaurantRepository;
        _productRepository = productRepository;
        _tokenAccessor = tokenAccessor;
    }

    // Satıcı bir restoranı yönetme yetkisine sahip mi? Admin tüm restoranları yönetebilir.
    // Müşteri (User) rolü ve dahili çağrılar için sahiplik kontrolü uygulanmaz (bu metotların
    // bazıları menü/katalog görüntülemede dahili olarak da çağrılır).
    private bool SellerOwnsRestaurant(Guid restaurantId)
    {
        var token = _tokenAccessor.GetToken();
        if (token == null) return false;
        if (token.Role == UserRoleEnums.Admin) return true;
        return token.RestaurantIds != null && token.RestaurantIds.Contains(restaurantId);
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

            // Satıcı yalnızca kendi restoranının ürünlerini listeleyebilir (başka satıcının katalog/
            // fiyat yapısı sızmamalı). Müşteri/dahili çağrılar bu kontrolden etkilenmez.
            var token = _tokenAccessor.GetToken();
            if (token != null && token.Role is UserRoleEnums.SellerAdmin or UserRoleEnums.SellerUser
                && !SellerOwnsRestaurant(requestDto.RestaurantId))
            {
                result.Fail("Bu restoranın ürünlerini görüntüleme yetkiniz yok.");
                return result;
            }

            var pgProduct = await _productRepository.GetListAsync(x => x.RestaurantId == requestDto.RestaurantId, size: int.MaxValue, include: x => x.Include(a => a.ProductAttributes).ThenInclude(b => b.ProductAttributeValues).ThenInclude(c => c.Product));
            var products = pgProduct.Items.OrderBy(x => x.OrderIndex).ToList();

            if (requestDto.ProductIds != null && requestDto.ProductIds.Count != 0)
                products = products.Where(x => requestDto.ProductIds.Contains(x.Id)).ToList();

            var productDtos = products.Select(product => new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ProductType = product.ProductType,
                Price = product.Price
            }).ToList();

            result.SetData(productDtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<Guid>> CreateProduct(AddProductDto request)
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

            // Sahiplik kontrolü: satıcı yalnızca kendi restoranına ürün ekleyebilir.
            if (!SellerOwnsRestaurant(request.RestaurantId))
            {
                response.AddErrorMessage("Bu restorana ürün ekleme yetkiniz yok.");
                return response;
            }

            var product = _mapper.Map<Product>(request);
            product.Id = Guid.NewGuid();
            product.OrderIndex = request.OrderIndex;
            product.Price = request.Price;

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

            // Sahiplik kontrolü: satıcı yalnızca kendi restoranının ürününü (fiyat dahil) güncelleyebilir.
            if (!SellerOwnsRestaurant(product.RestaurantId))
            {
                response.Fail("Bu ürünü güncelleme yetkiniz yok.");
                return response;
            }

            product.Name = productRequest.Name;
            product.Description = productRequest.Description;
            product.Price = productRequest.Price;
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

            // Sahiplik kontrolü: satıcı yalnızca kendi restoranının ürününü silebilir.
            if (!SellerOwnsRestaurant(product.RestaurantId))
            {
                response.Fail("Bu ürünü silme yetkiniz yok.");
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