using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Courier;

public interface IRestaurantCourierAgreementRepository : IAsyncRepository<RestaurantCourierAgreement, Guid>, IRepository<RestaurantCourierAgreement, Guid>
{
}
