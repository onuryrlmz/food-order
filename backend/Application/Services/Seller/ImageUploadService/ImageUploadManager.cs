using Application.Services.Common.TokenService;
using Domain.Entities.Seller;
using Domain.Service;
using Infrastructure.Adapters.AwsS3;
using Microsoft.AspNetCore.Http;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller.ImageUploadService;

public class ImageUploadManager : IImageUploadService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp"
    };

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    private readonly ITokenAccessor _tokenAccessor;
    private readonly IAwsS3ServiceAdapter _s3Adapter;
    private readonly IProductImageRepository _productImageRepository;

    public ImageUploadManager(ITokenAccessor tokenAccessor, IAwsS3ServiceAdapter s3Adapter, IProductImageRepository productImageRepository)
    {
        _tokenAccessor = tokenAccessor;
        _s3Adapter = s3Adapter;
        _productImageRepository = productImageRepository;
    }

    public async Task<ServiceObjectResult<ImageUploadResponseDto>> UploadImage(IFormFile file, Guid? productId)
    {
        var result = new ServiceObjectResult<ImageUploadResponseDto>();
        try
        {
            if (file == null || file.Length == 0)
            {
                result.Fail("No file provided");
                return result;
            }

            if (file.Length > MaxFileSize)
            {
                result.Fail("File size exceeds 5MB limit");
                return result;
            }

            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension))
            {
                result.Fail("Only jpg, png, and webp files are allowed");
                return result;
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                result.Fail("Invalid content type");
                return result;
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var bytes = ms.ToArray();

            var fileName = $"{Guid.NewGuid()}{extension}";
            var url = await _s3Adapter.UploadBytesAsync(bytes, fileName, "product-images", file.ContentType);

            if (string.IsNullOrEmpty(url))
            {
                result.Fail("Upload failed");
                return result;
            }

            var productImage = new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Url = url,
                OrderIndex = 0
            };

            await _productImageRepository.AddAsync(productImage);

            result.SetData(new ImageUploadResponseDto
            {
                Id = productImage.Id,
                Url = url
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> DeleteImage(Guid imageId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var image = await _productImageRepository.GetAsync(x => x.Id == imageId, enableTracking: true);
            if (image == null)
            {
                result.Fail("Image not found");
                return result;
            }

            await _productImageRepository.DeleteAsync(image);
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}
