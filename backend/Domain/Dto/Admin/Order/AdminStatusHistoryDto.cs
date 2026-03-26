namespace Domain.Dto.Admin.Order;

public class AdminStatusHistoryDto
{
    public Guid Id { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public string? Note { get; set; }
    public DateTime OccurredAt { get; set; }
}
