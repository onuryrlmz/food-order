using Base.Enums;
using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;

namespace Application.Services.Common.BackgroundJobs;

public class CourierJobService : ICourierJobService
{
    private readonly BaseDbContext _context;
    private readonly ILogger<CourierJobService> _logger;

    public CourierJobService(BaseDbContext context, ILogger<CourierJobService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CheckExpiredAssignments()
    {
        try
        {
            var now = DateTime.UtcNow;

            var expiredAssignments = await _context.Set<DeliveryAssignment>()
                .Where(d => d.DeletedDate == null
                    && (d.StatusId == (short)AuthorizationServiceEnums.DeliveryAssignmentStatusEnums.Pending
                        || d.StatusId == (short)AuthorizationServiceEnums.DeliveryAssignmentStatusEnums.Offered)
                    && d.ExpiresAt.HasValue
                    && d.ExpiresAt.Value < now)
                .ToListAsync();

            foreach (var assignment in expiredAssignments)
            {
                assignment.StatusId = (short)AuthorizationServiceEnums.DeliveryAssignmentStatusEnums.Expired;

                // Release courier if assigned
                if (assignment.CourierId.HasValue)
                {
                    var courier = await _context.Set<Domain.Entities.Courier.Courier>()
                        .FirstOrDefaultAsync(c => c.Id == assignment.CourierId.Value);
                    if (courier != null && courier.AvailabilityStatusId == (short)AuthorizationServiceEnums.CourierAvailabilityEnums.OnDelivery)
                    {
                        courier.AvailabilityStatusId = (short)AuthorizationServiceEnums.CourierAvailabilityEnums.Online;
                    }
                }

                _logger.LogWarning("Expired assignment: {AssignmentId} for Order {OrderId}",
                    assignment.Id, assignment.OrderId);
            }

            if (expiredAssignments.Count > 0)
                await _context.SaveChangesAsync();

            _logger.LogInformation("Checked expired assignments: {ExpiredCount} expired", expiredAssignments.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking expired assignments");
        }
    }

    public async Task AutoOfflineInactiveCouriers()
    {
        try
        {
            var threshold = DateTime.UtcNow.AddMinutes(-30);

            var inactiveCouriers = await _context.Set<Domain.Entities.Courier.Courier>()
                .Where(c => c.DeletedDate == null
                    && c.AvailabilityStatusId == (short)AuthorizationServiceEnums.CourierAvailabilityEnums.Online
                    && c.LastLocationUpdate.HasValue
                    && c.LastLocationUpdate.Value < threshold)
                .ToListAsync();

            foreach (var courier in inactiveCouriers)
            {
                courier.AvailabilityStatusId = (short)AuthorizationServiceEnums.CourierAvailabilityEnums.Offline;
                _logger.LogInformation("Auto-offline courier {CourierId} due to inactivity", courier.Id);
            }

            if (inactiveCouriers.Count > 0)
                await _context.SaveChangesAsync();

            _logger.LogInformation("Auto-offline check: {Count} couriers set to offline", inactiveCouriers.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in auto-offline couriers job");
        }
    }
}
