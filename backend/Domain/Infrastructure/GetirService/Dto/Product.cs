namespace Domain.Infrastructure.GetirService.Dto;

public class Product
{
    public Guid Id { get; set; }
    public string ReferenceId { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int ProductType { get; set; }
    public string Hash { get; set; }
}