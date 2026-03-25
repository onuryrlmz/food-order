namespace Domain.Dto.Common;

public class GetUserListResponseDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public short UserRoleId { get; set; }
    public string UserRoleName { get; set; } = string.Empty;
    public short UserStatusId { get; set; }
    public string UserStatusName { get; set; } = string.Empty;
    public Guid? SellerId { get; set; }
    public DateTime CreatedDate { get; set; }
}