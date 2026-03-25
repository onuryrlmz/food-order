using Domain.Service;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Seller.ImageUploadService;

public interface IImageUploadService
{
    Task<ServiceObjectResult<ImageUploadResponseDto>> UploadImage(IFormFile file, Guid? productId);
    Task<ServiceObjectResult<bool>> DeleteImage(Guid imageId);
}

public class ImageUploadResponseDto
{
    public Guid Id { get; set; }
    public string Url { get; set; }
}