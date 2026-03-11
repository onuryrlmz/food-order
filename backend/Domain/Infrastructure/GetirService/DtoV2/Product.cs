namespace Domain.Infrastructure.GetirService.DtoV2;

public class Product
{
    public Product()
    {
        ProductAttributes = new List<ProductAttribute>();
    }

    public Guid Id { get; set; }
    public string ReferenceId { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int ProductType { get; set; }
    public string Hash { get; set; }
    public List<ProductAttribute> ProductAttributes { get; set; }
}

public class ProductAttribute
{
    public ProductAttribute()
    {
        ProductAttributeValues = new List<ProductAttributeValue>();
    }

    public Guid Id { get; set; }
    public Guid MasterProductId { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
    public string? Description { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
    public List<ProductAttributeValue> ProductAttributeValues { get; set; }
}

public class ProductAttributeValue
{
    public Guid Id { get; set; }
    public Guid MasterProductId { get; set; }
    public Guid ProductAttributeId { get; set; }
    public Guid SubProductId { get; set; }
}