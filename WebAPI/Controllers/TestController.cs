using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers;

[Route("api/test")]
[ApiController]
public class TestController : BaseController
{
    [HttpPost("add")]
    public async Task<ServiceObjectResult<bool>> Add()
    {
        return new ServiceObjectResult<bool>();
    }
}