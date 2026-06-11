using System.Text.RegularExpressions;
using Base.Enums;
using Domain.Dto.Seller;

namespace Application.Services.Seller.SellerService;

public static class SellerRegistrationRules
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex IbanRegex = new(@"^TR\d{24}$", RegexOptions.Compiled);

    public static List<string> Validate(AddSellerDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add("İşletme adı zorunludur.");
        if (string.IsNullOrWhiteSpace(dto.LegalName))
            errors.Add("Ticari unvan zorunludur.");
        if (string.IsNullOrWhiteSpace(dto.OwnerFirstName))
            errors.Add("Yetkili adı zorunludur.");
        if (string.IsNullOrWhiteSpace(dto.OwnerLastName))
            errors.Add("Yetkili soyadı zorunludur.");
        if (string.IsNullOrWhiteSpace(dto.AddressLine1))
            errors.Add("Adres zorunludur.");

        if (string.IsNullOrWhiteSpace(dto.OwnerEmail) || !EmailRegex.IsMatch(dto.OwnerEmail.Trim()))
            errors.Add("Geçerli bir e-posta adresi giriniz.");

        if (string.IsNullOrWhiteSpace(dto.OwnerPhone) || dto.OwnerPhone.Count(char.IsDigit) < 10)
            errors.Add("Geçerli bir telefon numarası giriniz (en az 10 hane).");

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
            errors.Add("Şifre en az 8 karakter olmalıdır.");

        var iban = dto.IBAN?.Replace(" ", "").ToUpperInvariant() ?? "";
        if (!IbanRegex.IsMatch(iban))
            errors.Add("Geçerli bir IBAN giriniz (TR + 24 hane).");

        var taxCode = dto.TaxCode?.Trim() ?? "";
        if (taxCode.Length is not (10 or 11) || !taxCode.All(char.IsDigit))
            errors.Add("Vergi numarası 10 veya 11 haneli ve rakamlardan oluşmalıdır.");

        if (dto.CompanyType == (short)CompanyTypeEnums.Individual)
        {
            var tc = dto.IdentityNumber?.Trim() ?? "";
            if (tc.Length != 11 || !tc.All(char.IsDigit) || tc[0] == '0')
                errors.Add("Şahıs işletmesi için geçerli 11 haneli TC kimlik numarası zorunludur.");
        }

        return errors;
    }
}
