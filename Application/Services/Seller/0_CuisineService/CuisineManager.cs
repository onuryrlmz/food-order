using Persistence.IRepositories.Common;

namespace Application.Services.Seller._0_CuisineService;

public class CuisineManager : ICuisineService
{
    private readonly ICuisineRepository _cuisineRepository;

    public CuisineManager(ICuisineRepository cuisineRepository)
    {
        _cuisineRepository = cuisineRepository;
    }
}