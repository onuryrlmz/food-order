using Application.Services.Common.TokenService;
using Base.Enums;
using Domain.Dto.Courier;
using Domain.Entities.Courier;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Courier.CourierEarningService;

public class CourierEarningManager : ICourierEarningService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly BaseDbContext _context;

    public CourierEarningManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor, BaseDbContext context)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _context = context;
    }

    public async Task<ServiceCollectionResult> GetMyEarnings(DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 20)
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

            var query = _context.Set<CourierEarning>()
                .Where(e => e.CourierId == courier.Id && e.DeletedDate == null);

            if (from.HasValue) query = query.Where(e => e.CreatedDate >= from.Value);
            if (to.HasValue) query = query.Where(e => e.CreatedDate <= to.Value);

            var totalCount = await query.CountAsync();
            var earnings = await query
                .OrderByDescending(e => e.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
            result.TotalDataCount = totalCount;
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<EarningSummaryResponseDto>> GetEarningSummary()
    {
        var result = new ServiceObjectResult<EarningSummaryResponseDto>();
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

            var earnings = await _context.Set<CourierEarning>()
                .Where(e => e.CourierId == courier.Id && e.DeletedDate == null)
                .ToListAsync();

            var totalEarnings = earnings.Sum(e => e.TotalEarning);
            var settledAmount = earnings.Where(e => e.IsSettled).Sum(e => e.TotalEarning);

            result.SetData(new EarningSummaryResponseDto
            {
                TotalEarnings = totalEarnings,
                SettledAmount = settledAmount,
                PendingAmount = totalEarnings - settledAmount,
                TotalDeliveries = earnings.Count,
                AverageEarningPerDelivery = earnings.Count > 0 ? totalEarnings / earnings.Count : 0
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> SettleEarnings(List<Guid> earningIds)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var earnings = await _context.Set<CourierEarning>()
                .Where(e => earningIds.Contains(e.Id) && !e.IsSettled && e.DeletedDate == null)
                .ToListAsync();

            foreach (var earning in earnings)
            {
                earning.IsSettled = true;
                earning.SettledAt = DateTime.UtcNow;
            }

            _context.Set<CourierEarning>().UpdateRange(earnings);
            await _context.SaveChangesAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> CreateEarning(Guid deliveryAssignmentId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var assignment = await _unitOfWork.DeliveryAssignmentRepository.GetAsync(x => x.Id == deliveryAssignmentId);
            if (assignment == null)
            {
                result.Fail("Teslimat bulunamadı.");
                return result;
            }

            if (assignment.CourierId == null)
            {
                result.Fail("Kurye atanmamış.");
                return result;
            }

            var existing = await _context.Set<CourierEarning>()
                .AnyAsync(e => e.DeliveryAssignmentId == deliveryAssignmentId && e.DeletedDate == null);
            if (existing)
            {
                result.Fail("Bu teslimat için kazanç zaten oluşturulmuş.");
                return result;
            }

            var earning = new CourierEarning
            {
                Id = Guid.NewGuid(),
                CourierId = assignment.CourierId.Value,
                DeliveryAssignmentId = deliveryAssignmentId,
                OrderId = assignment.OrderId,
                DeliveryFee = assignment.DeliveryFee ?? 0,
                TotalEarning = assignment.DeliveryFee ?? 0,
                IsSettled = false
            };

            await _unitOfWork.CourierEarningRepository.AddAsync(earning);
            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}