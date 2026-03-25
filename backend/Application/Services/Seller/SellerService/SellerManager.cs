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

namespace Application.Services.Seller.SellerService;

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
                var companyType = requestDto.CompanyType == (short)AuthorizationServiceEnums.CompanyTypeEnums.Company
                    ? AuthorizationServiceEnums.CompanyTypeEnums.Company
                    : AuthorizationServiceEnums.CompanyTypeEnums.Individual;

                seller = await _sellerRepository.AddAsync(new Domain.Entities.Seller.Seller
                {
                    Id = Guid.NewGuid(),
                    CompanyType = (short)companyType,
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
                    Password = BCrypt.Net.BCrypt.HashPassword(requestDto.Password, 12),
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
                    var sellerUser = await _userRepository.GetAsync(x => x.SellerId == seller.Id && x.UserRoleId == (short)AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, withDeleted: false);
                    if (sellerUser != null)
                    {
                        // Get seller address for iyzico
                        var sellerAddress = await _addressRepository.GetAsync(a => a.SellerId == seller.Id &&
                                                                                   a.AddressType == (short)AuthorizationServiceEnums.AddressTypeEnums.Invoice);
                        var addressStr = sellerAddress?.AddressLine1 ?? "Adres bilgisi yok";

                        // Validate TC kimlik for Individual sellers
                        if (seller.CompanyType == (short)AuthorizationServiceEnums.CompanyTypeEnums.Individual &&
                            (string.IsNullOrEmpty(seller.IdentityNumber) || seller.IdentityNumber.Length != 11))
                        {
                            result.AddErrorMessage("Şahıs firması için 11 haneli TC kimlik numarası gereklidir.");
                            return result;
                        }

                        // iyzico sub-merchant kaydı — BLOCKING
                        var paymentSellerResponse = _iyzicoServiceAdapter.CreateSeller(new CreateSubMerchantDto
                        {
                            Id = seller.Id,
                            CompanyType = seller.CompanyType,
                            CompanyName = seller.LegalName,
                            TaxCode = seller.TaxCode,
                            TaxArea = seller.TaxArea,
                            IBAN = seller.IBAN,
                            Address = addressStr,
                            Email = sellerUser.Email,
                            IdentityNumber = seller.IdentityNumber
                        });

                        if (paymentSellerResponse.HasFailed || string.IsNullOrWhiteSpace(paymentSellerResponse.Data))
                        {
                            var errorMsg = paymentSellerResponse.Messages?.FirstOrDefault()?.Description ?? "Bilinmeyen hata";
                            result.Fail($"iyzico alt üye işyeri kaydı başarısız: {errorMsg}");
                            return result;
                        }

                        // Save subMerchantKey to SellerDetail
                        await _sellerDetailRepository.AddAsync(new SellerDetail
                        {
                            SellerId = seller.Id,
                            Key1 = (int)AuthorizationServiceEnums.SellerDetailKey1.PaymentSubMerchantKey,
                            Key2 = (int)AuthorizationServiceEnums.SellerDetailKey2.Iyzico,
                            ValueStr = paymentSellerResponse.Data
                        });

                        seller.CompanyStatus = (short)AuthorizationServiceEnums.CompanyStatusEnums.Approved;
                        await _sellerRepository.UpdateAsync(seller);

                        sellerUser.UserStatusId = (short)AuthorizationServiceEnums.UserStatusEnums.Active;
                        await _userRepository.UpdateAsync(sellerUser);

                        result.SetData(true);
                        //TODO: Satıcı onaylandı maili gönderilecek
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

    public async Task<ServiceObjectResult<bool>> UpdateSeller(UpdateSellerDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var seller = await _sellerRepository.GetAsync(x => x.Id == requestDto.Id, enableTracking: true);
            if (seller == null)
            {
                result.AddErrorMessage("Satıcı bulunamadı.");
                return result;
            }

            seller.Name = requestDto.Name;
            seller.LegalName = requestDto.LegalName;
            seller.TaxCode = requestDto.TaxCode;
            seller.TaxArea = requestDto.TaxArea;
            seller.IBAN = requestDto.IBAN;
            seller.IsEInvoiceAvaible = requestDto.IsEInvoiceAvaible;
            await _sellerRepository.UpdateAsync(seller);
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<GetSellerListResponseDto>> GetSellerList(int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<GetSellerListResponseDto>();
        try
        {
            pageSize = Math.Min(pageSize, 100);
            var sellers = await _sellerRepository.GetListAsync(
                null,
                q => q.OrderByDescending(s => s.CreatedDate),
                index: page - 1,
                size: pageSize);

            var sellerIds = sellers.Items.Select(s => s.Id).ToList();
            var ownerUsers = await _userRepository.GetListAsync(
                x => sellerIds.Contains(x.SellerId!.Value) &&
                     x.UserRoleId == (short)AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
                size: sellerIds.Count * 2);

            var ownerDict = ownerUsers.Items
                .GroupBy(u => u.SellerId!.Value)
                .ToDictionary(g => g.Key, g => g.First());

            var statusNames = new Dictionary<short, string>
            {
                { 1, "Beklemede" }, { 2, "Onaylı" }, { 3, "Reddedildi" }, { 4, "Bloke" }
            };
            var typeNames = new Dictionary<short, string>
            {
                { 1, "Şahıs" }, { 2, "Şirket" }
            };

            var dtos = sellers.Items.Select(s =>
            {
                ownerDict.TryGetValue(s.Id, out var owner);
                return new GetSellerListResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    LegalName = s.LegalName,
                    TaxCode = s.TaxCode,
                    CompanyStatus = s.CompanyStatus,
                    CompanyStatusName = statusNames.GetValueOrDefault(s.CompanyStatus, "?"),
                    CompanyType = s.CompanyType,
                    CompanyTypeName = typeNames.GetValueOrDefault(s.CompanyType, "?"),
                    OwnerEmail = owner?.Email,
                    OwnerFirstName = owner?.FirstName,
                    OwnerLastName = owner?.LastName,
                    CreatedDate = s.CreatedDate
                };
            }).ToList();

            result.SetData(sellers.Count, dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> RetryIyzicoRegistrationAsync(Guid sellerId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var seller = await _sellerRepository.GetAsync(x => x.Id == sellerId);
            if (seller == null)
            {
                result.Fail("Satıcı bulunamadı.");
                return result;
            }

            var sellerUser = await _userRepository.GetAsync(
                x => x.SellerId == seller.Id &&
                     x.UserRoleId == (short)AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
                withDeleted: false);

            if (sellerUser == null)
            {
                result.Fail("Satıcı yöneticisi bulunamadı.");
                return result;
            }

            var sellerAddress = await _addressRepository.GetAsync(a => a.SellerId == seller.Id &&
                                                                       a.AddressType == (short)AuthorizationServiceEnums.AddressTypeEnums.Invoice);
            var addressStr = sellerAddress?.AddressLine1 ?? "Adres bilgisi yok";

            // Check if subMerchantKey already exists
            var existingDetail = await _sellerDetailRepository.GetAsync(d => d.SellerId == seller.Id &&
                                                                             d.Key1 == (int)AuthorizationServiceEnums.SellerDetailKey1.PaymentSubMerchantKey &&
                                                                             d.Key2 == (int)AuthorizationServiceEnums.SellerDetailKey2.Iyzico);

            if (existingDetail != null)
            {
                // Update existing sub-merchant
                var updateResult = _iyzicoServiceAdapter.UpdateSeller(new UpdateSellerRequestDto
                {
                    Id = seller.Id,
                    SellerPaymentId = existingDetail.ValueStr,
                    CompanyType = seller.CompanyType,
                    CompanyName = seller.LegalName,
                    TaxCode = seller.TaxCode,
                    TaxArea = seller.TaxArea,
                    IBAN = seller.IBAN,
                    Address = addressStr,
                    Email = sellerUser.Email
                });

                if (updateResult.HasFailed)
                {
                    var msg = updateResult.Messages?.FirstOrDefault()?.Description ?? "Bilinmeyen hata";
                    result.Fail($"iyzico güncelleme başarısız: {msg}");
                    return result;
                }

                result.SetData(true);
                result.AddSuccessMessage("iyzico kaydı güncellendi.");
            }
            else
            {
                // Create new sub-merchant
                var createResult = _iyzicoServiceAdapter.CreateSeller(new CreateSubMerchantDto
                {
                    Id = seller.Id,
                    CompanyType = seller.CompanyType,
                    CompanyName = seller.LegalName,
                    TaxCode = seller.TaxCode,
                    TaxArea = seller.TaxArea,
                    IBAN = seller.IBAN,
                    Address = addressStr,
                    Email = sellerUser.Email,
                    IdentityNumber = seller.IdentityNumber
                });

                if (createResult.HasFailed || string.IsNullOrWhiteSpace(createResult.Data))
                {
                    var msg = createResult.Messages?.FirstOrDefault()?.Description ?? "Bilinmeyen hata";
                    result.Fail($"iyzico kayıt başarısız: {msg}");
                    return result;
                }

                await _sellerDetailRepository.AddAsync(new SellerDetail
                {
                    SellerId = seller.Id,
                    Key1 = (int)AuthorizationServiceEnums.SellerDetailKey1.PaymentSubMerchantKey,
                    Key2 = (int)AuthorizationServiceEnums.SellerDetailKey2.Iyzico,
                    ValueStr = createResult.Data
                });

                result.SetData(true);
                result.AddSuccessMessage("iyzico kaydı oluşturuldu.");
            }
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}