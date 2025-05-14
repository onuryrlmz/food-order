using Application.Services.Common;
using Base.Enums;
using Domain.Dto.Common;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers;

[Route("v1/address")]
[ApiController]
public class AddressController : BaseController
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpPost("add")]
    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.User })]
    public async Task<ServiceObjectResult<bool>> Add([FromBody] AddAddressDto requestDto)
    {
        return await _addressService.Add(requestDto);
    }
}