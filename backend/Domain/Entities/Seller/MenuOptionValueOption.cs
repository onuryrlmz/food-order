using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class MenuOptionValueOption : Entity<Guid>
{
    public Guid MenuOptionValueId { get; set; }
    public Guid? OptionTemplateValueOptionId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
    public int OrderIndex { get; set; }

    public virtual MenuOptionValue MenuOptionValue { get; set; }
    public virtual ICollection<MenuOptionValueOptionValue> MenuOptionValueOptionValues { get; set; }
}