namespace Domain.Dto.Buyer.Support;

public class SupportActionDto
{
    public Guid Id { get; set; }
    public short ActionType { get; set; }
    public string ActionData { get; set; }
    public bool IsApproved { get; set; }
    public bool IsExecuted { get; set; }
    public DateTime CreatedDate { get; set; }
}
