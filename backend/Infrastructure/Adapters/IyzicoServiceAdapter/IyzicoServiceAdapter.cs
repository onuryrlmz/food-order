using Base.Enums;
using Domain.Dto.Buyer.Order;
using Domain.Dto.Payment;
using Domain.Service;
using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Model;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter;

public class IyzicoServiceAdapter : IIyzicoServiceAdapter
{
    private readonly Options options = new()
    {
        ApiKey = "sandbox-bTL1IvKTv1M3zPJ3Lai68zhBB4OIDS3J",
        SecretKey = "sandbox-OmiR6AhVQOm4cZDjHpb0714LQx86vGSR",
        BaseUrl = "https://sandbox-api.iyzipay.com"
    };

    public ServiceObjectResult<string> CreateSeller(CreateSubMerchantDto requestDto)
    {
        var result = new ServiceObjectResult<string>();
        try
        {
            result.SetData(string.Empty);

            var merchantType = requestDto.CompanyType switch
            {
                (short)AuthorizationServiceEnums.CompanyTypeEnums.Company => SubMerchantType.LIMITED_OR_JOINT_STOCK_COMPANY.ToString(),
                (short)AuthorizationServiceEnums.CompanyTypeEnums.Individual => SubMerchantType.PRIVATE_COMPANY.ToString()
            };

            var request = new CreateSubMerchantRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = Guid.NewGuid().ToString(),
                SubMerchantExternalId = requestDto.Id.ToString(),
                SubMerchantType = merchantType,
                Address = requestDto.Address,
                Email = requestDto.Email,
                Name = requestDto.CompanyName,
                Iban = requestDto.IBAN,
                IdentityNumber = requestDto.TaxCode,
                TaxNumber = requestDto.TaxCode,
                Currency = Currency.TRY.ToString(),
                TaxOffice = requestDto.TaxArea,
                LegalCompanyTitle = requestDto.CompanyName
            };

            var subMerchant = SubMerchant.Create(request, options);

            if (subMerchant.Status == Status.SUCCESS.ToString())
                result.SetData(subMerchant.SubMerchantKey);
            else
                result.AddErrorMessage(subMerchant.ErrorMessage);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public ServiceObjectResult<bool> UpdateSeller(UpdateSellerRequestDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var request = new UpdateSubMerchantRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = Guid.NewGuid().ToString(),
                Email = requestDto.Email,
                Address = requestDto.Address,
                Iban = requestDto.IBAN,
                TaxOffice = requestDto.TaxArea,
                LegalCompanyTitle = requestDto.CompanyName,
                SubMerchantKey = requestDto.SellerPaymentId,
                IdentityNumber = requestDto.TaxCode,
                TaxNumber = requestDto.TaxCode
            };

            var subMerchant = SubMerchant.Update(request, options);

            if (subMerchant.Status == Status.SUCCESS.ToString())
                result.SetData(true);
            else
                result.AddErrorMessage(subMerchant.ErrorMessage);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<InitiatePaymentResponseDto>> InitiatePayment(IyzicoPaymentRequestDto requestDto)
    {
        var result = new ServiceObjectResult<InitiatePaymentResponseDto>();
        try
        {
            var request = new CreatePaymentRequest
            {
                Locale = "tr",
                ConversationId = requestDto.OrderId.ToString(),
                Price = requestDto.Amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
                PaidPrice = requestDto.Amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
                Currency = "TRY",
                Installment = 1,
                PaymentChannel = "WEB",
                PaymentGroup = "PRODUCT",
                CallbackUrl = requestDto.CallbackUrl,
                PaymentCard = !string.IsNullOrEmpty(requestDto.CardToken)
                    ? new PaymentCard
                    {
                        CardToken = requestDto.CardToken,
                        CardUserKey = requestDto.CardUserKey
                    }
                    : new PaymentCard
                    {
                        CardHolderName = requestDto.CardHolderName,
                        CardNumber = requestDto.CardNumber,
                        ExpireMonth = requestDto.ExpireMonth,
                        ExpireYear = requestDto.ExpireYear,
                        Cvc = requestDto.Cvc,
                        RegisterCard = requestDto.SaveCard ? 1 : 0
                    },
                Buyer = new Buyer
                {
                    Id = requestDto.BuyerId,
                    Name = requestDto.BuyerName,
                    Surname = requestDto.BuyerSurname,
                    Email = requestDto.BuyerEmail,
                    IdentityNumber = "11111111111",
                    RegistrationAddress = requestDto.DeliveryAddress,
                    Ip = requestDto.BuyerIp,
                    City = requestDto.DeliveryCity,
                    Country = "Turkey",
                    GsmNumber = requestDto.BuyerPhone
                },
                ShippingAddress = new Address
                {
                    ContactName = $"{requestDto.BuyerName} {requestDto.BuyerSurname}",
                    City = requestDto.DeliveryCity,
                    Country = "Turkey",
                    Description = requestDto.DeliveryAddress,
                    ZipCode = "00000"
                },
                BillingAddress = new Address
                {
                    ContactName = $"{requestDto.BuyerName} {requestDto.BuyerSurname}",
                    City = requestDto.DeliveryCity,
                    Country = "Turkey",
                    Description = requestDto.DeliveryAddress,
                    ZipCode = "00000"
                },
                BasketItems = new List<BasketItem>
                {
                    new()
                    {
                        Id = requestDto.OrderId.ToString(),
                        Name = "Yemek Siparişi",
                        Category1 = "Yemek",
                        ItemType = BasketItemType.PHYSICAL.ToString(),
                        Price = requestDto.Amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                    }
                }
            };

            var threedsInit = ThreedsInitialize.Create(request, options);

            if (threedsInit.Status == "success")
            {
                result.SetData(new InitiatePaymentResponseDto
                {
                    RequiresThreeDs = true,
                    ThreeDsHtmlContent = threedsInit.HtmlContent,
                    IsSuccess = true
                });
            }
            else
            {
                result.SetData(new InitiatePaymentResponseDto
                {
                    RequiresThreeDs = false,
                    IsSuccess = false,
                    ErrorMessage = threedsInit.ErrorMessage
                });
            }
        }
        catch (Exception e) { result.Fail(e); }
        return await Task.FromResult(result);
    }

    public async Task<ServiceObjectResult<ThreeDsCompleteResultDto>> CompleteThreeDsPayment(string conversationId, string paymentId, string conversationData)
    {
        var result = new ServiceObjectResult<ThreeDsCompleteResultDto>();
        try
        {
            var request = new CreateThreedsPaymentRequest
            {
                Locale = "tr",
                ConversationId = conversationId,
                PaymentId = paymentId,
                ConversationData = conversationData
            };

            var payment = ThreedsPayment.Create(request, options);

            if (payment.Status == "success")
            {
                var dto = new ThreeDsCompleteResultDto
                {
                    Success = true,
                    CardUserKey = payment.CardUserKey
                };

                // Kart kaydetme yapıldıysa kart detaylarını da doldur
                if (!string.IsNullOrEmpty(payment.CardUserKey) && !string.IsNullOrEmpty(payment.BinNumber))
                {
                    dto.CardToken = payment.CardToken;
                    dto.BinNumber = payment.BinNumber;
                    dto.LastFourDigits = payment.LastFourDigits;
                    dto.CardType = payment.CardType;
                    dto.CardAssociation = payment.CardAssociation;
                }

                result.SetData(dto);
            }
            else
            {
                result.AddErrorMessage(payment.ErrorMessage ?? "Ödeme tamamlanamadı.");
            }
        }
        catch (Exception e) { result.Fail(e); }
        return await Task.FromResult(result);
    }

    public ServiceObjectResult<CardStorageResultDto> CreateCard(string externalId, string email, string? cardUserKey, string cardAlias, string cardNumber, string expireYear, string expireMonth, string cardHolderName)
    {
        var result = new ServiceObjectResult<CardStorageResultDto>();
        try
        {
            var request = new CreateCardRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = Guid.NewGuid().ToString(),
                ExternalId = externalId,
                Email = email,
                CardUserKey = cardUserKey,
                Card = new CardInformation
                {
                    CardAlias = cardAlias,
                    CardNumber = cardNumber,
                    ExpireYear = expireYear,
                    ExpireMonth = expireMonth,
                    CardHolderName = cardHolderName
                }
            };

            var card = Card.Create(request, options);

            if (card.Status == Status.SUCCESS.ToString())
            {
                result.SetData(new CardStorageResultDto
                {
                    CardUserKey = card.CardUserKey,
                    CardToken = card.CardToken,
                    BinNumber = card.BinNumber,
                    LastFourDigits = card.LastFourDigits,
                    CardType = card.CardType,
                    CardAssociation = card.CardAssociation,
                    CardFamily = card.CardFamily,
                    CardAlias = card.CardAlias,
                    CardBankName = card.CardBankName
                });
            }
            else
            {
                result.AddErrorMessage(card.ErrorMessage ?? "Kart kaydedilemedi.");
            }
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public ServiceObjectResult<List<CardDetailDto>> GetCards(string cardUserKey)
    {
        var result = new ServiceObjectResult<List<CardDetailDto>>();
        try
        {
            var request = new RetrieveCardListRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = Guid.NewGuid().ToString(),
                CardUserKey = cardUserKey
            };

            var cardList = CardList.Retrieve(request, options);

            if (cardList.Status == Status.SUCCESS.ToString())
            {
                var cards = cardList.CardDetails?.Select(c => new CardDetailDto
                {
                    CardToken = c.CardToken,
                    CardAlias = c.CardAlias,
                    BinNumber = c.BinNumber,
                    LastFourDigits = c.LastFourDigits,
                    CardType = c.CardType,
                    CardAssociation = c.CardAssociation,
                    CardFamily = c.CardFamily,
                    CardBankName = c.CardBankName,
                    CardBankCode = c.CardBankCode,
                    ExpireMonth = c.ExpireMonth,
                    ExpireYear = c.ExpireYear
                }).ToList() ?? new List<CardDetailDto>();

                result.SetData(cards);
            }
            else
            {
                result.SetData(new List<CardDetailDto>());
            }
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public ServiceObjectResult<bool> DeleteCard(string cardUserKey, string cardToken)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var request = new DeleteCardRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = Guid.NewGuid().ToString(),
                CardUserKey = cardUserKey,
                CardToken = cardToken
            };

            var deleteResult = Card.Delete(request, options);

            if (deleteResult.Status == Status.SUCCESS.ToString())
                result.SetData(true);
            else
                result.AddErrorMessage(deleteResult.ErrorMessage ?? "Kart silinemedi.");
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }
}
