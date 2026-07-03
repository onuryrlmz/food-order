using Application.Services.Common.TokenService;
using AutoMapper;
using Base.Enums;
using Domain.Dto.Seller.CategoryDetail;
using Domain.Entities.Seller;
using Domain.Service;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller.CategoryDetailService;

public class CategoryDetailService : ICategoryDetailService
{
    private readonly ICategoryDetailRepository _categoryDetailRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ITokenAccessor _tokenAccessor;

    public CategoryDetailService(IMapper mapper, ICategoryRepository categoryRepository, ICategoryDetailRepository categoryDetailRepository, ITokenAccessor tokenAccessor)
    {
        _mapper = mapper;
        _categoryRepository = categoryRepository;
        _categoryDetailRepository = categoryDetailRepository;
        _tokenAccessor = tokenAccessor;
    }

    // Satıcı bir restoranı yönetme yetkisine sahip mi? Admin tüm restoranları yönetebilir.
    private bool SellerOwnsRestaurant(Guid restaurantId)
    {
        var token = _tokenAccessor.GetToken();
        if (token == null) return false;
        if (token.Role == UserRoleEnums.Admin) return true;
        return token.RestaurantIds != null && token.RestaurantIds.Contains(restaurantId);
    }

    public async Task<ServiceCollectionResult<CategoryDetailResponse>> GetCategoryDetailsByCategoryId(GetCategoryDetailsByCategoryIdRequestDto request)
    {
        var result = new ServiceCollectionResult<CategoryDetailResponse>();
        try
        {
            var categoryDetails = await _categoryDetailRepository.GetListAsync(x => x.CategoryId == request.CategoryId);
            result.SetData(categoryDetails.Items.Select(x => _mapper.Map<CategoryDetailResponse>(x)).OrderBy(x => x.OrderIndex).ToList());
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<Guid>> CreateCategoryDetail(CreateCategoryDetailRequestDto request)
    {
        var response = new ServiceObjectResult<Guid>();
        try
        {
            var category = await _categoryRepository.GetAsync(x => x.Id == request.CategoryId);
            if (category == null)
            {
                response.Fail("Category not found.");
                return response;
            }

            // Sahiplik kontrolü: kategori-menü bağlaması yalnızca çağıranın restoranında yapılabilir.
            if (!SellerOwnsRestaurant(category.RestaurantId))
            {
                response.Fail("Bu kategoriye menü ekleme yetkiniz yok.");
                return response;
            }

            var categoryDetail = _mapper.Map<CategoryDetail>(request);
            categoryDetail.Id = Guid.NewGuid();
            categoryDetail.OrderIndex = request.OrderIndex;
            await _categoryDetailRepository.AddAsync(categoryDetail);
            response.SetData(categoryDetail.Id);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> DeleteCategoryDetail(DeleteCategoryDetailRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var categoryDetail = await _categoryDetailRepository.GetAsync(x => x.Id == request.Id && x.CategoryId == request.CategoryId);
            if (categoryDetail == null)
            {
                response.Fail("Category Detail not found");
                return response;
            }

            // Sahiplik kontrolü: bağlı kategorinin restoranı çağıranınki olmalı.
            var category = await _categoryRepository.GetAsync(x => x.Id == categoryDetail.CategoryId);
            if (category == null || !SellerOwnsRestaurant(category.RestaurantId))
            {
                response.Fail("Bu kategori-menü bağlantısını silme yetkiniz yok.");
                return response;
            }

            await _categoryDetailRepository.DeleteAsync(categoryDetail);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}