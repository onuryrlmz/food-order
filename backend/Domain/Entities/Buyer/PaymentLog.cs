using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Buyer;

public class PaymentLog : Entity<Guid>
{
    public Guid? OrderId { get; set; }
    public Guid? PaymentId { get; set; }
    public string Action { get; set; }
    public string? RequestData { get; set; }
    public string? ResponseData { get; set; }
    public int? StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public int? DurationMs { get; set; }
    public string? IpAddress { get; set; }
}
