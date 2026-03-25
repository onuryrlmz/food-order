using Application.Services.Buyer.PaymentService;
using Base.Enums;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/finance")]
[ApiController]
public class AdminFinanceController : BaseController
{
    private readonly BaseDbContext _context;

    public AdminFinanceController(BaseDbContext context)
    {
        _context = context;
    }

    [HttpGet("summary")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<object>> GetSummary()
    {
        var result = new ServiceObjectResult<object>();
        try
        {
            var completedPayments = await _context.Set<Domain.Entities.Buyer.Payment>()
                .Where(p => p.StatusId == (short)AuthorizationServiceEnums.PaymentStatusEnums.Completed)
                .ToListAsync();

            var summary = new
            {
                TotalRevenue = completedPayments.Sum(p => p.Amount),
                TotalCommission = completedPayments.Sum(p => p.CommissionAmount),
                TotalPayout = completedPayments.Sum(p => p.SellerPayoutAmount),
                OrderCount = completedPayments.Count
            };

            result.SetData(summary);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    [HttpGet("sellers/{sellerId}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<object>> GetSellerFinance(Guid sellerId)
    {
        var result = new ServiceObjectResult<object>();
        try
        {
            var payments = await _context.Set<Domain.Entities.Buyer.Payment>()
                .Where(p => p.SellerId == sellerId && p.StatusId == (short)AuthorizationServiceEnums.PaymentStatusEnums.Completed)
                .OrderByDescending(p => p.CompletedAt)
                .Select(p => new
                {
                    p.Id,
                    p.OrderId,
                    p.Amount,
                    p.CommissionAmount,
                    p.SellerPayoutAmount,
                    p.CompletedAt,
                    p.RefundedAt
                })
                .ToListAsync();

            var summary = new
            {
                SellerId = sellerId,
                TotalRevenue = payments.Sum(p => p.Amount),
                TotalCommission = payments.Sum(p => p.CommissionAmount),
                TotalPayout = payments.Sum(p => p.SellerPayoutAmount),
                Payments = payments
            };

            result.SetData(summary);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    [HttpPut("settings/commission-rate")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> UpdateCommissionRate([FromBody] UpdateCommissionRateRequest request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            if (request.Rate < 0 || request.Rate > 1)
            {
                result.Fail("Komisyon oranı 0 ile 1 arasında olmalıdır.");
                return result;
            }

            var plans = await _context.Set<Domain.Entities.Seller.SubscriptionPlan>()
                .Where(p => p.IsActive)
                .ToListAsync();

            foreach (var plan in plans) plan.CommissionRate = request.Rate;

            await _context.SaveChangesAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}

public class UpdateCommissionRateRequest
{
    public decimal Rate { get; set; }
}