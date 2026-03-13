using Application.Services.Common.TokenService;
using AutoMapper;
using Domain.Dto.Common;
using Domain.Entities.Common;
using Domain.Service;
using Persistence.IRepositories.Common;

namespace Application.Services.Common.AddressService;

public class AddressManager : IAddressService
{
    private readonly IMapper _mapper;
    private readonly IAddressRepository _addressRepository;
    private readonly ITokenAccessor _tokenAccessor;

    public AddressManager(IAddressRepository addressRepository, IMapper mapper, ITokenAccessor tokenAccessor)
    {
        _addressRepository = addressRepository;
        _mapper = mapper;
        _tokenAccessor = tokenAccessor;
    }

    public async Task<ServiceObjectResult<bool>> Add(AddAddressDto requestDto)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var address = _mapper.Map<Address>(requestDto);
            address.UserId = _tokenAccessor.GetToken()?.UserId;

            if (requestDto.IsDefault)
            {
                var existing = await _addressRepository.GetListAsync(x => x.UserId == address.UserId && x.IsDefault, enableTracking: true, size: 999);
                foreach (var a in existing.Items)
                {
                    a.IsDefault = false;
                    _addressRepository.Update(a);
                }
            }

            await _addressRepository.AddAsync(address);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceCollectionResult<GetAddressDto>> GetList()
    {
        var response = new ServiceCollectionResult<GetAddressDto>();
        try
        {
            var userId = _tokenAccessor.GetToken()?.UserId;
            var list = await _addressRepository.GetListAsync(x => x.UserId == userId && x.DeletedDate == null, enableTracking: false, withDeleted: false, size: 999);
            response.SetData(_mapper.Map<List<GetAddressDto>>(list.Items));
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Update(UpdateAddressDto requestDto)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var userId = _tokenAccessor.GetToken()?.UserId;
            var address = await _addressRepository.GetAsync(x => x.Id == requestDto.Id && x.UserId == userId, enableTracking: true);
            if (address == null)
            {
                response.Fail("Adres bulunamadı.");
                return response;
            }

            if (requestDto.IsDefault && !address.IsDefault)
            {
                var existing = await _addressRepository.GetListAsync(x => x.UserId == userId && x.IsDefault && x.Id != requestDto.Id, enableTracking: true, size: 999);
                foreach (var a in existing.Items)
                {
                    a.IsDefault = false;
                    _addressRepository.Update(a);
                }
            }

            _mapper.Map(requestDto, address);
            _addressRepository.Update(address);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> Delete(Guid id)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var userId = _tokenAccessor.GetToken()?.UserId;
            var address = await _addressRepository.GetAsync(x => x.Id == id && x.UserId == userId, enableTracking: true);
            if (address == null)
            {
                response.Fail("Adres bulunamadı.");
                return response;
            }

            await _addressRepository.DeleteAsync(address);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> SetDefault(Guid id)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var userId = _tokenAccessor.GetToken()?.UserId;
            var existing = await _addressRepository.GetListAsync(x => x.UserId == userId && x.IsDefault, enableTracking: true, size: 999);
            foreach (var a in existing.Items)
            {
                a.IsDefault = false;
                _addressRepository.Update(a);
            }

            var address = await _addressRepository.GetAsync(x => x.Id == id && x.UserId == userId, enableTracking: true);
            if (address == null)
            {
                response.Fail("Adres bulunamadı.");
                return response;
            }

            address.IsDefault = true;
            _addressRepository.Update(address);
            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}