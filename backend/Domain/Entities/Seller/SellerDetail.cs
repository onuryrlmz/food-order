using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class SellerDetail : Entity<Guid>
{
    public Guid SellerId { get; set; }
    public bool IsActive { get; set; }
    public int Key1 { get; set; }
    public int? Key2 { get; set; }
    public string? ValueStr { get; set; }
    public int? ValueInt { get; set; }
    public decimal? ValueDec { get; set; }
    public DateTime? ValueDate { get; set; }
}