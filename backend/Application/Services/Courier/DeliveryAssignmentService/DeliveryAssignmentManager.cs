using Application.Services.Common.NotificationService;
using Application.Services.Common.TokenService;
using Base.Enums;
using Domain.Dto.Courier;
using Domain.Dto.Seller.Courier;
using Domain.Entities.Buyer;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Courier.DeliveryAssignmentService;

public class DeliveryAssignmentManager : IDeliveryAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly BaseDbContext _context;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public DeliveryAssignmentManager(
        IUnitOfWork unitOfWork,
        ITokenAccessor tokenAccessor,
        BaseDbContext context,
        IRealtimeNotifier realtimeNotifier)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _context = context;
        _realtimeNotifier = realtimeNotifier;
    }

    public async Task<ServiceObjectResult<DeliveryAssignmentResponseDto>> GetActiveAssignment()
    {
        var result = new ServiceObjectResult<DeliveryAssignmentResponseDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var courier = await _unitOfWork.CourierRepository.GetAsync(x => x.UserId == token.UserId);
            if (courier == null)
            {
                result.Fail("Kurye profili bulunamadı.");
                return result;
            }

            var activeStatuses = new[]
            {
                (short)DeliveryAssignmentStatusEnums.Offered,
                (short)DeliveryAssignmentStatusEnums.Accepted,
                (short)DeliveryAssignmentStatusEnums.PickedUp
            };

            var assignment = await _context.Set<Domain.Entities.Courier.DeliveryAssignment>()
                .Include(d => d.Order)
                .Where(d => d.CourierId == courier.Id
                            && activeStatuses.Contains(d.StatusId)
                            && d.DeletedDate == null)
                .OrderByDescending(d => d.CreatedDate)
                .FirstOrDefaultAsync();

            if (assignment == null)
            {
                result.Fail("Aktif teslimat bulunamadı.");
                return result;
            }

            var restaurant = await _context.Set<Domain.Entities.Seller.Restaurant>()
                .FirstOrDefaultAsync(r => r.Id == assignment.RestaurantId);

            var deliveryAddress = await _context.Set<Domain.Entities.Common.Address>()
                .FirstOrDefaultAsync(a => a.Id == assignment.Order.DeliveryAddressId);

            var customer = await _context.Set<Domain.Entities.Common.User>()
                .FirstOrDefaultAsync(u => u.Id == assignment.Order.UserId);

            result.SetData(new DeliveryAssignmentResponseDto
            {
                Id = assignment.Id,
                OrderId = assignment.OrderId,
                RestaurantId = assignment.RestaurantId,
                RestaurantName = restaurant?.Name ?? "",
                RestaurantPhone = restaurant?.Phone ?? "",
                RestaurantLatitude = assignment.RestaurantLatitude,
                RestaurantLongitude = assignment.RestaurantLongitude,
                CustomerLatitude = assignment.CustomerLatitude,
                CustomerLongitude = assignment.CustomerLongitude,
                StatusId = assignment.StatusId,
                DeliveryFee = assignment.DeliveryFee,
                DistanceKm = assignment.DistanceKm,
                OfferedAt = assignment.OfferedAt,
                AcceptedAt = assignment.AcceptedAt,
                PickedUpAt = assignment.PickedUpAt,
                DeliveredAt = assignment.DeliveredAt,
                ExpiresAt = assignment.ExpiresAt,
                CreatedDate = assignment.CreatedDate,
                OrderTotalPrice = assignment.Order.TotalPrice,
                OrderNotes = assignment.Order.Notes,
                CustomerAddress = deliveryAddress != null ? $"{deliveryAddress.AddressLine1} {deliveryAddress.AddressLine2}" : null,
                CustomerName = customer != null ? $"{customer.FirstName} {customer.LastName}" : null,
                CustomerPhone = customer?.PhoneNumber
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult> GetAssignmentHistory(int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var courier = await _unitOfWork.CourierRepository.GetAsync(x => x.UserId == token.UserId);
            if (courier == null)
            {
                result.Fail("Kurye profili bulunamadı.");
                return result;
            }

            var query = _context.Set<Domain.Entities.Courier.DeliveryAssignment>()
                .Where(d => d.CourierId == courier.Id && d.DeletedDate == null);

            var totalCount = await query.CountAsync();
            var assignments = await query
                .OrderByDescending(d => d.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DeliveryAssignmentResponseDto
                {
                    Id = d.Id,
                    OrderId = d.OrderId,
                    RestaurantId = d.RestaurantId,
                    StatusId = d.StatusId,
                    DeliveryFee = d.DeliveryFee,
                    DistanceKm = d.DistanceKm,
                    AcceptedAt = d.AcceptedAt,
                    PickedUpAt = d.PickedUpAt,
                    DeliveredAt = d.DeliveredAt,
                    CreatedDate = d.CreatedDate
                })
                .ToListAsync();

            result.RawData = assignments;
            result.TotalDataCount = totalCount;
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> AcceptAssignment(Guid assignmentId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var courier = await _unitOfWork.CourierRepository.GetAsync(
                x => x.UserId == token.UserId, enableTracking: true);
            if (courier == null)
            {
                result.Fail("Kurye profili bulunamadı.");
                return result;
            }

            var assignment = await _unitOfWork.DeliveryAssignmentRepository.GetAsync(
                x => x.Id == assignmentId, enableTracking: true);
            if (assignment == null)
            {
                result.Fail("Teslimat ataması bulunamadı.");
                return result;
            }

            if (assignment.StatusId != (short)DeliveryAssignmentStatusEnums.Offered
                && assignment.StatusId != (short)DeliveryAssignmentStatusEnums.Pending)
            {
                result.Fail("Bu teslimat kabul edilebilir durumda değil.");
                return result;
            }

            if (assignment.ExpiresAt.HasValue && assignment.ExpiresAt.Value < DateTime.UtcNow)
            {
                result.Fail("Teslimat süresi dolmuş.");
                return result;
            }

            assignment.CourierId = courier.Id;
            assignment.StatusId = (short)DeliveryAssignmentStatusEnums.Accepted;
            assignment.AcceptedAt = DateTime.UtcNow;
            _unitOfWork.DeliveryAssignmentRepository.Update(assignment);

            // Update courier availability
            courier.AvailabilityStatusId = (short)CourierAvailabilityEnums.OnDelivery;
            _unitOfWork.CourierRepository.Update(courier);

            // Update order status and courier reference
            var order = await _unitOfWork.OrderRepository.GetAsync(
                x => x.Id == assignment.OrderId, enableTracking: true);
            if (order != null)
            {
                order.CourierId = courier.Id;
                order.DeliveryAssignmentId = assignment.Id;
                order.StatusId = (short)OrderStatusEnums.CourierAssigned;
                _unitOfWork.OrderRepository.Update(order);

                // Add status history
                await _unitOfWork.OrderStatusHistoryRepository.AddAsync(new OrderStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    StatusId = (short)OrderStatusEnums.CourierAssigned,
                    Note = $"Kurye atandı: {courier.Id}",
                    OccurredAt = DateTime.UtcNow
                });
            }

            await _unitOfWork.CompleteAsync();

            // Notify order status change
            _ = Task.Run(async () =>
            {
                try
                {
                    await _realtimeNotifier.NotifyOrderStatusChanged(assignment.OrderId, (short)OrderStatusEnums.CourierAssigned);
                }
                catch
                {
                }
            });

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> RejectAssignment(Guid assignmentId, string? reason = null)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var courier = await _unitOfWork.CourierRepository.GetAsync(x => x.UserId == token.UserId);
            if (courier == null)
            {
                result.Fail("Kurye profili bulunamadı.");
                return result;
            }

            var assignment = await _unitOfWork.DeliveryAssignmentRepository.GetAsync(
                x => x.Id == assignmentId && x.CourierId == courier.Id, enableTracking: true);
            if (assignment == null)
            {
                result.Fail("Teslimat ataması bulunamadı.");
                return result;
            }

            assignment.StatusId = (short)DeliveryAssignmentStatusEnums.Rejected;
            assignment.RejectedAt = DateTime.UtcNow;
            assignment.RejectionReason = reason;
            _unitOfWork.DeliveryAssignmentRepository.Update(assignment);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> MarkPickedUp(Guid assignmentId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var courier = await _unitOfWork.CourierRepository.GetAsync(x => x.UserId == token.UserId);
            if (courier == null)
            {
                result.Fail("Kurye profili bulunamadı.");
                return result;
            }

            var assignment = await _unitOfWork.DeliveryAssignmentRepository.GetAsync(
                x => x.Id == assignmentId && x.CourierId == courier.Id, enableTracking: true);
            if (assignment == null)
            {
                result.Fail("Teslimat ataması bulunamadı.");
                return result;
            }

            if (assignment.StatusId != (short)DeliveryAssignmentStatusEnums.Accepted)
            {
                result.Fail("Teslimat henüz kabul edilmedi.");
                return result;
            }

            assignment.StatusId = (short)DeliveryAssignmentStatusEnums.PickedUp;
            assignment.PickedUpAt = DateTime.UtcNow;
            _unitOfWork.DeliveryAssignmentRepository.Update(assignment);

            // Update order
            var order = await _unitOfWork.OrderRepository.GetAsync(
                x => x.Id == assignment.OrderId, enableTracking: true);
            if (order != null)
            {
                order.StatusId = (short)OrderStatusEnums.CourierPickedUp;
                order.PickedUpAt = DateTime.UtcNow;
                _unitOfWork.OrderRepository.Update(order);

                await _unitOfWork.OrderStatusHistoryRepository.AddAsync(new OrderStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    StatusId = (short)OrderStatusEnums.CourierPickedUp,
                    Note = "Kurye siparişi teslim aldı",
                    OccurredAt = DateTime.UtcNow
                });
            }

            await _unitOfWork.CompleteAsync();

            _ = Task.Run(async () =>
            {
                try
                {
                    await _realtimeNotifier.NotifyOrderStatusChanged(assignment.OrderId, (short)OrderStatusEnums.CourierPickedUp);
                }
                catch
                {
                }
            });

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> MarkDelivered(Guid assignmentId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var courier = await _unitOfWork.CourierRepository.GetAsync(
                x => x.UserId == token.UserId, enableTracking: true);
            if (courier == null)
            {
                result.Fail("Kurye profili bulunamadı.");
                return result;
            }

            var assignment = await _unitOfWork.DeliveryAssignmentRepository.GetAsync(
                x => x.Id == assignmentId && x.CourierId == courier.Id, enableTracking: true);
            if (assignment == null)
            {
                result.Fail("Teslimat ataması bulunamadı.");
                return result;
            }

            if (assignment.StatusId != (short)DeliveryAssignmentStatusEnums.PickedUp)
            {
                result.Fail("Sipariş henüz teslim alınmadı.");
                return result;
            }

            assignment.StatusId = (short)DeliveryAssignmentStatusEnums.Delivered;
            assignment.DeliveredAt = DateTime.UtcNow;
            _unitOfWork.DeliveryAssignmentRepository.Update(assignment);

            // Update order
            var order = await _unitOfWork.OrderRepository.GetAsync(
                x => x.Id == assignment.OrderId, enableTracking: true);
            if (order != null)
            {
                order.StatusId = (short)OrderStatusEnums.Delivered;
                order.DeliveredAt = DateTime.UtcNow;
                _unitOfWork.OrderRepository.Update(order);

                await _unitOfWork.OrderStatusHistoryRepository.AddAsync(new OrderStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    StatusId = (short)OrderStatusEnums.Delivered,
                    Note = "Kurye teslim etti",
                    OccurredAt = DateTime.UtcNow
                });
            }

            // Update courier availability back to online
            courier.AvailabilityStatusId = (short)CourierAvailabilityEnums.Online;
            courier.TotalDeliveries += 1;
            _unitOfWork.CourierRepository.Update(courier);

            await _unitOfWork.CompleteAsync();

            _ = Task.Run(async () =>
            {
                try
                {
                    await _realtimeNotifier.NotifyOrderStatusChanged(assignment.OrderId, (short)OrderStatusEnums.Delivered);
                }
                catch
                {
                }
            });

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<DeliveryAssignmentResponseDto>> CreateAssignment(Guid orderId)
    {
        var result = new ServiceObjectResult<DeliveryAssignmentResponseDto>();
        try
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(x => x.Id == orderId);
            if (order == null)
            {
                result.Fail("Sipariş bulunamadı.");
                return result;
            }

            var restaurant = await _context.Set<Domain.Entities.Seller.Restaurant>()
                .FirstOrDefaultAsync(r => r.Id == order.RestaurantId);
            if (restaurant == null)
            {
                result.Fail("Restoran bulunamadı.");
                return result;
            }

            var deliveryAddress = await _context.Set<Domain.Entities.Common.Address>()
                .FirstOrDefaultAsync(a => a.Id == order.DeliveryAddressId);

            // Find best agreement based on priority
            var agreement = await _context.Set<Domain.Entities.Courier.RestaurantCourierAgreement>()
                .Where(a => a.RestaurantId == order.RestaurantId
                            && a.StatusId == (short)CourierAgreementStatusEnums.Active
                            && a.DeletedDate == null
                            && (!a.EffectiveUntil.HasValue || a.EffectiveUntil.Value >= DateTime.UtcNow))
                .OrderBy(a => a.Priority)
                .FirstOrDefaultAsync();

            var strategy = agreement?.AssignmentStrategyId
                           ?? restaurant.DefaultAssignmentStrategyId
                           ?? (short)CourierAssignmentStrategyEnums.AutoAssignNearest;

            var assignmentEntity = new Domain.Entities.Courier.DeliveryAssignment
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                RestaurantId = order.RestaurantId,
                CourierCompanyId = agreement?.CourierCompanyId,
                AgreementId = agreement?.Id,
                StatusId = (short)DeliveryAssignmentStatusEnums.Pending,
                AssignmentStrategyId = strategy,
                DeliveryFee = agreement?.AgreedDeliveryFee,
                RestaurantLatitude = restaurant.Latitude,
                RestaurantLongitude = restaurant.Longitude,
                CustomerLatitude = decimal.TryParse(deliveryAddress?.Latitude, out var lat) ? lat : null,
                CustomerLongitude = decimal.TryParse(deliveryAddress?.Longitude, out var lng) ? lng : null,
                ExpiresAt = DateTime.UtcNow.AddMinutes(3),
                AttemptNumber = 1
            };

            // Auto-assign nearest courier
            if (strategy == (short)CourierAssignmentStrategyEnums.AutoAssignNearest
                && restaurant.Latitude.HasValue && restaurant.Longitude.HasValue)
            {
                var nearestCourier = await FindNearestAvailableCourier(order.RestaurantId, restaurant.Latitude.Value, restaurant.Longitude.Value);
                if (nearestCourier != null)
                {
                    assignmentEntity.CourierId = nearestCourier.Id;
                    assignmentEntity.StatusId = (short)DeliveryAssignmentStatusEnums.Offered;
                    assignmentEntity.OfferedAt = DateTime.UtcNow;
                }
            }

            await _unitOfWork.DeliveryAssignmentRepository.AddAsync(assignmentEntity);
            await _unitOfWork.CompleteAsync();

            result.SetData(new DeliveryAssignmentResponseDto
            {
                Id = assignmentEntity.Id,
                OrderId = assignmentEntity.OrderId,
                RestaurantId = assignmentEntity.RestaurantId,
                RestaurantName = restaurant.Name,
                RestaurantPhone = restaurant.Phone,
                StatusId = assignmentEntity.StatusId,
                DeliveryFee = assignmentEntity.DeliveryFee,
                CreatedDate = assignmentEntity.CreatedDate,
                ExpiresAt = assignmentEntity.ExpiresAt
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    private async Task<Domain.Entities.Courier.Courier?> FindNearestAvailableCourier(Guid restaurantId, decimal restaurantLat, decimal restaurantLng)
    {
        // Get couriers with active agreements for this restaurant
        var agreements = await _context.Set<Domain.Entities.Courier.RestaurantCourierAgreement>()
            .Where(a => a.RestaurantId == restaurantId
                        && a.StatusId == (short)CourierAgreementStatusEnums.Active
                        && a.DeletedDate == null)
            .ToListAsync();

        var courierIds = agreements.Where(a => a.CourierId != null).Select(a => a.CourierId!.Value).ToList();
        var companyIds = agreements.Where(a => a.CourierCompanyId != null).Select(a => a.CourierCompanyId!.Value).ToList();

        var candidates = await _context.Set<Domain.Entities.Courier.Courier>()
            .Where(c => c.DeletedDate == null
                        && c.StatusId == (short)CourierStatusEnums.Active
                        && c.AvailabilityStatusId == (short)CourierAvailabilityEnums.Online
                        && c.CurrentLatitude != null && c.CurrentLongitude != null
                        && (c.RestaurantId == restaurantId
                            || courierIds.Contains(c.Id)
                            || (c.CourierCompanyId != null && companyIds.Contains(c.CourierCompanyId.Value))))
            .ToListAsync();

        if (!candidates.Any()) return null;

        // Simple distance calculation (Haversine approximation)
        return candidates
            .OrderBy(c => Math.Pow((double)(c.CurrentLatitude!.Value - restaurantLat), 2) +
                          Math.Pow((double)(c.CurrentLongitude!.Value - restaurantLng), 2))
            .FirstOrDefault();
    }

    public async Task<ServiceObjectResult<bool>> AssignManually(Guid restaurantId, Guid orderId, Guid courierId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var order = await _unitOfWork.OrderRepository.GetAsync(x => x.Id == orderId, enableTracking: true);
            if (order == null || order.RestaurantId != restaurantId)
            {
                result.Fail("Sipariş bulunamadı.");
                return result;
            }

            var courier = await _unitOfWork.CourierRepository.GetAsync(x => x.Id == courierId);
            if (courier == null)
            {
                result.Fail("Kurye bulunamadı.");
                return result;
            }

            var restaurant = await _context.Set<Domain.Entities.Seller.Restaurant>()
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            var deliveryAddress = await _context.Set<Domain.Entities.Common.Address>()
                .FirstOrDefaultAsync(a => a.Id == order.DeliveryAddressId);

            var assignment = new Domain.Entities.Courier.DeliveryAssignment
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                RestaurantId = restaurantId,
                CourierId = courierId,
                StatusId = (short)DeliveryAssignmentStatusEnums.Offered,
                AssignmentStrategyId = (short)CourierAssignmentStrategyEnums.ManualByRestaurant,
                RestaurantLatitude = restaurant?.Latitude,
                RestaurantLongitude = restaurant?.Longitude,
                CustomerLatitude = decimal.TryParse(deliveryAddress?.Latitude, out var lat) ? lat : null,
                CustomerLongitude = decimal.TryParse(deliveryAddress?.Longitude, out var lng) ? lng : null,
                OfferedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                AttemptNumber = 1
            };

            await _unitOfWork.DeliveryAssignmentRepository.AddAsync(assignment);
            order.DeliveryAssignmentId = assignment.Id;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> CancelAssignment(Guid assignmentId, string reason)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var assignment = await _unitOfWork.DeliveryAssignmentRepository.GetAsync(
                x => x.Id == assignmentId, enableTracking: true);
            if (assignment == null)
            {
                result.Fail("Teslimat ataması bulunamadı.");
                return result;
            }

            assignment.StatusId = (short)DeliveryAssignmentStatusEnums.Cancelled;
            assignment.CancelledAt = DateTime.UtcNow;
            assignment.CancellationReason = reason;
            _unitOfWork.DeliveryAssignmentRepository.Update(assignment);

            // Release courier
            if (assignment.CourierId.HasValue)
            {
                var courier = await _unitOfWork.CourierRepository.GetAsync(
                    x => x.Id == assignment.CourierId.Value, enableTracking: true);
                if (courier != null)
                {
                    courier.AvailabilityStatusId = (short)CourierAvailabilityEnums.Online;
                    _unitOfWork.CourierRepository.Update(courier);
                }
            }

            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult> GetActiveDeliveries(Guid restaurantId)
    {
        var result = new ServiceCollectionResult();
        try
        {
            var activeStatuses = new[]
            {
                (short)DeliveryAssignmentStatusEnums.Pending,
                (short)DeliveryAssignmentStatusEnums.Offered,
                (short)DeliveryAssignmentStatusEnums.Accepted,
                (short)DeliveryAssignmentStatusEnums.PickedUp
            };

            var assignments = await _context.Set<Domain.Entities.Courier.DeliveryAssignment>()
                .Include(d => d.Courier).ThenInclude(c => c!.User)
                .Where(d => d.RestaurantId == restaurantId
                            && activeStatuses.Contains(d.StatusId)
                            && d.DeletedDate == null)
                .OrderByDescending(d => d.CreatedDate)
                .Select(d => new DeliveryAssignmentResponseDto
                {
                    Id = d.Id,
                    OrderId = d.OrderId,
                    RestaurantId = d.RestaurantId,
                    StatusId = d.StatusId,
                    DeliveryFee = d.DeliveryFee,
                    DistanceKm = d.DistanceKm,
                    OfferedAt = d.OfferedAt,
                    AcceptedAt = d.AcceptedAt,
                    PickedUpAt = d.PickedUpAt,
                    ExpiresAt = d.ExpiresAt,
                    CreatedDate = d.CreatedDate
                })
                .ToListAsync();

            result.RawData = assignments;
            result.TotalDataCount = assignments.Count;
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult> GetAgreements(Guid restaurantId)
    {
        var result = new ServiceCollectionResult();
        try
        {
            var agreements = await _context.Set<Domain.Entities.Courier.RestaurantCourierAgreement>()
                .Include(a => a.CourierCompany)
                .Include(a => a.Courier).ThenInclude(c => c!.User)
                .Where(a => a.RestaurantId == restaurantId && a.DeletedDate == null)
                .OrderBy(a => a.Priority)
                .Select(a => new AgreementResponseDto
                {
                    Id = a.Id,
                    RestaurantId = a.RestaurantId,
                    CourierCompanyId = a.CourierCompanyId,
                    CourierCompanyName = a.CourierCompany != null ? a.CourierCompany.Name : null,
                    CourierId = a.CourierId,
                    CourierName = a.Courier != null && a.Courier.User != null ? a.Courier.User.FirstName + " " + a.Courier.User.LastName : null,
                    StatusId = a.StatusId,
                    AssignmentStrategyId = a.AssignmentStrategyId,
                    AgreedDeliveryFee = a.AgreedDeliveryFee,
                    PerKmFee = a.PerKmFee,
                    Priority = a.Priority,
                    IsDefault = a.IsDefault,
                    EffectiveFrom = a.EffectiveFrom,
                    EffectiveUntil = a.EffectiveUntil,
                    CreatedDate = a.CreatedDate
                })
                .ToListAsync();

            result.RawData = agreements;
            result.TotalDataCount = agreements.Count;
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<AgreementResponseDto>> CreateAgreement(Guid restaurantId, CreateAgreementRequestDto requestDto)
    {
        var result = new ServiceObjectResult<AgreementResponseDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var agreement = new Domain.Entities.Courier.RestaurantCourierAgreement
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                CourierCompanyId = requestDto.CourierCompanyId,
                CourierId = requestDto.CourierId,
                StatusId = (short)CourierAgreementStatusEnums.Active,
                AssignmentStrategyId = requestDto.AssignmentStrategyId,
                AgreedDeliveryFee = requestDto.AgreedDeliveryFee,
                PerKmFee = requestDto.PerKmFee,
                Priority = requestDto.Priority,
                IsDefault = requestDto.IsDefault,
                EffectiveFrom = requestDto.EffectiveFrom,
                EffectiveUntil = requestDto.EffectiveUntil
            };

            await _unitOfWork.RestaurantCourierAgreementRepository.AddAsync(agreement);
            await _unitOfWork.CompleteAsync();

            result.SetData(new AgreementResponseDto
            {
                Id = agreement.Id,
                RestaurantId = agreement.RestaurantId,
                CourierCompanyId = agreement.CourierCompanyId,
                CourierId = agreement.CourierId,
                StatusId = agreement.StatusId,
                AssignmentStrategyId = agreement.AssignmentStrategyId,
                AgreedDeliveryFee = agreement.AgreedDeliveryFee,
                PerKmFee = agreement.PerKmFee,
                Priority = agreement.Priority,
                IsDefault = agreement.IsDefault,
                EffectiveFrom = agreement.EffectiveFrom,
                EffectiveUntil = agreement.EffectiveUntil,
                CreatedDate = agreement.CreatedDate
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateAgreement(Guid agreementId, CreateAgreementRequestDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var agreement = await _unitOfWork.RestaurantCourierAgreementRepository.GetAsync(
                x => x.Id == agreementId, enableTracking: true);
            if (agreement == null)
            {
                result.Fail("Anlaşma bulunamadı.");
                return result;
            }

            agreement.AssignmentStrategyId = requestDto.AssignmentStrategyId;
            agreement.AgreedDeliveryFee = requestDto.AgreedDeliveryFee;
            agreement.PerKmFee = requestDto.PerKmFee;
            agreement.Priority = requestDto.Priority;
            agreement.IsDefault = requestDto.IsDefault;
            agreement.EffectiveFrom = requestDto.EffectiveFrom;
            agreement.EffectiveUntil = requestDto.EffectiveUntil;

            _unitOfWork.RestaurantCourierAgreementRepository.Update(agreement);
            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> TerminateAgreement(Guid agreementId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var agreement = await _unitOfWork.RestaurantCourierAgreementRepository.GetAsync(
                x => x.Id == agreementId, enableTracking: true);
            if (agreement == null)
            {
                result.Fail("Anlaşma bulunamadı.");
                return result;
            }

            agreement.StatusId = (short)CourierAgreementStatusEnums.Terminated;
            _unitOfWork.RestaurantCourierAgreementRepository.Update(agreement);
            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateDeliverySettings(Guid restaurantId, UpdateDeliverySettingsRequestDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var restaurant = await _context.Set<Domain.Entities.Seller.Restaurant>()
                .FirstOrDefaultAsync(r => r.Id == restaurantId);
            if (restaurant == null)
            {
                result.Fail("Restoran bulunamadı.");
                return result;
            }

            restaurant.DefaultAssignmentStrategyId = requestDto.DefaultAssignmentStrategyId;
            restaurant.HasOwnCouriers = requestDto.HasOwnCouriers;
            _context.Set<Domain.Entities.Seller.Restaurant>().Update(restaurant);
            await _context.SaveChangesAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<CourierTrackingResponseDto>> GetOrderTracking(Guid orderId)
    {
        var result = new ServiceObjectResult<CourierTrackingResponseDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var order = await _unitOfWork.OrderRepository.GetAsync(x => x.Id == orderId);
            if (order == null)
            {
                result.Fail("Sipariş bulunamadı.");
                return result;
            }

            if (order.CourierId == null)
            {
                result.SetData(new CourierTrackingResponseDto());
                return result;
            }

            var courier = await _context.Set<Domain.Entities.Courier.Courier>()
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == order.CourierId);

            var assignment = order.DeliveryAssignmentId.HasValue
                ? await _unitOfWork.DeliveryAssignmentRepository.GetAsync(x => x.Id == order.DeliveryAssignmentId.Value)
                : null;

            result.SetData(new CourierTrackingResponseDto
            {
                CourierId = courier?.Id,
                CourierName = courier?.User != null ? $"{courier.User.FirstName} {courier.User.LastName}" : null,
                CourierPhone = courier?.User?.PhoneNumber,
                VehicleType = courier?.VehicleType,
                Latitude = courier?.CurrentLatitude,
                Longitude = courier?.CurrentLongitude,
                LastLocationUpdate = courier?.LastLocationUpdate,
                AssignmentStatusId = assignment?.StatusId
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}