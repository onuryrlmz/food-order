namespace Domain.Dto.Common;

public class UpdateUserProfileDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime? BirthDate { get; set; }
    public short? SexId { get; set; }
}