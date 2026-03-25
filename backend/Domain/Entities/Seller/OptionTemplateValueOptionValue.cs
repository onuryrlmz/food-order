using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class OptionTemplateValueOptionValue : Entity<Guid>
{
    public Guid OptionTemplateValueOptionId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public int OrderIndex { get; set; }

    public virtual OptionTemplateValueOption OptionTemplateValueOption { get; set; }
    public virtual Product Product { get; set; }
}