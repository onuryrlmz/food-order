using Application.Services.Seller._2_RestaurantService;
using Domain.Service;
using Infrastructure.Adapters.YemekSepetiAdapter;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers;

[Route("api/test")]
[ApiController]
public class TestController : BaseController
{
    private readonly IYemekSepetiAdapter _yemekSepetiAdapter;

    public TestController(IYemekSepetiAdapter yemekSepetiAdapter)
    {
        _yemekSepetiAdapter = yemekSepetiAdapter;
    }

    [HttpPost("add")]
    public async Task<ServiceObjectResult<bool>> Add()
    {
        return new ServiceObjectResult<bool>();
    }

    [HttpPost("ys-transfer-restaurant")]
    public async Task<ServiceObjectResult<bool>> YsTransferRestaurant(string ysRestaurantId)
    {
        try
        {
            _yemekSepetiAdapter.TransferRestaurant(ysRestaurantId);
            return new ServiceObjectResult<bool>();
        }
        catch (Exception e)
        {
            return new ServiceObjectResult<bool>();
        }
    }
}