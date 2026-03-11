namespace Domain.Dto.Seller.CategoryDetail;

public class CreateCategoryDetailRequestDto
{
    public Guid CategoryId { get; set; }
    public Guid MenuId { get; set; }
    public int OrderIndex { get; set; }
}