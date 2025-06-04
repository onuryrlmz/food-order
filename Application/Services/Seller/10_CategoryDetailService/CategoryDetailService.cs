using AutoMapper;
using Domain.Dto.Seller.CategoryDetail;
using Domain.Entities.Seller;
using Domain.Service;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller._10_CategoryDetailService;

public class CategoryDetailService : ICategoryDetailService
{
    private readonly ICategoryDetailRepository _categoryDetailRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryDetailService(IMapper mapper, ICategoryRepository categoryRepository, ICategoryDetailRepository categoryDetailRepository)
    {
        _mapper = mapper;
        _categoryRepository = categoryRepository;
        _categoryDetailRepository = categoryDetailRepository;
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