using Base.Enums;
using Domain.Dto.Courier.Company;
using Domain.Entities.Courier;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;
using Persistence.IRepositories.Common;

namespace Application.Services.Courier.CourierCompanyService;

public class CourierCompanyManager : ICourierCompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly BaseDbContext _context;

    public CourierCompanyManager(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        BaseDbContext context)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _context = context;
    }

    #region Company Management

    public async Task<ServiceObjectResult<CourierCompanyDto>> RegisterCompanyAsync(Guid ownerUserId, RegisterCourierCompanyRequestDto request)
    {
        var result = new ServiceObjectResult<CourierCompanyDto>();
        try
        {
            var user = await _userRepository.GetAsync(u => u.Id == ownerUserId);
            if (user == null || user.UserRoleId != (short)AuthorizationServiceEnums.UserRoleEnums.Courier)
            {
                result.Fail("Sadece kuryeler firma kaydı yapabilir.");
                return result;
            }

            var existing = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.OwnerUserId == ownerUserId);
            if (existing != null)
            {
                result.Fail("Zaten bir firmaya sahipsiniz.");
                return result;
            }

            var company = new CourierCompany
            {
                Name = request.Name,
                LegalName = request.LegalName,
                TaxCode = request.TaxCode,
                TaxArea = request.TaxArea,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                OwnerUserId = ownerUserId,
                StatusId = (short)AuthorizationServiceEnums.CourierCompanyStatusEnums.PendingApproval
            };

            await _unitOfWork.CourierCompanyRepository.AddAsync(company);
            await _unitOfWork.CompleteAsync();

            result.SetData(new CourierCompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                LegalName = company.LegalName,
                ContactEmail = company.ContactEmail,
                ContactPhone = company.ContactPhone,
                StatusId = company.StatusId,
                StatusName = "Onay Bekliyor",
                MemberCount = 0,
                CreatedDate = company.CreatedDate
            });
            result.AddSuccessMessage("Firma kaydı oluşturuldu, admin onayı bekleniyor.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceObjectResult<CourierCompanyDto>> GetMyCompanyAsync(Guid ownerUserId)
    {
        var result = new ServiceObjectResult<CourierCompanyDto>();
        try
        {
            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.OwnerUserId == ownerUserId);
            if (company == null)
            {
                result.Fail("Firma bulunamadı.");
                return result;
            }

            var memberCount = (await _unitOfWork.CourierCompanyMemberRepository.GetListAsync(
                m => m.CourierCompanyId == company.Id &&
                     m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.Active)).Items.Count;

            result.SetData(MapCompanyToDto(company, memberCount));
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateCompanyAsync(Guid ownerUserId, UpdateCourierCompanyRequestDto request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.OwnerUserId == ownerUserId, enableTracking: true);
            if (company == null)
            {
                result.Fail("Firma bulunamadı.");
                return result;
            }

            company.Name = request.Name;
            company.LegalName = request.LegalName;
            company.TaxCode = request.TaxCode;
            company.TaxArea = request.TaxArea;
            company.ContactPhone = request.ContactPhone;

            _unitOfWork.CourierCompanyRepository.Update(company);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Firma bilgileri güncellendi.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    #endregion

    #region Member Management

    public async Task<ServiceCollectionResult<CourierCompanyMemberDto>> SearchCouriersAsync(string query)
    {
        var result = new ServiceCollectionResult<CourierCompanyMemberDto>();
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
            {
                result.Fail("Arama sorgusu en az 3 karakter olmalıdır.");
                return result;
            }

            var users = await _userRepository.GetListAsync(
                u => u.UserRoleId == (short)AuthorizationServiceEnums.UserRoleEnums.Courier &&
                     u.UserStatusId == (short)AuthorizationServiceEnums.UserStatusEnums.Active &&
                     (u.Email.Contains(query) || u.FirstName.Contains(query) || u.LastName.Contains(query)),
                size: 20);

            var dtos = users.Items.Select(u => new CourierCompanyMemberDto
            {
                CourierId = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber
            }).ToList();

            result.SetData(dtos);
        }
        catch (Exception ex)
        {
            result.Fail($"Arama hatası: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> RequestCourierMembershipAsync(Guid ownerUserId, Guid courierId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.OwnerUserId == ownerUserId);
            if (company == null)
            {
                result.Fail("Firma bulunamadı.");
                return result;
            }

            // Check company is active
            if (company.StatusId != (short)AuthorizationServiceEnums.CourierCompanyStatusEnums.Active)
            {
                result.Fail("Firmanız aktif değil, üye talebi gönderilemez.");
                return result;
            }

            // Check target user is a courier
            var courierUser = await _userRepository.GetAsync(u => u.Id == courierId);
            if (courierUser == null || courierUser.UserRoleId != (short)AuthorizationServiceEnums.UserRoleEnums.Courier)
            {
                result.Fail("Hedef kullanıcı kurye değil.");
                return result;
            }

            // Check courier not already in another company (active or pending)
            var existingMembership = await _unitOfWork.CourierCompanyMemberRepository.GetAsync(
                m => m.CourierId == courierId &&
                     (m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.Active ||
                      m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.PendingApproval));
            if (existingMembership != null)
            {
                result.Fail("Bu kurye zaten bir firmaya bağlı veya bekleyen bir talebi var.");
                return result;
            }

            // Check target user is not a company owner
            var isOwner = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.OwnerUserId == courierId);
            if (isOwner != null)
            {
                result.Fail("Firma sahibi başka bir firmaya bağlanamaz.");
                return result;
            }

            var member = new CourierCompanyMember
            {
                CourierCompanyId = company.Id,
                CourierId = courierId,
                StatusId = (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.PendingApproval,
                RequestedAt = DateTime.UtcNow
            };

            await _unitOfWork.CourierCompanyMemberRepository.AddAsync(member);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Üyelik talebi gönderildi.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceCollectionResult<CourierCompanyMemberDto>> GetCompanyMembersAsync(Guid ownerUserId)
    {
        var result = new ServiceCollectionResult<CourierCompanyMemberDto>();
        try
        {
            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.OwnerUserId == ownerUserId);
            if (company == null)
            {
                result.Fail("Firma bulunamadı.");
                return result;
            }

            var members = await _unitOfWork.CourierCompanyMemberRepository.GetListAsync(
                m => m.CourierCompanyId == company.Id &&
                     (m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.Active ||
                      m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.PendingApproval),
                size: 200);

            var courierIds = members.Items.Select(m => m.CourierId).Distinct().ToList();
            var users = (await _userRepository.GetListAsync(u => courierIds.Contains(u.Id), size: courierIds.Count))
                .Items.ToDictionary(u => u.Id);

            var dtos = members.Items.Select(m =>
            {
                users.TryGetValue(m.CourierId, out var user);
                return new CourierCompanyMemberDto
                {
                    Id = m.Id,
                    CourierId = m.CourierId,
                    FirstName = user?.FirstName,
                    LastName = user?.LastName,
                    Email = user?.Email ?? "",
                    PhoneNumber = user?.PhoneNumber,
                    StatusId = m.StatusId,
                    StatusName = ((AuthorizationServiceEnums.CourierCompanyMemberStatusEnums)m.StatusId).ToString(),
                    RequestedAt = m.RequestedAt,
                    ApprovedAt = m.ApprovedAt
                };
            }).ToList();

            result.SetData(dtos);
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> RemoveMemberAsync(Guid ownerUserId, Guid memberId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.OwnerUserId == ownerUserId);
            if (company == null)
            {
                result.Fail("Firma bulunamadı.");
                return result;
            }

            var member = await _unitOfWork.CourierCompanyMemberRepository.GetAsync(
                m => m.Id == memberId && m.CourierCompanyId == company.Id, enableTracking: true);
            if (member == null)
            {
                result.Fail("Üye bulunamadı.");
                return result;
            }

            member.StatusId = (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.RemovedByCompany;
            _unitOfWork.CourierCompanyMemberRepository.Update(member);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Üye firmadan çıkarıldı.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    #endregion

    #region Courier Side

    public async Task<ServiceCollectionResult<CompanyInviteDto>> GetCompanyInvitesAsync(Guid courierId)
    {
        var result = new ServiceCollectionResult<CompanyInviteDto>();
        try
        {
            var invites = await _unitOfWork.CourierCompanyMemberRepository.GetListAsync(
                m => m.CourierId == courierId &&
                     m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.PendingApproval);

            var companyIds = invites.Items.Select(i => i.CourierCompanyId).Distinct().ToList();
            var companies = (await _unitOfWork.CourierCompanyRepository.GetListAsync(c => companyIds.Contains(c.Id), size: companyIds.Count))
                .Items.ToDictionary(c => c.Id);

            var dtos = invites.Items.Select(i =>
            {
                companies.TryGetValue(i.CourierCompanyId, out var company);
                return new CompanyInviteDto
                {
                    Id = i.Id,
                    CourierCompanyId = i.CourierCompanyId,
                    CompanyName = company?.Name ?? "",
                    ContactEmail = company?.ContactEmail ?? "",
                    RequestedAt = i.RequestedAt
                };
            }).ToList();

            result.SetData(dtos);
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> AcceptCompanyInviteAsync(Guid courierId, Guid inviteId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var invite = await _unitOfWork.CourierCompanyMemberRepository.GetAsync(
                m => m.Id == inviteId && m.CourierId == courierId &&
                     m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.PendingApproval,
                enableTracking: true);

            if (invite == null)
            {
                result.Fail("Davet bulunamadı.");
                return result;
            }

            invite.StatusId = (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.Active;
            invite.ApprovedAt = DateTime.UtcNow;
            _unitOfWork.CourierCompanyMemberRepository.Update(invite);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Firma davetini kabul ettiniz.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> RejectCompanyInviteAsync(Guid courierId, Guid inviteId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var invite = await _unitOfWork.CourierCompanyMemberRepository.GetAsync(
                m => m.Id == inviteId && m.CourierId == courierId &&
                     m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.PendingApproval,
                enableTracking: true);

            if (invite == null)
            {
                result.Fail("Davet bulunamadı.");
                return result;
            }

            invite.StatusId = (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.LeftByChoice;
            _unitOfWork.CourierCompanyMemberRepository.Update(invite);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Firma daveti reddedildi.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> LeaveCompanyAsync(Guid courierId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var membership = await _unitOfWork.CourierCompanyMemberRepository.GetAsync(
                m => m.CourierId == courierId &&
                     m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.Active,
                enableTracking: true);

            if (membership == null)
            {
                result.Fail("Aktif bir firma üyeliğiniz yok.");
                return result;
            }

            membership.StatusId = (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.LeftByChoice;
            _unitOfWork.CourierCompanyMemberRepository.Update(membership);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Firmadan ayrıldınız.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    #endregion

    #region Restaurant <-> Company

    public async Task<ServiceObjectResult<bool>> InviteCompanyToRestaurantAsync(Guid restaurantId, Guid companyId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.Id == companyId);
            if (company == null)
            {
                result.Fail("Kurye firması bulunamadı.");
                return result;
            }

            if (company.StatusId != (short)AuthorizationServiceEnums.CourierCompanyStatusEnums.Active)
            {
                result.Fail("Firma aktif değil.");
                return result;
            }

            var existing = await _unitOfWork.RestaurantCourierCompanyRepository.GetAsync(
                r => r.RestaurantId == restaurantId && r.CourierCompanyId == companyId &&
                     (r.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.PendingApproval ||
                      r.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.Active));

            if (existing != null)
            {
                result.Fail("Bu firma zaten eklenmiş veya onay bekliyor.");
                return result;
            }

            var relation = new RestaurantCourierCompany
            {
                RestaurantId = restaurantId,
                CourierCompanyId = companyId,
                StatusId = (short)AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.PendingApproval
            };

            await _unitOfWork.RestaurantCourierCompanyRepository.AddAsync(relation);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Kurye firmasına davet gönderildi.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceCollectionResult<RestaurantCourierCompanyDto>> GetRestaurantCompaniesAsync(Guid restaurantId)
    {
        var result = new ServiceCollectionResult<RestaurantCourierCompanyDto>();
        try
        {
            var relations = await _unitOfWork.RestaurantCourierCompanyRepository.GetListAsync(
                r => r.RestaurantId == restaurantId &&
                     (r.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.PendingApproval ||
                      r.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.Active));

            var companyIds = relations.Items.Select(r => r.CourierCompanyId).Distinct().ToList();
            var companies = (await _unitOfWork.CourierCompanyRepository.GetListAsync(c => companyIds.Contains(c.Id), size: companyIds.Count))
                .Items.ToDictionary(c => c.Id);

            var dtos = relations.Items.Select(r =>
            {
                companies.TryGetValue(r.CourierCompanyId, out var company);
                return new RestaurantCourierCompanyDto
                {
                    Id = r.Id,
                    CourierCompanyId = r.CourierCompanyId,
                    CompanyName = company?.Name ?? "",
                    StatusId = r.StatusId,
                    StatusName = ((AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums)r.StatusId).ToString(),
                    AgreementStartDate = r.AgreementStartDate
                };
            }).ToList();

            result.SetData(dtos);
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> RemoveCompanyFromRestaurantAsync(Guid restaurantId, Guid companyId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var relation = await _unitOfWork.RestaurantCourierCompanyRepository.GetAsync(
                r => r.RestaurantId == restaurantId && r.CourierCompanyId == companyId &&
                     (r.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.PendingApproval ||
                      r.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.Active),
                enableTracking: true);

            if (relation == null)
            {
                result.Fail("Kurye firması ilişkisi bulunamadı.");
                return result;
            }

            relation.StatusId = (short)AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.TerminatedByRestaurant;
            relation.AgreementEndDate = DateTime.UtcNow;
            _unitOfWork.RestaurantCourierCompanyRepository.Update(relation);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Kurye firması anlaşması sonlandırıldı.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    #endregion

    #region Restaurant Invites (Company Side)

    public async Task<ServiceCollectionResult<RestaurantCourierCompanyDto>> GetRestaurantInvitesAsync(Guid ownerUserId)
    {
        var result = new ServiceCollectionResult<RestaurantCourierCompanyDto>();
        try
        {
            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.OwnerUserId == ownerUserId);
            if (company == null)
            {
                result.Fail("Firma bulunamadı.");
                return result;
            }

            var invites = await _unitOfWork.RestaurantCourierCompanyRepository.GetListAsync(
                r => r.CourierCompanyId == company.Id &&
                     r.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.PendingApproval);

            var restaurantIds = invites.Items.Select(i => i.RestaurantId).Distinct().ToList();
            var restaurants = await _context.Set<Domain.Entities.Seller.Restaurant>()
                .Where(r => restaurantIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, r => r.Name);

            var dtos = invites.Items.Select(i => new RestaurantCourierCompanyDto
            {
                Id = i.Id,
                CourierCompanyId = i.CourierCompanyId,
                CompanyName = restaurants.GetValueOrDefault(i.RestaurantId, "Bilinmiyor"),
                StatusId = i.StatusId,
                StatusName = "Onay Bekliyor",
                AgreementStartDate = i.AgreementStartDate
            }).ToList();

            result.SetData(dtos);
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> AcceptRestaurantInviteAsync(Guid ownerUserId, Guid inviteId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.OwnerUserId == ownerUserId);
            if (company == null)
            {
                result.Fail("Firma bulunamadı.");
                return result;
            }

            var invite = await _unitOfWork.RestaurantCourierCompanyRepository.GetAsync(
                r => r.Id == inviteId && r.CourierCompanyId == company.Id &&
                     r.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.PendingApproval,
                enableTracking: true);

            if (invite == null)
            {
                result.Fail("Davet bulunamadı.");
                return result;
            }

            invite.StatusId = (short)AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.Active;
            invite.AgreementStartDate = DateTime.UtcNow;
            _unitOfWork.RestaurantCourierCompanyRepository.Update(invite);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Restoran daveti kabul edildi.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> RejectRestaurantInviteAsync(Guid ownerUserId, Guid inviteId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.OwnerUserId == ownerUserId);
            if (company == null)
            {
                result.Fail("Firma bulunamadı.");
                return result;
            }

            var invite = await _unitOfWork.RestaurantCourierCompanyRepository.GetAsync(
                r => r.Id == inviteId && r.CourierCompanyId == company.Id &&
                     r.StatusId == (short)AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.PendingApproval,
                enableTracking: true);

            if (invite == null)
            {
                result.Fail("Davet bulunamadı.");
                return result;
            }

            invite.StatusId = (short)AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.TerminatedByCompany;
            _unitOfWork.RestaurantCourierCompanyRepository.Update(invite);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Restoran daveti reddedildi.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    #endregion

    #region Pickup Flow

    public async Task<ServiceCollectionResult<PickupOrderDto>> GetPendingPickupOrdersAsync(Guid courierId, Guid restaurantId, int page, int size)
    {
        var result = new ServiceCollectionResult<PickupOrderDto>();
        try
        {
            size = Math.Min(size, 50);

            // Find courier's company
            var membership = await _unitOfWork.CourierCompanyMemberRepository.GetAsync(
                m => m.CourierId == courierId &&
                     m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.Active);

            // Also check if courier is a company owner
            Guid? companyId = membership?.CourierCompanyId;
            if (companyId == null)
            {
                var ownedCompany = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.OwnerUserId == courierId);
                companyId = ownedCompany?.Id;
            }

            if (companyId == null)
            {
                result.Fail("Bir kurye firmasına bağlı değilsiniz.");
                return result;
            }

            var orders = await _unitOfWork.OrderRepository.GetListAsync(
                o => o.CourierCompanyId == companyId &&
                     o.RestaurantId == restaurantId &&
                     o.PickedUpByCourierId == null &&
                     o.StatusId == (short)AuthorizationServiceEnums.OrderStatusEnums.OnTheWay,
                orderBy: q => q.OrderBy(o => o.CreatedDate),
                index: page - 1,
                size: size);

            var restaurant = await _context.Set<Domain.Entities.Seller.Restaurant>()
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            var addressIds = orders.Items.Select(o => o.DeliveryAddressId).Distinct().ToList();
            var addresses = await _context.Set<Domain.Entities.Common.Address>()
                .Where(a => addressIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id);

            var dtos = orders.Items.Select(o =>
            {
                addresses.TryGetValue(o.DeliveryAddressId, out var addr);
                return new PickupOrderDto
                {
                    OrderId = o.Id,
                    RestaurantName = restaurant?.Name ?? "Bilinmiyor",
                    DeliveryAddress = addr?.AddressLine1 ?? "Bilinmiyor",
                    DeliveryDistanceKm = o.DeliveryDistanceKm,
                    TotalPrice = o.TotalPrice,
                    CreatedDate = o.CreatedDate
                };
            }).ToList();

            result.SetData(orders.Count, dtos);
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> ConfirmPickupAsync(Guid courierId, Guid orderId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(o => o.Id == orderId, enableTracking: true);
            if (order == null)
            {
                result.Fail("Sipariş bulunamadı.");
                return result;
            }

            if (order.StatusId != (short)AuthorizationServiceEnums.OrderStatusEnums.OnTheWay)
            {
                result.Fail("Sipariş 'Yolda' durumunda değil.");
                return result;
            }

            if (order.CourierCompanyId == null)
            {
                result.Fail("Bu sipariş bir kurye firmasına atanmamış.");
                return result;
            }

            // Validate courier belongs to the company
            var membership = await _unitOfWork.CourierCompanyMemberRepository.GetAsync(
                m => m.CourierId == courierId && m.CourierCompanyId == order.CourierCompanyId &&
                     m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.Active);

            var isOwner = await _unitOfWork.CourierCompanyRepository.GetAsync(
                c => c.Id == order.CourierCompanyId && c.OwnerUserId == courierId);

            if (membership == null && isOwner == null)
            {
                result.Fail("Bu siparişi almaya yetkiniz yok.");
                return result;
            }

            order.PickedUpByCourierId = courierId;
            order.PickedUpAt = DateTime.UtcNow;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Sipariş alındı.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    #endregion

    #region Admin

    public async Task<ServiceCollectionResult<CourierCompanyDto>> GetAllCompaniesAsync(int page, int size)
    {
        var result = new ServiceCollectionResult<CourierCompanyDto>();
        try
        {
            size = Math.Min(size, 50);
            var companies = await _unitOfWork.CourierCompanyRepository.GetListAsync(
                orderBy: q => q.OrderByDescending(c => c.CreatedDate),
                index: page - 1,
                size: size);

            var companyIds = companies.Items.Select(c => c.Id).ToList();

            // Get member counts
            var allMembers = await _unitOfWork.CourierCompanyMemberRepository.GetListAsync(
                m => companyIds.Contains(m.CourierCompanyId) &&
                     m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.Active,
                size: 10000);

            var memberCounts = allMembers.Items
                .GroupBy(m => m.CourierCompanyId)
                .ToDictionary(g => g.Key, g => g.Count());

            var dtos = companies.Items.Select(c =>
                MapCompanyToDto(c, memberCounts.GetValueOrDefault(c.Id, 0))).ToList();

            result.SetData(companies.Count, dtos);
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> ApproveCompanyAsync(Guid companyId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.Id == companyId, enableTracking: true);
            if (company == null)
            {
                result.Fail("Firma bulunamadı.");
                return result;
            }

            company.StatusId = (short)AuthorizationServiceEnums.CourierCompanyStatusEnums.Active;
            _unitOfWork.CourierCompanyRepository.Update(company);

            // Update owner role to CourierCompanyAdmin
            var owner = await _userRepository.GetAsync(u => u.Id == company.OwnerUserId, enableTracking: true);
            if (owner != null)
            {
                owner.UserRoleId = (short)AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin;
                await _userRepository.UpdateAsync(owner);
            }

            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage("Firma onaylandı.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> RejectCompanyAsync(Guid companyId)
    {
        return await UpdateCompanyStatusAsync(companyId, AuthorizationServiceEnums.CourierCompanyStatusEnums.Banned, "Firma reddedildi.");
    }

    public async Task<ServiceObjectResult<bool>> SuspendCompanyAsync(Guid companyId)
    {
        return await UpdateCompanyStatusAsync(companyId, AuthorizationServiceEnums.CourierCompanyStatusEnums.Suspended, "Firma askıya alındı.");
    }

    public async Task<ServiceObjectResult<bool>> BanCompanyAsync(Guid companyId)
    {
        return await UpdateCompanyStatusAsync(companyId, AuthorizationServiceEnums.CourierCompanyStatusEnums.Banned, "Firma yasaklandı.");
    }

    private async Task<ServiceObjectResult<bool>> UpdateCompanyStatusAsync(Guid companyId, AuthorizationServiceEnums.CourierCompanyStatusEnums status, string message)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var company = await _unitOfWork.CourierCompanyRepository.GetAsync(c => c.Id == companyId, enableTracking: true);
            if (company == null)
            {
                result.Fail("Firma bulunamadı.");
                return result;
            }

            company.StatusId = (short)status;
            _unitOfWork.CourierCompanyRepository.Update(company);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
            result.AddSuccessMessage(message);
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    #endregion

    #region Helpers

    private static readonly Dictionary<short, string> StatusNames = new()
    {
        { (short)AuthorizationServiceEnums.CourierCompanyStatusEnums.PendingApproval, "Onay Bekliyor" },
        { (short)AuthorizationServiceEnums.CourierCompanyStatusEnums.Active, "Aktif" },
        { (short)AuthorizationServiceEnums.CourierCompanyStatusEnums.Suspended, "Askıda" },
        { (short)AuthorizationServiceEnums.CourierCompanyStatusEnums.Banned, "Yasaklı" }
    };

    private static CourierCompanyDto MapCompanyToDto(CourierCompany c, int memberCount) => new()
    {
        Id = c.Id,
        Name = c.Name,
        LegalName = c.LegalName,
        ContactEmail = c.ContactEmail,
        ContactPhone = c.ContactPhone,
        StatusId = c.StatusId,
        StatusName = StatusNames.GetValueOrDefault(c.StatusId, "?"),
        MemberCount = memberCount,
        CreatedDate = c.CreatedDate
    };

    #endregion
}
