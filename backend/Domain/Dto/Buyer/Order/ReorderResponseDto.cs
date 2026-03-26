namespace Domain.Dto.Buyer.Order;

public class ReorderResponseDto
{
    public bool Success { get; set; }
    public List<ReorderWarningDto> Warnings { get; set; } = new();
}
