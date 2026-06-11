using Application.Services.Seller.SellerService;
using Base.Enums;
using Domain.Dto.Seller;
using Xunit;

namespace Application.Tests;

public class SellerRegistrationRulesTests
{
    private static AddSellerDto ValidDto() => new()
    {
        CompanyType = (short)CompanyTypeEnums.Company,
        Name = "Test Lokantası",
        LegalName = "Test Gıda Ltd. Şti.",
        TaxCode = "1234567890",
        TaxArea = "Kadıköy VD",
        IBAN = "TR" + new string('1', 24),
        IsEInvoiceAvaible = false,
        OwnerFirstName = "Onur",
        OwnerLastName = "Test",
        OwnerEmail = "onur@test.com",
        OwnerPhone = "5551112233",
        Password = "Sifre1234",
        AddressLine1 = "Test Mah. Deneme Sk. No:1"
    };

    private static AddSellerDto ValidIndividualDto()
    {
        var dto = ValidDto();
        dto.CompanyType = (short)CompanyTypeEnums.Individual;
        dto.TaxCode = "12345678901";
        dto.IdentityNumber = "12345678901";
        return dto;
    }

    [Fact]
    public void Validate_NullDto_Throws()
        => Assert.Throws<ArgumentNullException>(() => SellerRegistrationRules.Validate(null!));

    [Fact]
    public void Validate_ValidCompanyDto_ReturnsNoErrors()
        => Assert.Empty(SellerRegistrationRules.Validate(ValidDto()));

    [Fact]
    public void Validate_ValidIndividualDto_ReturnsNoErrors()
        => Assert.Empty(SellerRegistrationRules.Validate(ValidIndividualDto()));

    [Fact]
    public void Validate_InvalidEmail_ReturnsError()
    {
        var dto = ValidDto();
        dto.OwnerEmail = "gecersiz-eposta";
        Assert.Contains(SellerRegistrationRules.ErrInvalidEmail, SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_NullEmail_ReturnsError()
    {
        var dto = ValidDto();
        dto.OwnerEmail = null!;
        Assert.Contains(SellerRegistrationRules.ErrInvalidEmail, SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_ShortPassword_ReturnsError()
    {
        var dto = ValidDto();
        dto.Password = "kisa";
        Assert.Contains(SellerRegistrationRules.ErrShortPassword, SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_InvalidIban_ReturnsError()
    {
        var dto = ValidDto();
        dto.IBAN = "TR123";
        Assert.Contains(SellerRegistrationRules.ErrInvalidIban, SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_NullIban_ReturnsError()
    {
        var dto = ValidDto();
        dto.IBAN = null!;
        Assert.Contains(SellerRegistrationRules.ErrInvalidIban, SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_IbanWithSpaces_IsAccepted()
    {
        var dto = ValidDto();
        dto.IBAN = "TR11 1111 1111 1111 1111 1111 11";
        Assert.Empty(SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_NonNumericTaxCode_ReturnsError()
    {
        var dto = ValidDto();
        dto.TaxCode = "12345abcde";
        Assert.Contains(SellerRegistrationRules.ErrInvalidTaxCodeCompany, SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_CompanyWithElevenDigitTaxCode_ReturnsError()
    {
        var dto = ValidDto();
        dto.TaxCode = "12345678901";
        Assert.Contains(SellerRegistrationRules.ErrInvalidTaxCodeCompany, SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_IndividualWithTenDigitTaxCode_ReturnsError()
    {
        var dto = ValidIndividualDto();
        dto.TaxCode = "1234567890";
        Assert.Contains(SellerRegistrationRules.ErrInvalidTaxCodeIndividual, SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_IndividualWithoutIdentityNumber_ReturnsError()
    {
        var dto = ValidIndividualDto();
        dto.IdentityNumber = null;
        Assert.Contains(SellerRegistrationRules.ErrInvalidIdentityNumber, SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_IdentityNumberStartingWithZero_ReturnsError()
    {
        var dto = ValidIndividualDto();
        dto.IdentityNumber = "01234567890";
        Assert.Contains(SellerRegistrationRules.ErrInvalidIdentityNumber, SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_MissingBusinessName_ReturnsError()
    {
        var dto = ValidDto();
        dto.Name = "";
        Assert.Contains(SellerRegistrationRules.ErrBusinessNameRequired, SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_ShortPhone_ReturnsError()
    {
        var dto = ValidDto();
        dto.OwnerPhone = "123";
        Assert.Contains(SellerRegistrationRules.ErrInvalidPhone, SellerRegistrationRules.Validate(dto));
    }
}
