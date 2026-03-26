namespace Domain.Dto.Buyer.Support;

public class SupportStatsDto
{
    public int TotalTickets { get; set; }
    public int OpenTickets { get; set; }
    public int EscalatedTickets { get; set; }
    public int ResolvedTickets { get; set; }
    public int ClosedTickets { get; set; }
    public double AverageRating { get; set; }
    public double AverageResolutionHours { get; set; }
}
