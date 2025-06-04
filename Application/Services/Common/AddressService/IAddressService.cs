using Domain.Dto.Common;
using Domain.Service;

namespace Application.Services.Common.AddressService;

public interface IAddressService
{
    Task<ServiceObjectResult<bool>> Add(AddAddressDto requestDto);
}