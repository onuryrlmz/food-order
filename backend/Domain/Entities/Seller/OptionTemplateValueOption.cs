using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class OptionTemplateValueOption : Entity<Guid>
{
    public Guid OptionTemplateValueId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
    public int OrderIndex { get; set; }

    public virtual OptionTemplateValue OptionTemplateValue { get; set; }
    public virtual ICollection<OptionTemplateValueOptionValue> OptionTemplateValueOptionValues { get; set; }
}