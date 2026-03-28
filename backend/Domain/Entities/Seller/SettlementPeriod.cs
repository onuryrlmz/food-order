using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class SettlementPeriod : Entity<Guid>
{
    public Guid SellerId { get; set; }
    public Guid RestaurantId { get; set; }
    public DateTime PeriodDate { get; set; }
    public int TotalOrderCount { get; set; }
    public decimal TotalOrderAmount { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalFixedFee { get; set; }
    public decimal TotalNetAmount { get; set; }
    public short StatusId { get; set; }
    public string? IBAN { get; set; }
    public string? BankTransferRef { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? Notes { get; set; }
    public virtual ICollection<SettlementItem> Items { get; set; }
}
