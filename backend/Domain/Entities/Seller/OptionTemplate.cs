using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class OptionTemplate : Entity<Guid>
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
    public int OrderIndex { get; set; }

    public virtual Restaurant? Restaurant { get; set; }
    public virtual ICollection<OptionTemplateValue> OptionTemplateValues { get; set; }
}
