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

    [Fact]
    public void Validate_ValidCompanyDto_ReturnsNoErrors()
        => Assert.Empty(SellerRegistrationRules.Validate(ValidDto()));

    [Fact]
    public void Validate_InvalidEmail_ReturnsError()
    {
        var dto = ValidDto();
        dto.OwnerEmail = "gecersiz-eposta";
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("e-posta"));
    }

    [Fact]
    public void Validate_ShortPassword_ReturnsError()
    {
        var dto = ValidDto();
        dto.Password = "kisa";
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("Şifre"));
    }

    [Fact]
    public void Validate_InvalidIban_ReturnsError()
    {
        var dto = ValidDto();
        dto.IBAN = "TR123";
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("IBAN"));
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
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("Vergi"));
    }

    [Fact]
    public void Validate_IndividualWithoutIdentityNumber_ReturnsError()
    {
        var dto = ValidDto();
        dto.CompanyType = (short)CompanyTypeEnums.Individual;
        dto.TaxCode = "12345678901";
        dto.IdentityNumber = null;
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("TC"));
    }

    [Fact]
    public void Validate_IndividualWithValidIdentityNumber_ReturnsNoErrors()
    {
        var dto = ValidDto();
        dto.CompanyType = (short)CompanyTypeEnums.Individual;
        dto.TaxCode = "12345678901";
        dto.IdentityNumber = "12345678901";
        Assert.Empty(SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_MissingBusinessName_ReturnsError()
    {
        var dto = ValidDto();
        dto.Name = "";
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("İşletme adı"));
    }

    [Fact]
    public void Validate_ShortPhone_ReturnsError()
    {
        var dto = ValidDto();
        dto.OwnerPhone = "123";
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("telefon"));
    }
}
