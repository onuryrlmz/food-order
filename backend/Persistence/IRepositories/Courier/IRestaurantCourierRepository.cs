using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Courier;

public interface IRestaurantCourierRepository : IAsyncRepository<RestaurantCourier, Guid>, IRepository<RestaurantCourier, Guid>
{
}
