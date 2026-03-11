using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2.Subscription;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model.V2.Subscription;

public class Product : IyzipayResourceV2
{
    public static ResponseData<ProductResource> Create(CreateProductRequest request, Options options)
    {
        var uri = $"{options.BaseUrl}/v2/subscription/products";
        return RestHttpClientV2.Create().Post<ResponseData<ProductResource>>(uri, GetHttpHeadersWithRequestBody(request, uri, options), request);
    }

    public static ResponseData<ProductResource> Update(UpdateProductRequest request, Options options)
    {
        var uri = $"{options.BaseUrl}/v2/subscription/products/{request.ProductReferenceCode}";
        return RestHttpClientV2.Create().Post<ResponseData<ProductResource>>(uri, GetHttpHeadersWithRequestBody(request, uri, options), request);
    }

    public static IyzipayResourceV2 Delete(DeleteProductRequest request, Options options)
    {
        var uri = $"{options.BaseUrl}/v2/subscription/products/{request.ProductReferenceCode}";
        return RestHttpClientV2.Create().Delete<IyzipayResourceV2>(uri, GetHttpHeadersWithRequestBody(request, uri, options), request);
    }

    public static ResponseData<ProductResource> Retrieve(RetrieveProductRequest request, Options options)
    {
        var uri = $"{options.BaseUrl}/v2/subscription/products/{request.ProductReferenceCode}";
        return RestHttpClientV2.Create().Get<ResponseData<ProductResource>>(uri, GetHttpHeadersWithUrlParams(request, uri, options));
    }

    public static ResponsePagingData<ProductResource> RetrieveAll(PagingRequest request, Options options)
    {
        var uri = $"{options.BaseUrl}/v2/subscription/products{GetQueryParams(request)}";
        return RestHttpClientV2.Create().Get<ResponsePagingData<ProductResource>>(uri, GetHttpHeadersWithUrlParams(request, uri, options));
    }


    private static string GetQueryParams(BaseRequestV2 request)
    {
        if (request == null) return "";

        var queryParams = "?conversationId=" + request.ConversationId;

        if (!string.IsNullOrEmpty(request.Locale)) queryParams += "&locale=" + request.Locale;

        if (!(request is PagingRequest pagingRequest)) return queryParams;

        if (pagingRequest.Page != null) queryParams += "&page=" + pagingRequest.Page;

        if (pagingRequest.Count != null) queryParams += "&count=" + pagingRequest.Count;
        return queryParams;
    }
}