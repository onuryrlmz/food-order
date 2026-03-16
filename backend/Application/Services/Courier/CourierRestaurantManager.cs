using Base.Enums;
using Domain.Dto.Courier;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories.Courier;

namespace Application.Services.Courier;

public class CourierRestaurantManager : ICourierRestaurantService
{
    private readonly IRestaurantCourierRepository _restaurantCourierRepository;
    private readonly BaseDbContext _context;

    public CourierRestaurantManager(IRestaurantCourierRepository restaurantCourierRepository, BaseDbContext context)
    {
        _restaurantCourierRepository = restaurantCourierRepository;
        _context = context;
    }

    public async Task<ServiceCollectionResult<CourierRestaurantDto>> GetMyRestaurantsAsync(Guid userId)
    {
        var result = new ServiceCollectionResult<CourierRestaurantDto>();
        try
        {
            var relations = await _restaurantCourierRepository.GetListAsync(
                rc => rc.CourierId == userId
                    && rc.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.Active);

            result.SetData(await MapToDtos(relations.Items));
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceCollectionResult<CourierRestaurantDto>> GetPendingInvitesAsync(Guid userId)
    {
        var result = new ServiceCollectionResult<CourierRestaurantDto>();
        try
        {
            var relations = await _restaurantCourierRepository.GetListAsync(
                rc => rc.CourierId == userId
                    && rc.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.PendingApproval);

            result.SetData(await MapToDtos(relations.Items));
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> AcceptInviteAsync(Guid userId, Guid restaurantCourierId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var relation = await _restaurantCourierRepository.GetAsync(
                rc => rc.Id == restaurantCourierId && rc.CourierId == userId
                    && rc.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.PendingApproval);

            if (relation == null)
            {
                result.AddErrorMessage("Davet bulunamadı.");
                return result;
            }

            relation.StatusId = (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.Active;
            relation.AgreementStartDate = DateTime.UtcNow;
            await _restaurantCourierRepository.UpdateAsync(relation);

            result.SetData(true);
            result.AddSuccessMessage("Davet kabul edildi. Anlaşma başladı.");
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> RejectInviteAsync(Guid userId, Guid restaurantCourierId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var relation = await _restaurantCourierRepository.GetAsync(
                rc => rc.Id == restaurantCourierId && rc.CourierId == userId
                    && rc.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.PendingApproval);

            if (relation == null)
            {
                result.AddErrorMessage("Davet bulunamadı.");
                return result;
            }

            relation.StatusId = (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.RejectedByCourier;
            await _restaurantCourierRepository.UpdateAsync(relation);

            result.SetData(true);
            result.AddSuccessMessage("Davet reddedildi.");
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> LeaveRestaurantAsync(Guid userId, Guid restaurantCourierId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var relation = await _restaurantCourierRepository.GetAsync(
                rc => rc.Id == restaurantCourierId && rc.CourierId == userId
                    && rc.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.Active);

            if (relation == null)
            {
                result.AddErrorMessage("Aktif anlaşma bulunamadı.");
                return result;
            }

            relation.StatusId = (short)AuthorizationServiceEnums.RestaurantCourierStatusEnums.TerminatedByCourier;
            relation.AgreementEndDate = DateTime.UtcNow;
            await _restaurantCourierRepository.UpdateAsync(relation);

            result.SetData(true);
            result.AddSuccessMessage("Anlaşma sonlandırıldı.");
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    private async Task<List<CourierRestaurantDto>> MapToDtos(IEnumerable<Domain.Entities.Courier.RestaurantCourier> relations)
    {
        var items = relations.ToList();
        var restaurantIds = items.Select(r => r.RestaurantId).Distinct().ToList();
        var restaurants = await _context.Set<Domain.Entities.Seller.Restaurant>()
            .Where(r => restaurantIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r.Name);

        return items.Select(rc => new CourierRestaurantDto
        {
            RestaurantCourierId = rc.Id,
            RestaurantId = rc.RestaurantId,
            RestaurantName = restaurants.GetValueOrDefault(rc.RestaurantId, "Bilinmiyor"),
            StatusId = rc.StatusId,
            StatusName = ((AuthorizationServiceEnums.RestaurantCourierStatusEnums)rc.StatusId).ToString(),
            AgreementStartDate = rc.AgreementStartDate,
            CreatedDate = rc.CreatedDate
        }).ToList();
    }
}
