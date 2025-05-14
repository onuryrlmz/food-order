namespace WebAPI.Controllers;

/*[Route("api/productAttributeValue")]
[ApiController]
public class ProductAttributeValueController : BaseController
{
    [HttpPost("add")]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateProductAttributeValueCommand createProductAttributeValueCommand)
    {
        var request = new MediatRRequest<CreateProductAttributeValueCommand, ServiceObjectResult<Guid>>(createProductAttributeValueCommand, Client);
        return await Mediator.Send(request);
    }

    [HttpPut("update")]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateProductAttributeValueCommand updateProductAttributeValueCommand)
    {
        var request = new MediatRRequest<UpdateProductAttributeValueCommand, ServiceObjectResult<bool>>(updateProductAttributeValueCommand, Client);
        return await Mediator.Send(request);
    }

    [HttpDelete("delete")]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteProductAttibuteValueCommand deleteProductAttibuteValueCommand)
    {
        var request = new MediatRRequest<DeleteProductAttibuteValueCommand, ServiceObjectResult<bool>>(deleteProductAttibuteValueCommand, Client);
        return await Mediator.Send(request);
    }

    [HttpPost("getProductAttibuteValuesByProductAttibuteId")]
    public async Task<ServiceCollectionResult<ProductAttributeValueResponse>> GetProductAttibuteValuesByProductAttibuteId([FromBody] GetProductAttibuteValuesByProductAttibuteIdQuery getProductAttibuteValuesByProductAttibuteIdQuery)
    {
        var request = new MediatRRequest<GetProductAttibuteValuesByProductAttibuteIdQuery, ServiceCollectionResult<ProductAttributeValueResponse>>(getProductAttibuteValuesByProductAttibuteIdQuery, Client);
        return await Mediator.Send(request);
    }
}*/