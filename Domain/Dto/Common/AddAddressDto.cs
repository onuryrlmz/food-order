namespace Domain.Dto.Common;

public class AddAddressDto
{
    public Guid UserId { get; set; }
    public short AddressType { get; set; }
    public string AddressName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public Guid CityId { get; set; }
    public Guid TownId { get; set; }
    public Guid NeighbourhoodId { get; set; }
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public bool IsDefault { get; set; }
    public short InvoiceType { get; set; }
    public string? TaxCode { get; set; }
    public string? TaxArea { get; set; }
}