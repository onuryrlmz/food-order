using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface IRestaurantWorkingHourRepository : IAsyncRepository<RestaurantWorkingHour, Guid>, IRepository<RestaurantWorkingHour, Guid>
{
}