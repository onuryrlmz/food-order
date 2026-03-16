using System.Data;
using Application.Services.Common.TokenService;
using Dapper;
using Domain.Dto.Buyer.Favorite;
using Domain.Entities.Buyer;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Buyer.FavoriteService;

public class FavoriteManager : IFavoriteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly BaseDbContext _context;

    public FavoriteManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor, BaseDbContext context)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _context = context;
    }

    public async Task<ServiceObjectResult<bool>> AddFavorite(Guid restaurantId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            // Check restaurant exists
            var restaurant = await _unitOfWork.RestaurantRepository.GetAsync(x => x.Id == restaurantId);
            if (restaurant == null)
            {
                result.Fail("Restaurant not found");
                return result;
            }

            // Check not already favorited
            var existing = await _unitOfWork.FavoriteRestaurantRepository.GetAsync(
                x => x.UserId == token.UserId && x.RestaurantId == restaurantId);
            if (existing != null)
            {
                result.SetData(true); // Already favorited, treat as success
                return result;
            }

            await _unitOfWork.FavoriteRestaurantRepository.AddAsync(new FavoriteRestaurant
            {
                Id = Guid.NewGuid(),
                UserId = token.UserId,
                RestaurantId = restaurantId
            });

            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> RemoveFavorite(Guid restaurantId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            var favorite = await _unitOfWork.FavoriteRestaurantRepository.GetAsync(
                x => x.UserId == token.UserId && x.RestaurantId == restaurantId, enableTracking: true);
            if (favorite == null)
            {
                result.SetData(true); // Not favorited, treat as success
                return result;
            }

            await _unitOfWork.FavoriteRestaurantRepository.DeleteAsync(favorite);
            await _unitOfWork.CompleteAsync();
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<FavoriteRestaurantDto>> GetFavorites(int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<FavoriteRestaurantDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            pageSize = Math.Min(pageSize, 50);
            var offset = (page - 1) * pageSize;

            const string countQuery = @"
                SELECT COUNT(*) FROM `FavoriteRestaurant`
                WHERE `UserId` = @userId AND `DeletedDate` IS NULL";

            const string query = @"
                SELECT f.`Id`, f.`RestaurantId`, r.`Name` AS RestaurantName,
                       r.`CoverImage`, r.`Rating`, r.`RatingCount`,
                       r.`MinimumOrderPrice`, r.`MinDeliveryTime`, r.`MaxDeliveryTime`,
                       r.`IsOpen`, f.`CreatedDate`
                FROM `FavoriteRestaurant` f
                INNER JOIN `Restaurant` r ON r.`Id` = f.`RestaurantId`
                WHERE f.`UserId` = @userId AND f.`DeletedDate` IS NULL AND r.`DeletedDate` IS NULL
                ORDER BY f.`CreatedDate` DESC
                LIMIT @pageSize OFFSET @offset";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var totalCount = await conn.ExecuteScalarAsync<int>(countQuery, new { userId = token.UserId });
            var favorites = (await conn.QueryAsync<FavoriteRestaurantDto>(
                query, new { userId = token.UserId, pageSize, offset })).ToList();

            result.SetData(totalCount, favorites);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<HashSet<Guid>>> GetFavoriteRestaurantIds()
    {
        var result = new ServiceObjectResult<HashSet<Guid>>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.SetData(new HashSet<Guid>());
                return result;
            }

            const string query = @"
                SELECT `RestaurantId` FROM `FavoriteRestaurant`
                WHERE `UserId` = @userId AND `DeletedDate` IS NULL";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var ids = (await conn.QueryAsync<Guid>(query, new { userId = token.UserId })).ToHashSet();
            result.SetData(ids);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}
