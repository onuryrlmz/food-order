using System.Data;
using Application.Services.Common.TokenService;
using Base.Enums;
using Dapper;
using Domain.Dto.Buyer.Review;
using Domain.Entities.Buyer;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Buyer.ReviewService;

public class ReviewManager : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly BaseDbContext _context;

    public ReviewManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor, BaseDbContext context)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _context = context;
    }

    public async Task<ServiceObjectResult<ReviewDto>> CreateReview(CreateReviewRequestDto requestDto)
    {
        var result = new ServiceObjectResult<ReviewDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            // Validate rating range
            if (requestDto.Rating < 1 || requestDto.Rating > 5)
            {
                result.Fail("Rating must be between 1 and 5");
                return result;
            }

            // Validate order exists, belongs to user, and is delivered
            var order = await _unitOfWork.OrderRepository.GetAsync(x => x.Id == requestDto.OrderId && x.UserId == token.UserId);
            if (order == null)
            {
                result.Fail("Order not found");
                return result;
            }

            if (order.StatusId != (short)OrderStatusEnums.Delivered)
            {
                result.Fail("Only delivered orders can be reviewed");
                return result;
            }

            // Check no existing review for this order
            var existingReview = await _unitOfWork.ReviewRepository.GetAsync(x => x.OrderId == requestDto.OrderId);
            if (existingReview != null)
            {
                result.Fail("This order has already been reviewed");
                return result;
            }

            // Create review
            var review = new Review
            {
                Id = Guid.NewGuid(),
                OrderId = requestDto.OrderId,
                UserId = token.UserId,
                RestaurantId = order.RestaurantId,
                Rating = requestDto.Rating,
                Comment = requestDto.Comment
            };

            await _unitOfWork.ReviewRepository.AddAsync(review);

            // Update restaurant rating (running average)
            var restaurant = await _unitOfWork.RestaurantRepository.GetAsync(
                x => x.Id == order.RestaurantId, enableTracking: true);
            if (restaurant != null)
            {
                var oldRating = restaurant.Rating;
                var count = restaurant.RatingCount;
                restaurant.Rating = (oldRating * count + requestDto.Rating) / (count + 1);
                restaurant.RatingCount = count + 1;
                await _unitOfWork.RestaurantRepository.UpdateAsync(restaurant);
            }

            // Kurye rating güncelle
            if (order.CourierId.HasValue)
            {
                var courier = await _unitOfWork.CourierRepository.GetAsync(
                    x => x.Id == order.CourierId.Value, enableTracking: true);
                if (courier != null)
                {
                    var newCount = courier.RatingCount + 1;
                    courier.Rating = ((courier.Rating * courier.RatingCount) + requestDto.Rating) / newCount;
                    courier.RatingCount = newCount;
                    _unitOfWork.CourierRepository.Update(courier);
                }
            }
            else
            {
                // CourierId Order üzerinde yoksa DeliveryAssignment üzerinden bul
                var assignment = await _unitOfWork.DeliveryAssignmentRepository.GetAsync(
                    x => x.OrderId == order.Id && x.StatusId == (short)Base.Enums.DeliveryAssignmentStatusEnums.Delivered);
                if (assignment?.CourierId != null)
                {
                    var courier = await _unitOfWork.CourierRepository.GetAsync(
                        x => x.Id == assignment.CourierId.Value, enableTracking: true);
                    if (courier != null)
                    {
                        var newCount = courier.RatingCount + 1;
                        courier.Rating = ((courier.Rating * courier.RatingCount) + requestDto.Rating) / newCount;
                        courier.RatingCount = newCount;
                        _unitOfWork.CourierRepository.Update(courier);
                    }
                }
            }

            await _unitOfWork.CompleteAsync();

            result.SetData(new ReviewDto
            {
                Id = review.Id,
                OrderId = review.OrderId,
                UserId = review.UserId,
                RestaurantId = review.RestaurantId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedDate = review.CreatedDate
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<ReviewDto>> GetRestaurantReviews(Guid restaurantId, int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<ReviewDto>();
        try
        {
            pageSize = Math.Min(pageSize, 50);
            var offset = (page - 1) * pageSize;

            const string countQuery = @"
                SELECT COUNT(*) FROM `Review`
                WHERE `RestaurantId` = @restaurantId AND `DeletedDate` IS NULL";

            const string query = @"
                SELECT r.`Id`, r.`OrderId`, r.`UserId`, r.`RestaurantId`,
                       r.`Rating`, r.`Comment`, r.`CreatedDate`,
                       u.`FirstName` AS UserFirstName, u.`LastName` AS UserLastName
                FROM `Review` r
                INNER JOIN `User` u ON u.`Id` = r.`UserId`
                WHERE r.`RestaurantId` = @restaurantId AND r.`DeletedDate` IS NULL
                ORDER BY r.`CreatedDate` DESC
                LIMIT @pageSize OFFSET @offset";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var totalCount = await conn.ExecuteScalarAsync<int>(countQuery, new { restaurantId });
            var reviews = (await conn.QueryAsync<ReviewDto>(query, new { restaurantId, pageSize, offset })).ToList();

            result.SetData(totalCount, reviews);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> DeleteReview(Guid reviewId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var review = await _unitOfWork.ReviewRepository.GetAsync(
                x => x.Id == reviewId, enableTracking: true);
            if (review == null)
            {
                result.Fail("Review not found");
                return result;
            }

            // Recalculate restaurant rating
            var restaurant = await _unitOfWork.RestaurantRepository.GetAsync(
                x => x.Id == review.RestaurantId, enableTracking: true);
            if (restaurant != null && restaurant.RatingCount > 1)
            {
                restaurant.Rating = (restaurant.Rating * restaurant.RatingCount - review.Rating) / (restaurant.RatingCount - 1);
                restaurant.RatingCount -= 1;
                await _unitOfWork.RestaurantRepository.UpdateAsync(restaurant);
            }
            else if (restaurant != null)
            {
                restaurant.Rating = 0;
                restaurant.RatingCount = 0;
                await _unitOfWork.RestaurantRepository.UpdateAsync(restaurant);
            }

            await _unitOfWork.ReviewRepository.DeleteAsync(review);
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