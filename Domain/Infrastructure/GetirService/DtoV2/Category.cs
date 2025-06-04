namespace Domain.Infrastructure.GetirService.DtoV2;

public class Category
{
    public Category()
    {
        Menus = [];
    }

    public Guid Id { get; set; }
    public string ReferenceId { get; set; }
    public string Name { get; set; }
    public List<Menu> Menus { get; set; }
}