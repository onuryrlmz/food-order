using Base.Constant;
using Base.Enums;
using Base.Security;
using Domain.Dto.Common;
using Domain.Dto.Payment;
using Domain.Dto.Seller;
using Domain.Entities.Common;
using Domain.Entities.Seller;
using Domain.Service;
using Infrastructure.Adapters.IyzicoServiceAdapter;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller._1_SellerService;

public class SellerManager : ISellerService
{
    private readonly ISellerRepository _sellerRepository;
    private readonly ISellerDetailRepository _sellerDetailRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IUserRepository _userRepository;
    private readonly IIyzicoServiceAdapter _iyzicoServiceAdapter;

    public SellerManager(ISellerRepository sellerRepository, ISellerDetailRepository sellerDetailRepository, IAddressRepository addressRepository, IUserRepository userRepository, IIyzicoServiceAdapter iyzicoServiceAdapter)
    {
        _sellerRepository = sellerRepository;
        _sellerDetailRepository = sellerDetailRepository;
        _addressRepository = addressRepository;
        _userRepository = userRepository;
        _iyzicoServiceAdapter = iyzicoServiceAdapter;
    }

    public async Task<ServiceObjectResult<bool>> AddSeller(AddSellerDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var seller = await _sellerRepository.GetAsync(x => x.TaxCode == requestDto.TaxCode);
            if (seller != null)
            {
                result.AddErrorMessage("Bu vergi numarası ile kayıtlı bir satıcı bulunmaktadır.");
            }
            else
            {
                seller = await _sellerRepository.AddAsync(new Domain.Entities.Seller.Seller
                {
                    Id = Guid.NewGuid(),
                    CompanyType = requestDto.CompanyType,
                    CompanyStatus = (short)AuthorizationServiceEnums.CompanyStatusEnums.Pending,
                    Name = requestDto.Name,
                    LegalName = requestDto.LegalName,
                    TaxCode = requestDto.TaxCode,
                    TaxArea = requestDto.TaxArea,
                    IBAN = requestDto.IBAN,
                    IsEInvoiceAvaible = requestDto.IsEInvoiceAvaible
                });

                var address = new Address
                {
                    Id = Guid.NewGuid(),
                    SellerId = seller.Id,
                    AddressType = (short)AuthorizationServiceEnums.AddressTypeEnums.Invoice,
                    AddressName = "Fatura Adresi",
                    FirstName = requestDto.OwnerFirstName,
                    LastName = requestDto.OwnerLastName,
                    Phone = requestDto.OwnerPhone,
                    CityId = requestDto.CityId,
                    TownId = requestDto.TownId,
                    NeighbourhoodId = requestDto.NeighbourhoodId,
                    AddressLine1 = requestDto.AddressLine1,
                    AddressLine2 = requestDto.AddressLine2,
                    IsDefault = true
                };
                await _addressRepository.AddAsync(address);

                address.Id = Guid.NewGuid();
                address.AddressName = "Kargo Adresi";
                address.AddressType = (short)AuthorizationServiceEnums.AddressTypeEnums.Shipping;
                await _addressRepository.AddAsync(address);

                address.Id = Guid.NewGuid();
                address.AddressName = "İade Adresi";
                address.AddressType = (short)AuthorizationServiceEnums.AddressTypeEnums.Return;
                await _addressRepository.AddAsync(address);

                await _userRepository.AddAsync(new User
                {
                    Id = Guid.NewGuid(),
                    UserRoleId = (short)AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
                    UserStatusId = (short)AuthorizationServiceEnums.UserStatusEnums.WaitingForActivation,
                    Email = requestDto.OwnerEmail,
                    Password = requestDto.Password.Encrypt(Global.EncryptionKey),
                    FirstName = requestDto.OwnerFirstName,
                    LastName = requestDto.OwnerLastName,
                    PhoneNumber = requestDto.OwnerPhone,
                    SellerId = seller.Id,
                    ActivationKey = Guid.NewGuid()
                });

                result.SetData(true);

                //TODO: Mail gönderimi yapılacak
            }
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> ConfirmSeller(ConfirmSellerDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var seller = await _sellerRepository.GetAsync(x => x.Id == requestDto.Id);
            if (seller == null)
            {
                result.AddErrorMessage("Satıcı bulunamadı");
            }
            else
            {
                if (seller.CompanyStatus == (short)AuthorizationServiceEnums.CompanyStatusEnums.Approved)
                {
                    result.AddErrorMessage("Satıcı zaten onaylanmış");
                }
                else
                {
                    var sellerUser = await _userRepository.GetAsync(x => x.SellerId == seller.Id && x.UserRoleId == (short)AuthorizationServiceEnums.UserRoleEnums.SellerAdmin && x.UserStatusId == (short)AuthorizationServiceEnums.UserStatusEnums.WaitingForActivation);
                    if (sellerUser != null)
                    {
                        var paymentSellerResponse = _iyzicoServiceAdapter.CreateSeller(new CreateSubMerchantDto
                        {
                            Id = seller.Id,
                            CompanyType = seller.CompanyType,
                            CompanyName = seller.LegalName,
                            TaxCode = seller.TaxCode,
                            TaxArea = seller.TaxArea,
                            IBAN = seller.IBAN,
                            Address = @"Test Adres",
                            Email = sellerUser.Email
                        });

                        if (string.IsNullOrWhiteSpace(paymentSellerResponse.Data))
                        {
                            result.AddErrorMessage("Ödeme servisinde satıcı oluşturulamadı");
                        }
                        else
                        {
                            seller.CompanyStatus = (short)AuthorizationServiceEnums.CompanyStatusEnums.Approved;
                            await _sellerRepository.UpdateAsync(seller);

                            sellerUser.UserStatusId = (short)AuthorizationServiceEnums.UserStatusEnums.Active;
                            await _userRepository.UpdateAsync(sellerUser);

                            var sellerDetail = new SellerDetail
                            {
                                SellerId = seller.Id,
                                Key1 = (int)AuthorizationServiceEnums.SellerDetailKey1.PaymentSubMerchantKey,
                                Key2 = (int)AuthorizationServiceEnums.SellerDetailKey2.Iyzico,
                                ValueStr = paymentSellerResponse.Data
                            };
                            await _sellerDetailRepository.AddAsync(sellerDetail);

                            result.SetData(true);

                            //TODO: Satıcı onaylandı maili gönderilecek
                        }
                    }
                    else
                    {
                        result.AddErrorMessage("Satıcı yöneticisi bulunamadı");
                    }
                }
            }
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}