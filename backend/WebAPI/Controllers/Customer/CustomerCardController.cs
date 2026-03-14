using Application.Services.Buyer.CardService;
using Base.Enums;
using Domain.Service;
using Infrastructure.Adapters.IyzicoServiceAdapter;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/card")]
[ApiController]
public class CustomerCardController : BaseController
{
    private readonly ICardService _cardService;

    public CustomerCardController(ICardService cardService) => _cardService = cardService;

    [HttpGet("list")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<List<CardDetailDto>>> GetCards()
        => await _cardService.GetCards();

    [HttpPost("create")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<CardStorageResultDto>> CreateCard([FromBody] CreateCardRequestDto requestDto)
        => await _cardService.CreateCard(requestDto);

    [HttpDelete("{cardToken}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> DeleteCard(string cardToken)
        => await _cardService.DeleteCard(cardToken);
}
