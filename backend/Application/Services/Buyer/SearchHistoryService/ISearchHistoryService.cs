using Domain.Dto.Buyer.Search;
using Domain.Service;

namespace Application.Services.Buyer.SearchHistoryService;

public interface ISearchHistoryService
{
    Task<ServiceObjectResult<bool>> RecordSearch(RecordSearchRequestDto requestDto);
    Task<ServiceCollectionResult<SearchHistoryDto>> GetRecentSearches(int limit = 20);
    Task<ServiceObjectResult<bool>> ClearSearchHistory();
    Task<ServiceCollectionResult<PopularSearchDto>> GetPopularSearches(int limit = 10);
    Task<ServiceCollectionResult<SearchSuggestionDto>> GetSearchSuggestions(string partialQuery, int limit = 10);
}
