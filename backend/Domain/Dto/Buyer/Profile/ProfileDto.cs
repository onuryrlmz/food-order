namespace Domain.Dto.Buyer.Profile;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime? BirthDate { get; set; }
    public short? SexId { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public DateTime CreatedDate { get; set; }
}
