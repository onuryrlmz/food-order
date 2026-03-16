using Domain.Dto.Buyer.Review;
using Domain.Service;

namespace Application.Services.Buyer.ReviewService;

public interface IReviewService
{
    Task<ServiceObjectResult<ReviewDto>> CreateReview(CreateReviewRequestDto requestDto);
    Task<ServiceCollectionResult<ReviewDto>> GetRestaurantReviews(Guid restaurantId, int page = 1, int pageSize = 20);
    Task<ServiceObjectResult<bool>> DeleteReview(Guid reviewId);
}
