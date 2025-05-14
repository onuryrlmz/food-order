using Base.Enums;
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
}