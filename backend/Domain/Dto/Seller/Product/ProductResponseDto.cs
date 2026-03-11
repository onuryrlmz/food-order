using NArchitecture.Core.Application.Dtos;
using Newtonsoft.Json;

namespace Domain.Dto.Seller.Product;

public class ProductResponseDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int ProductType { get; set; }
    public int OrderIndex { get; set; }
}