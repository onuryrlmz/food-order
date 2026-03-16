using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Courier;

public interface IRestaurantCourierCompanyRepository : IAsyncRepository<RestaurantCourierCompany, Guid>, IRepository<RestaurantCourierCompany, Guid>
{
}
