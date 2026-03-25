using Domain.Dto.Common;
using Domain.Service;

namespace Application.Services.Common.AddressService;

public interface IAddressService
{
    Task<ServiceObjectResult<bool>> Add(AddAddressDto requestDto);
    Task<ServiceCollectionResult<GetAddressDto>> GetList();
    Task<ServiceObjectResult<bool>> Update(UpdateAddressDto requestDto);
    Task<ServiceObjectResult<bool>> Delete(Guid id);
    Task<ServiceObjectResult<bool>> SetDefault(Guid id);
}