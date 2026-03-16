using Base.Enums;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Common;

public class User : Entity<Guid>
{
    public short UserRoleId { get; set; }
    public short UserStatusId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public short? SexId { get; set; }
    public Guid? SellerId { get; set; }
    public Guid? ActivationKey { get; set; }
    public short CourierStatusId { get; set; } = (short)AuthorizationServiceEnums.CourierStatusEnums.Offline;
    public virtual AuthorizationServiceEnums.UserRoleEnums UserRoleEnum => (AuthorizationServiceEnums.UserRoleEnums)UserRoleId;
    public virtual AuthorizationServiceEnums.UserStatusEnums UserStatusEnum => (AuthorizationServiceEnums.UserStatusEnums)UserStatusId;
}