using Application.Services.Common.TokenService;
using Base.Enums;
using Domain.Dto.Courier;
using Domain.Dto.Admin.Courier;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Courier.CourierService;

public class CourierManager : ICourierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly BaseDbContext _context;

    public CourierManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor, BaseDbContext context)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _context = context;
    }

    public async Task<ServiceObjectResult<CourierProfileResponseDto>> Register(RegisterCourierRequestDto requestDto)
    {
        var result = new ServiceObjectResult<CourierProfileResponseDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            // Check if user already registered as courier
            var existing = await _unitOfWork.CourierRepository.GetAsync(x => x.UserId == token.UserId);
            if (existing != null)
            {
                result.Fail("Bu kullanıcı zaten kurye olarak kayıtlı.");
                return result;
            }

            // Validate courier type specific rules
            if (requestDto.CourierTypeId == (short)CourierTypeEnums.RestaurantOwn
                && requestDto.RestaurantId == null)
            {
                result.Fail("Restoran kuryesi için restoran ID gereklidir.");
                return result;
            }

            if (requestDto.CourierTypeId == (short)CourierTypeEnums.CompanyMember
                && requestDto.CourierCompanyId == null)
            {
                result.Fail("Firma kuryesi için firma ID gereklidir.");
                return result;
            }

            var courier = new Domain.Entities.Courier.Courier
            {
                Id = Guid.NewGuid(),
                UserId = token.UserId,
                CourierTypeId = requestDto.CourierTypeId,
                CourierCompanyId = requestDto.CourierCompanyId,
                RestaurantId = requestDto.RestaurantId,
                StatusId = (short)CourierStatusEnums.Pending,
                AvailabilityStatusId = (short)CourierAvailabilityEnums.Offline,
                VehicleType = requestDto.VehicleType,
                VehiclePlate = requestDto.VehiclePlate,
                IdentityNumber = requestDto.IdentityNumber,
                IBAN = requestDto.IBAN
            };

            await _unitOfWork.CourierRepository.AddAsync(courier);
            await _unitOfWork.CompleteAsync();

            var user = await _context.Set<Domain.Entities.Common.User>().FindAsync(token.UserId);

            result.SetData(new CourierProfileResponseDto
            {
                Id = courier.Id,
                UserId = courier.UserId,
                FirstName = user?.FirstName ?? "",
                LastName = user?.LastName ?? "",
                Phone = user?.PhoneNumber ?? "",
                CourierTypeId = courier.CourierTypeId,
                StatusId = courier.StatusId,
                AvailabilityStatusId = courier.AvailabilityStatusId,
                VehicleType = courier.VehicleType,
                VehiclePlate = courier.VehiclePlate,
                Rating = courier.Rating,
                RatingCount = courier.RatingCount,
                TotalDeliveries = courier.TotalDeliveries,
                CourierCompanyId = courier.CourierCompanyId,
                RestaurantId = courier.RestaurantId
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<CourierProfileResponseDto>> GetProfile()
    {
        var result = new ServiceObjectResult<CourierProfileResponseDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var courier = await _context.Set<Domain.Entities.Courier.Courier>()
                .Include(c => c.User)
                .Include(c => c.CourierCompany)
                .Include(c => c.Restaurant)
                .FirstOrDefaultAsync(c => c.UserId == token.UserId && c.DeletedDate == null);

            if (courier == null)
            {
                result.Fail("Kurye profili bulunamadı.");
                return result;
            }

            result.SetData(new CourierProfileResponseDto
            {
                Id = courier.Id,
                UserId = courier.UserId,
                FirstName = courier.User?.FirstName ?? "",
                LastName = courier.User?.LastName ?? "",
                Phone = courier.User?.PhoneNumber ?? "",
                CourierTypeId = courier.CourierTypeId,
                StatusId = courier.StatusId,
                AvailabilityStatusId = courier.AvailabilityStatusId,
                VehicleType = courier.VehicleType,
                VehiclePlate = courier.VehiclePlate,
                Rating = courier.Rating,
                RatingCount = courier.RatingCount,
                TotalDeliveries = courier.TotalDeliveries,
                CourierCompanyId = courier.CourierCompanyId,
                CourierCompanyName = courier.CourierCompany?.Name,
                RestaurantId = courier.RestaurantId,
                RestaurantName = courier.Restaurant?.Name
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateProfile(UpdateCourierProfileRequestDto requestDto)
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

            if (requestDto.VehicleType != null) courier.VehicleType = requestDto.VehicleType;
            if (requestDto.VehiclePlate != null) courier.VehiclePlate = requestDto.VehiclePlate;
            if (requestDto.IBAN != null) courier.IBAN = requestDto.IBAN;

            _unitOfWork.CourierRepository.Update(courier);
            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> GoOnline()
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

            if (courier.StatusId != (short)CourierStatusEnums.Active)
            {
                result.Fail("Kurye hesabınız aktif değil.");
                return result;
            }

            courier.AvailabilityStatusId = (short)CourierAvailabilityEnums.Online;
            _unitOfWork.CourierRepository.Update(courier);
            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> GoOffline()
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

            courier.AvailabilityStatusId = (short)CourierAvailabilityEnums.Offline;
            _unitOfWork.CourierRepository.Update(courier);
            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateLocation(UpdateLocationRequestDto requestDto)
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

            courier.CurrentLatitude = requestDto.Latitude;
            courier.CurrentLongitude = requestDto.Longitude;
            courier.LastLocationUpdate = DateTime.UtcNow;

            _unitOfWork.CourierRepository.Update(courier);

            // Konum geçmişine kaydet
            var activeAssignment = await _unitOfWork.DeliveryAssignmentRepository.GetAsync(
                x => x.CourierId == courier.Id && (x.StatusId == 3 || x.StatusId == 5));

            var locationHistory = new Domain.Entities.Courier.CourierLocationHistory
            {
                Id = Guid.NewGuid(),
                CourierId = courier.Id,
                DeliveryAssignmentId = activeAssignment?.Id,
                Latitude = requestDto.Latitude,
                Longitude = requestDto.Longitude,
                RecordedAt = DateTime.UtcNow,
            };
            await _unitOfWork.CourierLocationHistoryRepository.AddAsync(locationHistory);

            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult> GetAllCouriersForAdmin(int page = 1, int pageSize = 20, short? statusId = null)
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

            var query = _context.Set<Domain.Entities.Courier.Courier>()
                .Include(c => c.User)
                .Include(c => c.CourierCompany)
                .Include(c => c.Restaurant)
                .Where(c => c.DeletedDate == null);

            if (statusId.HasValue)
                query = query.Where(c => c.StatusId == statusId.Value);

            var totalCount = await query.CountAsync();
            var couriers = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new AdminCourierResponseDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    FirstName = c.User.FirstName,
                    LastName = c.User.LastName,
                    Email = c.User.Email,
                    Phone = c.User.PhoneNumber,
                    CourierTypeId = c.CourierTypeId,
                    StatusId = c.StatusId,
                    AvailabilityStatusId = c.AvailabilityStatusId,
                    VehicleType = c.VehicleType,
                    VehiclePlate = c.VehiclePlate,
                    IdentityNumber = c.IdentityNumber,
                    Rating = c.Rating,
                    TotalDeliveries = c.TotalDeliveries,
                    CourierCompanyId = c.CourierCompanyId,
                    CourierCompanyName = c.CourierCompany != null ? c.CourierCompany.Name : null,
                    RestaurantId = c.RestaurantId,
                    RestaurantName = c.Restaurant != null ? c.Restaurant.Name : null,
                    CreatedDate = c.CreatedDate
                })
                .ToListAsync();

            result.RawData = couriers;
            result.TotalDataCount = totalCount;
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> ApproveCourier(Guid courierId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var courier = await _unitOfWork.CourierRepository.GetAsync(x => x.Id == courierId, enableTracking: true);
            if (courier == null)
            {
                result.Fail("Kurye bulunamadı.");
                return result;
            }

            courier.StatusId = (short)CourierStatusEnums.Active;
            _unitOfWork.CourierRepository.Update(courier);

            // Update user role to Courier
            var user = await _context.Set<Domain.Entities.Common.User>()
                .FirstOrDefaultAsync(u => u.Id == courier.UserId);
            if (user != null)
            {
                user.UserRoleId = (short)UserRoleEnums.Courier;
                _context.Set<Domain.Entities.Common.User>().Update(user);
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

    public async Task<ServiceObjectResult<bool>> SuspendCourier(Guid courierId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var courier = await _unitOfWork.CourierRepository.GetAsync(x => x.Id == courierId, enableTracking: true);
            if (courier == null)
            {
                result.Fail("Kurye bulunamadı.");
                return result;
            }

            courier.StatusId = (short)CourierStatusEnums.Suspended;
            courier.AvailabilityStatusId = (short)CourierAvailabilityEnums.Offline;
            _unitOfWork.CourierRepository.Update(courier);
            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult> GetAvailableCouriersForRestaurant(Guid restaurantId)
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

            // Get couriers that have an active agreement with this restaurant
            var agreements = await _context.Set<Domain.Entities.Courier.RestaurantCourierAgreement>()
                .Where(a => a.RestaurantId == restaurantId
                            && a.StatusId == (short)CourierAgreementStatusEnums.Active
                            && a.DeletedDate == null)
                .ToListAsync();

            var courierIds = agreements.Where(a => a.CourierId != null).Select(a => a.CourierId!.Value).ToList();
            var companyIds = agreements.Where(a => a.CourierCompanyId != null).Select(a => a.CourierCompanyId!.Value).ToList();

            // Also include restaurant's own couriers
            var couriers = await _context.Set<Domain.Entities.Courier.Courier>()
                .Include(c => c.User)
                .Where(c => c.DeletedDate == null
                            && c.StatusId == (short)CourierStatusEnums.Active
                            && (c.RestaurantId == restaurantId
                                || courierIds.Contains(c.Id)
                                || (c.CourierCompanyId != null && companyIds.Contains(c.CourierCompanyId.Value))))
                .Select(c => new CourierProfileResponseDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    FirstName = c.User.FirstName,
                    LastName = c.User.LastName,
                    Phone = c.User.PhoneNumber,
                    CourierTypeId = c.CourierTypeId,
                    StatusId = c.StatusId,
                    AvailabilityStatusId = c.AvailabilityStatusId,
                    VehicleType = c.VehicleType,
                    VehiclePlate = c.VehiclePlate,
                    Rating = c.Rating,
                    RatingCount = c.RatingCount,
                    TotalDeliveries = c.TotalDeliveries,
                    CourierCompanyId = c.CourierCompanyId,
                    RestaurantId = c.RestaurantId
                })
                .ToListAsync();

            result.RawData = couriers;
            result.TotalDataCount = couriers.Count;
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}