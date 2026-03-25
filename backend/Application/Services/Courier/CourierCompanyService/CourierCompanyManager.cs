using Application.Services.Common.TokenService;
using Base.Enums;
using Domain.Dto.Courier;
using Domain.Dto.Admin.Courier;
using Domain.Entities.Courier;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Courier.CourierCompanyService;

public class CourierCompanyManager : ICourierCompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly BaseDbContext _context;

    public CourierCompanyManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor, BaseDbContext context)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _context = context;
    }

    public async Task<ServiceObjectResult<CourierCompanyResponseDto>> Register(RegisterCourierCompanyRequestDto requestDto)
    {
        var result = new ServiceObjectResult<CourierCompanyResponseDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Kimlik doğrulama hatası."); return result; }

            var company = new CourierCompany
            {
                Id = Guid.NewGuid(),
                Name = requestDto.Name,
                LegalName = requestDto.LegalName,
                TaxCode = requestDto.TaxCode,
                TaxArea = requestDto.TaxArea,
                IBAN = requestDto.IBAN,
                Phone = requestDto.Phone,
                Email = requestDto.Email,
                ContactPerson = requestDto.ContactPerson,
                CompanyTypeId = requestDto.CompanyTypeId,
                IdentityNumber = requestDto.IdentityNumber,
                StatusId = (short)AuthorizationServiceEnums.CourierCompanyStatusEnums.Pending,
                CommissionRate = 0
            };

            await _unitOfWork.CourierCompanyRepository.AddAsync(company);
            await _unitOfWork.CompleteAsync();

            result.SetData(new CourierCompanyResponseDto
            {
                Id = company.Id,
                Name = company.Name,
                LegalName = company.LegalName,
                Phone = company.Phone,
                Email = company.Email,
                ContactPerson = company.ContactPerson,
                StatusId = company.StatusId,
                CompanyTypeId = company.CompanyTypeId,
                CommissionRate = company.CommissionRate,
                LogoUrl = company.LogoUrl,
                CourierCount = 0,
                CreatedDate = company.CreatedDate
            });
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<CourierCompanyResponseDto>> GetProfile()
    {
        var result = new ServiceObjectResult<CourierCompanyResponseDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Kimlik doğrulama hatası."); return result; }

            // Find courier company where the user is the admin (registered the company)
            // For now, find via courier entity linked to this user with CompanyMember type
            var courier = await _unitOfWork.CourierRepository.GetAsync(x => x.UserId == token.UserId);
            if (courier?.CourierCompanyId == null) { result.Fail("Firma bulunamadı."); return result; }

            var company = await _context.Set<CourierCompany>()
                .Include(c => c.Couriers)
                .FirstOrDefaultAsync(c => c.Id == courier.CourierCompanyId && c.DeletedDate == null);
            if (company == null) { result.Fail("Firma bulunamadı."); return result; }

            result.SetData(new CourierCompanyResponseDto
            {
                Id = company.Id,
                Name = company.Name,
                LegalName = company.LegalName,
                Phone = company.Phone,
                Email = company.Email,
                ContactPerson = company.ContactPerson,
                StatusId = company.StatusId,
                CompanyTypeId = company.CompanyTypeId,
                CommissionRate = company.CommissionRate,
                LogoUrl = company.LogoUrl,
                CourierCount = company.Couriers?.Count(c => c.DeletedDate == null) ?? 0,
                CreatedDate = company.CreatedDate
            });
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateProfile(RegisterCourierCompanyRequestDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Kimlik doğrulama hatası."); return result; }

            var courier = await _unitOfWork.CourierRepository.GetAsync(x => x.UserId == token.UserId);
            if (courier?.CourierCompanyId == null) { result.Fail("Firma bulunamadı."); return result; }

            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(
                x => x.Id == courier.CourierCompanyId.Value, enableTracking: true);
            if (company == null) { result.Fail("Firma bulunamadı."); return result; }

            company.Name = requestDto.Name;
            company.LegalName = requestDto.LegalName;
            company.Phone = requestDto.Phone;
            company.Email = requestDto.Email;
            company.ContactPerson = requestDto.ContactPerson;

            _unitOfWork.CourierCompanyRepository.Update(company);
            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceCollectionResult> GetMembers(int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Kimlik doğrulama hatası."); return result; }

            var courier = await _unitOfWork.CourierRepository.GetAsync(x => x.UserId == token.UserId);
            if (courier?.CourierCompanyId == null) { result.Fail("Firma bulunamadı."); return result; }

            var query = _context.Set<Domain.Entities.Courier.Courier>()
                .Include(c => c.User)
                .Where(c => c.CourierCompanyId == courier.CourierCompanyId && c.DeletedDate == null);

            var totalCount = await query.CountAsync();
            var members = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
                    TotalDeliveries = c.TotalDeliveries
                })
                .ToListAsync();

            result.RawData = members;
            result.TotalDataCount = totalCount;
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> AddMember(Guid courierId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Kimlik doğrulama hatası."); return result; }

            var adminCourier = await _unitOfWork.CourierRepository.GetAsync(x => x.UserId == token.UserId);
            if (adminCourier?.CourierCompanyId == null) { result.Fail("Firma bulunamadı."); return result; }

            var memberCourier = await _unitOfWork.CourierRepository.GetAsync(
                x => x.Id == courierId, enableTracking: true);
            if (memberCourier == null) { result.Fail("Kurye bulunamadı."); return result; }

            memberCourier.CourierCompanyId = adminCourier.CourierCompanyId;
            memberCourier.CourierTypeId = (short)AuthorizationServiceEnums.CourierTypeEnums.CompanyMember;
            _unitOfWork.CourierRepository.Update(memberCourier);
            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> RemoveMember(Guid courierId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Kimlik doğrulama hatası."); return result; }

            var adminCourier = await _unitOfWork.CourierRepository.GetAsync(x => x.UserId == token.UserId);
            if (adminCourier?.CourierCompanyId == null) { result.Fail("Firma bulunamadı."); return result; }

            var memberCourier = await _unitOfWork.CourierRepository.GetAsync(
                x => x.Id == courierId && x.CourierCompanyId == adminCourier.CourierCompanyId, enableTracking: true);
            if (memberCourier == null) { result.Fail("Kurye bulunamadı."); return result; }

            memberCourier.CourierCompanyId = null;
            memberCourier.CourierTypeId = (short)AuthorizationServiceEnums.CourierTypeEnums.Individual;
            _unitOfWork.CourierRepository.Update(memberCourier);
            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceCollectionResult> GetCompanyEarnings(DateTime? from = null, DateTime? to = null)
    {
        var result = new ServiceCollectionResult();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Kimlik doğrulama hatası."); return result; }

            var courier = await _unitOfWork.CourierRepository.GetAsync(x => x.UserId == token.UserId);
            if (courier?.CourierCompanyId == null) { result.Fail("Firma bulunamadı."); return result; }

            var companyMemberIds = await _context.Set<Domain.Entities.Courier.Courier>()
                .Where(c => c.CourierCompanyId == courier.CourierCompanyId && c.DeletedDate == null)
                .Select(c => c.Id)
                .ToListAsync();

            var query = _context.Set<CourierEarning>()
                .Where(e => companyMemberIds.Contains(e.CourierId) && e.DeletedDate == null);

            if (from.HasValue) query = query.Where(e => e.CreatedDate >= from.Value);
            if (to.HasValue) query = query.Where(e => e.CreatedDate <= to.Value);

            var earnings = await query
                .OrderByDescending(e => e.CreatedDate)
                .Select(e => new CourierEarningResponseDto
                {
                    Id = e.Id,
                    OrderId = e.OrderId,
                    DeliveryFee = e.DeliveryFee,
                    TipAmount = e.TipAmount,
                    BonusAmount = e.BonusAmount,
                    TotalEarning = e.TotalEarning,
                    IsSettled = e.IsSettled,
                    SettledAt = e.SettledAt,
                    CreatedDate = e.CreatedDate
                })
                .ToListAsync();

            result.RawData = earnings;
            result.TotalDataCount = earnings.Count;
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceCollectionResult> GetAllCompaniesForAdmin(int page = 1, int pageSize = 20, short? statusId = null)
    {
        var result = new ServiceCollectionResult();
        try
        {
            var query = _context.Set<CourierCompany>()
                .Include(c => c.Couriers)
                .Where(c => c.DeletedDate == null);

            if (statusId.HasValue)
                query = query.Where(c => c.StatusId == statusId.Value);

            var totalCount = await query.CountAsync();
            var companies = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new AdminCourierCompanyResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    LegalName = c.LegalName,
                    TaxCode = c.TaxCode,
                    TaxArea = c.TaxArea,
                    IBAN = c.IBAN,
                    Phone = c.Phone,
                    Email = c.Email,
                    ContactPerson = c.ContactPerson,
                    StatusId = c.StatusId,
                    CompanyTypeId = c.CompanyTypeId,
                    IdentityNumber = c.IdentityNumber,
                    CommissionRate = c.CommissionRate,
                    CourierCount = c.Couriers.Count(x => x.DeletedDate == null),
                    CreatedDate = c.CreatedDate
                })
                .ToListAsync();

            result.RawData = companies;
            result.TotalDataCount = totalCount;
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> ApproveCompany(Guid companyId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(
                x => x.Id == companyId, enableTracking: true);
            if (company == null) { result.Fail("Firma bulunamadı."); return result; }

            company.StatusId = (short)AuthorizationServiceEnums.CourierCompanyStatusEnums.Active;
            _unitOfWork.CourierCompanyRepository.Update(company);
            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> SuspendCompany(Guid companyId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(
                x => x.Id == companyId, enableTracking: true);
            if (company == null) { result.Fail("Firma bulunamadı."); return result; }

            company.StatusId = (short)AuthorizationServiceEnums.CourierCompanyStatusEnums.Suspended;
            _unitOfWork.CourierCompanyRepository.Update(company);
            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }
}
