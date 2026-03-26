using Application.Services.Buyer.SearchHistoryService;
using Base.Enums;
using Domain.Dto.Buyer.Search;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/search")]
[ApiController]
public class CustomerSearchController : BaseController
{
    private readonly ISearchHistoryService _searchHistoryService;

    public CustomerSearchController(ISearchHistoryService searchHistoryService)
    {
        _searchHistoryService = searchHistoryService;
    }

    [HttpPost("record")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> RecordSearch([FromBody] RecordSearchRequestDto requestDto)
    {
        return await _searchHistoryService.RecordSearch(requestDto);
    }

    [HttpGet("recent")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceCollectionResult<SearchHistoryDto>> GetRecentSearches([FromQuery] int limit = 20)
    {
        return await _searchHistoryService.GetRecentSearches(limit);
    }

    [HttpDelete("history")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> ClearHistory()
    {
        return await _searchHistoryService.ClearSearchHistory();
    }

    [HttpGet("popular")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceCollectionResult<PopularSearchDto>> GetPopularSearches([FromQuery] int limit = 10)
    {
        return await _searchHistoryService.GetPopularSearches(limit);
    }

    [HttpGet("suggestions")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.User)]
    public async Task<ServiceCollectionResult<SearchSuggestionDto>> GetSuggestions(
        [FromQuery] string q,
        [FromQuery] int limit = 10)
    {
        return await _searchHistoryService.GetSearchSuggestions(q, limit);
    }
}
