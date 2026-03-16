using Base.Enums;
using Domain.Dto.Seller.Courier;
using Domain.Entities.Courier;
using Domain.Service;
using Persistence.IRepositories;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Courier;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller.CourierService;

public class CourierManager : ICourierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IRestaurantCourierRepository _restaurantCourierRepository;

    public CourierManager(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IRestaurantRepository restaurantRepository,
        IRestaurantCourierRepository restaurantCourierRepository)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _restaurantRepository = restaurantRepository;
        _restaurantCourierRepository = restaurantCourierRepository;
    }

    public async Task<ServiceObjectResult<bool>> AddCourierAsync(Guid restaurantId, string email)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var restaurant = await _restaurantRepository.GetAsync(r => r.Id == restaurantId);
            if (restaurant == null)
            {
                result.Fail("Restoran bulunamadı.");
                return result;
            }

            var user = await _userRepository.GetAsync(
                u => u.Email == email &&
                     u.UserRoleId == (short)AuthorizationServiceEnums.UserRoleEnums.Courier);

            if (user == null)
            {
                result.Fail("Bu e-posta ile kayıtlı bir kurye bulunamadı.");
                return result;
            }

            if (user.UserStatusId != (short)AuthorizationServiceEnums.UserStatusEnums.Active)
            {
                result.Fail("Bu kurye henüz aktif değil.");
                return result;
            }

            var existingRelation = await _restaurantCourierRepository.GetAsync(
                rc => rc.RestaurantId == restaurantId && rc.CourierId == user.Id
                    && (rc.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.PendingApproval
                        || rc.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.Active));

            if (existingRelation != null)
            {
                result.Fail("Bu kurye zaten eklenmiş veya onay bekliyor.");
                return result;
            }

            var restaurantCourier = new RestaurantCourier
            {
                RestaurantId = restaurantId,
                CourierId = user.Id,
                StatusId = (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.PendingApproval
            };
            await _restaurantCourierRepository.AddAsync(restaurantCourier);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Kurye davet edildi. Kuryenin onayı bekleniyor.");
            return result;
        }
        catch (Exception ex)
        {
            result.Fail($"Kurye ekleme hatası: {ex.Message}");
            return result;
        }
    }

    public async Task<ServiceCollectionResult<RestaurantCourierDto>> GetRestaurantCouriersAsync(Guid restaurantId)
    {
        var result = new ServiceCollectionResult<RestaurantCourierDto>();
        try
        {
            var relations = await _restaurantCourierRepository.GetListAsync(
                rc => rc.RestaurantId == restaurantId
                    && (rc.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.PendingApproval
                        || rc.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.Active));

            var courierUserIds = relations.Items.Select(r => r.CourierId).Distinct().ToList();
            var users = (await _userRepository.GetListAsync(u => courierUserIds.Contains(u.Id)))
                .Items.ToDictionary(u => u.Id);

            var dtoList = relations.Items.Select(rc =>
            {
                users.TryGetValue(rc.CourierId, out var user);
                return new RestaurantCourierDto
                {
                    Id = rc.Id,
                    CourierId = rc.CourierId,
                    Email = user?.Email ?? "",
                    PhoneNumber = user?.PhoneNumber,
                    FirstName = user?.FirstName,
                    LastName = user?.LastName,
                    StatusId = rc.StatusId,
                    StatusName = ((AuthorizationServiceEnums.RestaurantCourierStatusEnums)rc.StatusId).ToString(),
                    CourierStatusId = user?.CourierStatusId,
                    CourierStatusName = user?.CourierStatusId != null
                        ? ((AuthorizationServiceEnums.CourierStatusEnums)user.CourierStatusId).ToString()
                        : null,
                    AgreementStartDate = rc.AgreementStartDate,
                    AgreementEndDate = rc.AgreementEndDate,
                    CreatedDate = rc.CreatedDate
                };
            }).ToList();

            result.SetData(dtoList);
            return result;
        }
        catch (Exception ex)
        {
            result.Fail($"Kurye listeleme hatası: {ex.Message}");
            return result;
        }
    }

    public async Task<ServiceObjectResult<bool>> RemoveCourierAsync(Guid restaurantId, Guid courierId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var relation = await _restaurantCourierRepository.GetAsync(
                rc => rc.RestaurantId == restaurantId && rc.CourierId == courierId
                    && (rc.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.PendingApproval
                        || rc.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.Active));

            if (relation == null)
            {
                result.Fail("Kurye bulunamadı.");
                return result;
            }

            relation.StatusId = (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.TerminatedByRestaurant;
            relation.AgreementEndDate = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Kurye anlaşması sonlandırıldı.");
            return result;
        }
        catch (Exception ex)
        {
            result.Fail($"Kurye silme hatası: {ex.Message}");
            return result;
        }
    }
}
