using Persistence.IRepositories.Common;

namespace Application.Services.Seller;

public class CuisineManager : ICuisineService
{
    private readonly ICuisineRepository _cuisineRepository;

    public CuisineManager(ICuisineRepository cuisineRepository)
    {
        _cuisineRepository = cuisineRepository;
    }
}