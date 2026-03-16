using Base.Entities;

namespace Domain.Dto.Seller.Subscription;

public class UpgradePreviewDto : IDto
{
    public string CurrentPlanName { get; set; }
    public string TargetPlanName { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal TargetPrice { get; set; }
    public decimal ProratedAmount { get; set; }
    public int RemainingDays { get; set; }
    public int TotalDays { get; set; }
    public int TargetMaxOrders { get; set; }
}
