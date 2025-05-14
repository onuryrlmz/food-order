namespace WebAPI.Controllers;

/*[Route("api/productAttribute")]
[ApiController]
public class ProductAttributeController : BaseController
{
    [HttpPost("add")]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateProductAttributeCommand createProductAttributeCommand)
    {
        var request = new MediatRRequest<CreateProductAttributeCommand, ServiceObjectResult<Guid>>(createProductAttributeCommand, Client);
        return await Mediator.Send(request);
    }

    [HttpPut("update")]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateProductAttributeCommand updateProductAttributeCommand)
    {
        var request = new MediatRRequest<UpdateProductAttributeCommand, ServiceObjectResult<bool>>(updateProductAttributeCommand, Client);
        return await Mediator.Send(request);
    }

    [HttpDelete("delete")]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteProductAttibuteCommand deleteProductAttibuteCommand)
    {
        var request = new MediatRRequest<DeleteProductAttibuteCommand, ServiceObjectResult<bool>>(deleteProductAttibuteCommand, Client);
        return await Mediator.Send(request);
    }

    [HttpPost("getProductAttibutesByProductId")]
    public async Task<ServiceCollectionResult<ProductAttributeResponse>> GetProductAttibutesByProductId([FromBody] GetProductAttibutesByProductIdQuery getProductAttibutesByProductIdQuery)
    {
        var request = new MediatRRequest<GetProductAttibutesByProductIdQuery, ServiceCollectionResult<ProductAttributeResponse>>(getProductAttibutesByProductIdQuery, Client);
        return await Mediator.Send(request);
    }
}*/