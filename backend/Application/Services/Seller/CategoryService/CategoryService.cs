using Application.Services.Common.TokenService;
using Application.Services.Seller.CategoryDetailService;
using Application.Services.Seller.MenuService;
using AutoMapper;
using Base.Enums;
using Domain.Dto.Seller.Category;
using Domain.Dto.Seller.CategoryDetail;
using Domain.Dto.Seller.Menu;
using Domain.Entities.Seller;
using Domain.Service;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller.CategoryService;

public class CategoryService : ICategoryService
{
    private readonly ICategoryDetailService _categoryDetailService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IMenuService _menuService;
    private readonly ITokenAccessor _tokenAccessor;

    public CategoryService(IMapper mapper, ICategoryRepository categoryRepository, ICategoryDetailService categoryDetailService, IMenuService menuService, ITokenAccessor tokenAccessor)
    {
        _mapper = mapper;
        _categoryRepository = categoryRepository;
        _categoryDetailService = categoryDetailService;
        _menuService = menuService;
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

    public async Task<ServiceCollectionResult<CategoryResponse>> GetCategoriesByRestaurantId(GetCategoriesByRestaurantIdRequestDto request)
    {
        var result = new ServiceCollectionResult<CategoryResponse>();
        try
        {
            var resultCategories = await _categoryRepository.GetListAsync(x => x.RestaurantId == request.RestaurantId);
            var mappedCategories = resultCategories.Items.Select(x => _mapper.Map<CategoryResponse>(x)).OrderBy(x => x.OrderIndex).ToList();

            if (request.GetMenus)
                foreach (var mappedCategory in mappedCategories)
                {
                    var categoryDetails = await _categoryDetailService.GetCategoryDetailsByCategoryId(new GetCategoryDetailsByCategoryIdRequestDto { RestaurantId = request.RestaurantId, CategoryId = mappedCategory.Id });
                    mappedCategory.CategoryDetails = categoryDetails.Data.ToList();
                    mappedCategory.Menus = [];

                    foreach (var categoryDetail in categoryDetails.Data)
                    {
                        MenuResponseDto menu = null;

                        if (request.GetMenuProducts)
                        {
                            var responseMenu = await _menuService.GetMenuWithProductsById(new GetMenuRequestDto
                            {
                                MenuId = categoryDetail.MenuId,
                                RestaurantId = request.RestaurantId
                            });
                            menu = responseMenu.Data;
                        }
                        else
                        {
                            var responseMenu = await _menuService.GetMenuById(new GetMenuRequestDto
                            {
                                MenuId = categoryDetail.MenuId,
                                RestaurantId = request.RestaurantId
                            });
                            menu = responseMenu.Data;
                        }

                        mappedCategory.Menus.Add(menu);
                    }
                }

            result.SetData(mappedCategories);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<Guid>> CreateCategory(CreateCategoryRequestDto request)
    {
        var response = new ServiceObjectResult<Guid>();
        try
        {
            // Sahiplik kontrolü: satıcı yalnızca kendi restoranına kategori ekleyebilir.
            if (!SellerOwnsRestaurant(request.RestaurantId))
            {
                response.Fail("Bu restorana kategori ekleme yetkiniz yok.");
                return response;
            }

            var category = _mapper.Map<Category>(request);
            category.Id = Guid.NewGuid();
            category.OrderIndex = request.OrderIndex;

            await _categoryRepository.AddAsync(category);
            response.SetData(category.Id);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> UpdateCategory(UpdateCategoryRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            // Sahiplik kontrolü: satıcı yalnızca kendi restoranının kategorisini güncelleyebilir.
            if (!SellerOwnsRestaurant(request.RestaurantId))
            {
                response.Fail("Bu kategoriyi güncelleme yetkiniz yok.");
                return response;
            }

            var category = await _categoryRepository.GetAsync(x => x.Id == request.Id && x.RestaurantId == request.RestaurantId);
            if (category == null)
            {
                response.Fail("Category not found");
                return response;
            }

            category.Name = request.Name;
            category.OrderIndex = request.OrderIndex;
            await _categoryRepository.UpdateAsync(category);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> DeleteCategory(DeleteCategoryRequestDto request)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var category = await _categoryRepository.GetAsync(x => x.Id == request.Id);
            if (category == null)
            {
                response.Fail("Category not found");
                return response;
            }

            // Sahiplik kontrolü: satıcı yalnızca kendi restoranının kategorisini silebilir.
            if (!SellerOwnsRestaurant(category.RestaurantId))
            {
                response.Fail("Bu kategoriyi silme yetkiniz yok.");
                return response;
            }

            await _categoryRepository.DeleteAsync(category);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}