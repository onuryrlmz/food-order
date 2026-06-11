using System.Text.RegularExpressions;
using Base.Enums;
using Domain.Dto.Seller;

namespace Application.Services.Seller.SellerService;

public static class SellerRegistrationRules
{
    public const string ErrBusinessNameRequired = "İşletme adı zorunludur.";
    public const string ErrLegalNameRequired = "Ticari unvan zorunludur.";
    public const string ErrFirstNameRequired = "Yetkili adı zorunludur.";
    public const string ErrLastNameRequired = "Yetkili soyadı zorunludur.";
    public const string ErrAddressRequired = "Adres zorunludur.";
    public const string ErrInvalidEmail = "Geçerli bir e-posta adresi giriniz.";
    public const string ErrInvalidPhone = "Geçerli bir telefon numarası giriniz (en az 10 hane).";
    public const string ErrShortPassword = "Şifre en az 8 karakter olmalıdır.";
    public const string ErrInvalidIban = "Geçerli bir IBAN giriniz (TR + 24 hane).";
    public const string ErrInvalidTaxCodeCompany = "Şirket için vergi numarası 10 haneli ve rakamlardan oluşmalıdır.";
    public const string ErrInvalidTaxCodeIndividual = "Şahıs işletmesi için vergi kimlik numarası 11 haneli ve rakamlardan oluşmalıdır.";
    public const string ErrInvalidIdentityNumber = "Şahıs işletmesi için geçerli 11 haneli TC kimlik numarası zorunludur.";

    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex IbanRegex = new(@"^TR\d{24}$", RegexOptions.Compiled);

    public static List<string> Validate(AddSellerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add(ErrBusinessNameRequired);
        if (string.IsNullOrWhiteSpace(dto.LegalName))
            errors.Add(ErrLegalNameRequired);
        if (string.IsNullOrWhiteSpace(dto.OwnerFirstName))
            errors.Add(ErrFirstNameRequired);
        if (string.IsNullOrWhiteSpace(dto.OwnerLastName))
            errors.Add(ErrLastNameRequired);
        if (string.IsNullOrWhiteSpace(dto.AddressLine1))
            errors.Add(ErrAddressRequired);

        if (string.IsNullOrWhiteSpace(dto.OwnerEmail) || !EmailRegex.IsMatch(dto.OwnerEmail.Trim()))
            errors.Add(ErrInvalidEmail);

        if (string.IsNullOrWhiteSpace(dto.OwnerPhone) || dto.OwnerPhone.Count(char.IsDigit) < 10)
            errors.Add(ErrInvalidPhone);

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
            errors.Add(ErrShortPassword);

        var iban = dto.IBAN?.Replace(" ", "").ToUpperInvariant() ?? "";
        if (!IbanRegex.IsMatch(iban))
            errors.Add(ErrInvalidIban);

        // Şirket VKN 10 hane; şahıs işletmesinde vergi kimlik no = TC kimlik no (11 hane)
        var taxCode = dto.TaxCode?.Trim() ?? "";
        var isIndividual = dto.CompanyType == (short)CompanyTypeEnums.Individual;
        if (isIndividual)
        {
            if (taxCode.Length != 11 || !taxCode.All(char.IsDigit))
                errors.Add(ErrInvalidTaxCodeIndividual);
        }
        else
        {
            if (taxCode.Length != 10 || !taxCode.All(char.IsDigit))
                errors.Add(ErrInvalidTaxCodeCompany);
        }

        if (isIndividual)
        {
            // Bilinçli karar: yalnızca format kontrolü yapılır; TC checksum doğrulaması
            // İyzico alt üye kaydı ve resmi süreçlere bırakılmıştır (bkz. plan "Kapsam Dışı").
            var tc = dto.IdentityNumber?.Trim() ?? "";
            if (tc.Length != 11 || !tc.All(char.IsDigit) || tc[0] == '0')
                errors.Add(ErrInvalidIdentityNumber);
        }

        return errors;
    }
}
