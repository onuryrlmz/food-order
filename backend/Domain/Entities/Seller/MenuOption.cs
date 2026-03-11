using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class MenuOption : Entity<Guid>
{
    public Guid MenuId { get; set; }
    public Guid? OptionTemplateId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
    public int OrderIndex { get; set; }

    public virtual Menu? Menu { get; set; }
    public virtual OptionTemplate? OptionTemplate { get; set; }
    public virtual ICollection<MenuOptionValue> MenuOptionValues { get; set; }
}