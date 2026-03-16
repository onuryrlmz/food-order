using Application.Services.Buyer.ReviewService;
using Base.Enums;
using Domain.Dto.Buyer.Review;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/review")]
[ApiController]
public class CustomerReviewController : BaseController
{
    private readonly IReviewService _reviewService;

    public CustomerReviewController(IReviewService reviewService) => _reviewService = reviewService;

    [HttpPost]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<ReviewDto>> Create([FromBody] CreateReviewRequestDto requestDto)
        => await _reviewService.CreateReview(requestDto);

    [HttpGet("restaurant/{restaurantId}")]
    [AuthorizeAPIRequest(false, false)]
    public async Task<ServiceCollectionResult<ReviewDto>> GetByRestaurant(
        Guid restaurantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => await _reviewService.GetRestaurantReviews(restaurantId, page, pageSize);

    [HttpDelete("{reviewId}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Delete(Guid reviewId)
        => await _reviewService.DeleteReview(reviewId);
}
