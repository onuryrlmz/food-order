using Application.Services.Seller.ImageUploadService;
using Base.Enums;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/image")]
[ApiController]
public class SellerImageController : BaseController
{
    private readonly IImageUploadService _imageUploadService;

    public SellerImageController(IImageUploadService imageUploadService)
    {
        _imageUploadService = imageUploadService;
    }

    [HttpPost("upload")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<ImageUploadResponseDto>> Upload(
        IFormFile file,
        [FromQuery] Guid? productId = null)
    {
        return await _imageUploadService.UploadImage(file, productId);
    }

    [HttpDelete("{imageId}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<bool>> Delete(Guid imageId)
    {
        return await _imageUploadService.DeleteImage(imageId);
    }
}