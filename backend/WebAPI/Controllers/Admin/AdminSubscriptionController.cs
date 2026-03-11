using Application.Services.Seller.SubscriptionService;
using Base.Enums;
using Domain.Dto.Seller.Subscription;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/subscription")]
[ApiController]
public class AdminSubscriptionController : BaseController
{
    private readonly ISubscriptionService _subscriptionService;

    public AdminSubscriptionController(ISubscriptionService subscriptionService) => _subscriptionService = subscriptionService;

    [HttpGet("plans")]
    public async Task<ServiceCollectionResult<GetSubscriptionPlanResponseDto>> GetPlans()
        => await _subscriptionService.GetPlans();

    [HttpPost("plans")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<Guid>> CreatePlan([FromBody] CreateSubscriptionPlanDto requestDto)
        => await _subscriptionService.CreatePlan(requestDto);

    [HttpPut("plans/{planId}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> UpdatePlan(Guid planId, [FromBody] CreateSubscriptionPlanDto requestDto)
        => await _subscriptionService.UpdatePlan(planId, requestDto);

    [HttpGet("all")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<GetSubscriptionResponseDto>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => await _subscriptionService.GetAllSubscriptions(page, pageSize);

    [HttpPost("expire-check")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> ExpireCheck()
        => await _subscriptionService.CheckAndExpireSubscriptions();
}
