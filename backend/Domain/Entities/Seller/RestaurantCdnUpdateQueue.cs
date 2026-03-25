using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

/// <summary>
/// CDN'de restoran JSON dosyasının güncellenmesi gereken restoranları tutar.
/// Background worker bu kuyruğu işler, JSON üretir ve R2/CDN'e yükler.
/// </summary>
public class RestaurantCdnUpdateQueue : Entity<Guid>
{
    public Guid RestaurantId { get; set; }
    public short StatusId { get; set; } = (short)CdnUpdateStatus.Pending;
    public string? ErrorMessage { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public int RetryCount { get; set; } = 0;

    public enum CdnUpdateStatus : short
    {
        Pending = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4
    }
}