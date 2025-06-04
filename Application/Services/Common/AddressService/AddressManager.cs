using AutoMapper;
using Base.Entities;
using Domain.Dto.Common;
using Domain.Entities.Common;
using Domain.Service;
using Persistence.IRepositories.Common;

namespace Application.Services.Common.AddressService;

public class AddressManager : IAddressService
{
    public TokenDto _tokenDto { get; set; }
    private readonly IMapper _mapper;
    private readonly IAddressRepository _addressRepository;

    public AddressManager(IAddressRepository addressRepository, IMapper mapper)
    {
        _addressRepository = addressRepository;
        _mapper = mapper;
    }

    public async Task<ServiceObjectResult<bool>> Add(AddAddressDto requestDto)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var mappedSellerAddress = _mapper.Map<Address>(requestDto);
            await _addressRepository.AddAsync(mappedSellerAddress);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}