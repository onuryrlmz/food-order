using Application.Services.Common.AddressService;
using Base.Enums;
using Domain.Dto.Common;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/address")]
[ApiController]
public class CustomerAddressController : BaseController
{
    private readonly IAddressService _addressService;

    public CustomerAddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.User,
        UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<GetAddressDto>> GetList()
    {
        return await _addressService.GetList();
    }

    [HttpPost]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.User,
        UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Add([FromBody] AddAddressDto requestDto)
    {
        return await _addressService.Add(requestDto);
    }

    [HttpPut]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.User,
        UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateAddressDto requestDto)
    {
        return await _addressService.Update(requestDto);
    }

    [HttpDelete("{id}")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.User,
        UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Delete(Guid id)
    {
        return await _addressService.Delete(id);
    }

    [HttpPatch("{id}/set-default")]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.User,
        UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> SetDefault(Guid id)
    {
        return await _addressService.SetDefault(id);
    }
}