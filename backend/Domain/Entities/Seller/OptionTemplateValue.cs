using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class OptionTemplateValue : Entity<Guid>
{
    public Guid OptionTemplateId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public int OrderIndex { get; set; }

    public virtual OptionTemplate OptionTemplate { get; set; }
    public virtual Product Product { get; set; }
    public virtual ICollection<OptionTemplateValueOption> OptionTemplateValueOptions { get; set; }
}
