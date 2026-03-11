namespace Domain.Infrastructure.GetirService.Dto;

public class Menu
{
    public Menu()
    {
        MenuOptions = [];
    }

    public Guid Id { get; set; }
    public string ReferenceId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string ImageUrl { get; set; }
    public double Price { get; set; }
    public int OrderIndex { get; set; }
    public List<MenuOption> MenuOptions { get; set; }
}

public class MenuOption
{
    public MenuOption()
    {
        MenuOptionValues = [];
    }

    public Guid Id { get; set; }
    public string ReferenceId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MaxCount { get; set; }
    public int MinCount { get; set; }
    public int OrderIndex { get; set; }

    public List<MenuOptionValue> MenuOptionValues { get; set; }
}

public class MenuOptionValue
{
    public MenuOptionValue()
    {
        MenuOptionValueOptions = [];
    }

    public Guid Id { get; set; }
    public string ReferenceId { get; set; }
    public string Name { get; set; }
    public string ProductReferenceId { get; set; }
    public double Price { get; set; }
    public int OrderIndex { get; set; }
    public List<MenuOptionValueOption> MenuOptionValueOptions { get; set; }
}

public class MenuOptionValueOption
{
    public MenuOptionValueOption()
    {
        MenuOptionValueOptionValues = [];
    }

    public Guid Id { get; set; }
    public string ReferenceId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MaxCount { get; set; }
    public int MinCount { get; set; }
    public int OrderIndex { get; set; }

    public List<MenuOptionValueOptionValue> MenuOptionValueOptionValues { get; set; }
}

public class MenuOptionValueOptionValue
{
    public Guid Id { get; set; }
    public string ReferenceId { get; set; }
    public string Name { get; set; }
    public string ProductReferenceId { get; set; }
    public double Price { get; set; }
    public int OrderIndex { get; set; }
}