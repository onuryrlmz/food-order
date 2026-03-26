using System.Data;
using Application.Services.Common.TokenService;
using Dapper;
using Domain.Dto.Buyer.Search;
using Domain.Entities.Buyer;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories;

namespace Application.Services.Buyer.SearchHistoryService;

public class SearchHistoryManager : ISearchHistoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly BaseDbContext _context;

    public SearchHistoryManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor, BaseDbContext context)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _context = context;
    }

    public async Task<ServiceObjectResult<bool>> RecordSearch(RecordSearchRequestDto requestDto)
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

            if (string.IsNullOrWhiteSpace(requestDto.Query))
            {
                result.Fail("Search query cannot be empty");
                return result;
            }

            var trimmedQuery = requestDto.Query.Trim();

            // Check if same query was the most recent search (avoid duplicates)
            var lastSearch = await _unitOfWork.SearchHistoryRepository.GetAsync(
                x => x.UserId == token.UserId && x.Query == trimmedQuery && x.DeletedDate == null);

            if (lastSearch != null && (DateTime.UtcNow - lastSearch.CreatedDate).TotalMinutes < 5)
            {
                result.SetData(true);
                return result;
            }

            var searchHistory = new SearchHistory
            {
                Id = Guid.NewGuid(),
                UserId = token.UserId,
                Query = trimmedQuery,
                SearchType = requestDto.SearchType,
                ResultCount = requestDto.ResultCount
            };

            await _unitOfWork.SearchHistoryRepository.AddAsync(searchHistory);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<SearchHistoryDto>> GetRecentSearches(int limit = 20)
    {
        var result = new ServiceCollectionResult<SearchHistoryDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            limit = Math.Min(limit, 50);

            const string query = @"
                SELECT `Query`, MAX(`Id`) AS Id, MAX(`SearchType`) AS SearchType,
                       MAX(`ResultCount`) AS ResultCount, MAX(`CreatedDate`) AS CreatedDate
                FROM `SearchHistory`
                WHERE `UserId` = @userId AND `DeletedDate` IS NULL
                GROUP BY `Query`
                ORDER BY MAX(`CreatedDate`) DESC
                LIMIT @limit";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var searches = (await conn.QueryAsync<SearchHistoryDto>(query, new { userId = token.UserId, limit })).ToList();

            result.SetData(searches);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> ClearSearchHistory()
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

            const string query = @"
                UPDATE `SearchHistory`
                SET `DeletedDate` = @now
                WHERE `UserId` = @userId AND `DeletedDate` IS NULL";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await conn.ExecuteAsync(query, new { userId = token.UserId, now = DateTime.UtcNow });

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<PopularSearchDto>> GetPopularSearches(int limit = 10)
    {
        var result = new ServiceCollectionResult<PopularSearchDto>();
        try
        {
            limit = Math.Min(limit, 20);

            const string query = @"
                SELECT `Query`, COUNT(*) AS SearchCount
                FROM `SearchHistory`
                WHERE `DeletedDate` IS NULL
                  AND `CreatedDate` >= @since
                GROUP BY `Query`
                ORDER BY SearchCount DESC
                LIMIT @limit";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var since = DateTime.UtcNow.AddDays(-7);
            var searches = (await conn.QueryAsync<PopularSearchDto>(query, new { since, limit })).ToList();

            result.SetData(searches);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<SearchSuggestionDto>> GetSearchSuggestions(string partialQuery, int limit = 10)
    {
        var result = new ServiceCollectionResult<SearchSuggestionDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            if (string.IsNullOrWhiteSpace(partialQuery))
            {
                result.SetData(new List<SearchSuggestionDto>());
                return result;
            }

            limit = Math.Min(limit, 20);
            var searchPattern = $"{partialQuery.Trim()}%";

            // User's own recent searches matching prefix
            const string userQuery = @"
                SELECT DISTINCT `Query`, 'history' AS Source
                FROM `SearchHistory`
                WHERE `UserId` = @userId AND `DeletedDate` IS NULL
                  AND `Query` LIKE @searchPattern
                ORDER BY MAX(`CreatedDate`) DESC
                LIMIT @halfLimit";

            // Popular searches matching prefix
            const string popularQuery = @"
                SELECT `Query`, 'popular' AS Source
                FROM `SearchHistory`
                WHERE `DeletedDate` IS NULL
                  AND `Query` LIKE @searchPattern
                  AND `CreatedDate` >= @since
                GROUP BY `Query`
                ORDER BY COUNT(*) DESC
                LIMIT @halfLimit";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var halfLimit = Math.Max(limit / 2, 5);
            var since = DateTime.UtcNow.AddDays(-7);

            var userSuggestions = (await conn.QueryAsync<SearchSuggestionDto>(userQuery,
                new { userId = token.UserId, searchPattern, halfLimit })).ToList();

            var popularSuggestions = (await conn.QueryAsync<SearchSuggestionDto>(popularQuery,
                new { searchPattern, since, halfLimit })).ToList();

            // Combine: user history first, then popular (deduplicated)
            var existingQueries = new HashSet<string>(userSuggestions.Select(s => s.Query), StringComparer.OrdinalIgnoreCase);
            var combined = userSuggestions
                .Concat(popularSuggestions.Where(s => !existingQueries.Contains(s.Query)))
                .Take(limit)
                .ToList();

            result.SetData(combined);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}
