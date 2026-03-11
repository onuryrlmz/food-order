using NArchitecture.Core.Application.Dtos;

namespace Domain.Dto.Seller.OptionTemplate;

public class CreateOptionTemplateRequestDto : IDto
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
}

public class UpdateOptionTemplateRequestDto : IDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
}

public class DeleteOptionTemplateRequestDto : IDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
}

public class AddOptionTemplateValueRequestDto : IDto
{
    public Guid OptionTemplateId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
}

public class UpdateOptionTemplateValueRequestDto : IDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
}

public class DeleteOptionTemplateValueRequestDto : IDto
{
    public Guid Id { get; set; }
}

public class AddOptionTemplateValueOptionRequestDto : IDto
{
    public Guid OptionTemplateValueId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
}

public class UpdateOptionTemplateValueOptionRequestDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
}

public class DeleteOptionTemplateValueOptionRequestDto : IDto
{
    public Guid Id { get; set; }
}

public class AddOptionTemplateValueOptionValueRequestDto : IDto
{
    public Guid OptionTemplateValueOptionId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
}

public class UpdateOptionTemplateValueOptionValueRequestDto : IDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
}

public class DeleteOptionTemplateValueOptionValueRequestDto : IDto
{
    public Guid Id { get; set; }
}
