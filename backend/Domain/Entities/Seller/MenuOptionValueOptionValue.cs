using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class MenuOptionValueOptionValue : Entity<Guid>
{
    public Guid MenuOptionValueOptionId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? OptionTemplateValueOptionValueId { get; set; }
    public decimal Price { get; set; }
    public int OrderIndex { get; set; }

    public virtual Product Product { get; set; }
    public virtual MenuOptionValueOption MenuOptionValueOption { get; set; }
}