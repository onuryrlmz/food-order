using Application.Services.Common.TokenService;
using Base.Enums;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/finance")]
[ApiController]
public class SellerFinanceController : BaseController
{
    private readonly BaseDbContext _context;
    private readonly ITokenAccessor _tokenAccessor;

    public SellerFinanceController(BaseDbContext context, ITokenAccessor tokenAccessor)
    {
        _context = context;
        _tokenAccessor = tokenAccessor;
    }

    [HttpGet("summary")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.SellerAdmin,
        UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<object>> GetSummary()
    {
        var result = new ServiceObjectResult<object>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token?.SellerId == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var payments = await _context.Set<Domain.Entities.Buyer.Payment>()
                .Where(p => p.SellerId == token.SellerId.Value && p.StatusId == (short)PaymentStatusEnums.Completed)
                .ToListAsync();

            var summary = new
            {
                TotalRevenue = payments.Sum(p => p.Amount),
                TotalCommission = payments.Sum(p => p.CommissionAmount),
                TotalPayout = payments.Sum(p => p.SellerPayoutAmount),
                OrderCount = payments.Count
            };

            result.SetData(summary);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    [HttpGet("payments")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.SellerAdmin,
        UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<object>> GetPayments([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = new ServiceCollectionResult<object>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token?.SellerId == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            pageSize = Math.Min(pageSize, 50);

            var payments = await _context.Set<Domain.Entities.Buyer.Payment>()
                .Where(p => p.SellerId == token.SellerId.Value && p.StatusId == (short)PaymentStatusEnums.Completed)
                .OrderByDescending(p => p.CompletedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => (object)new
                {
                    p.Id,
                    p.OrderId,
                    p.Amount,
                    p.CommissionAmount,
                    p.SellerPayoutAmount,
                    p.CompletedAt,
                    p.RefundedAt,
                    p.RefundReason
                })
                .ToListAsync();

            result.SetData(payments);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}