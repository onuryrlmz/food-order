using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class CheckoutFormInitializeResource : IyzipayResource
{
    public string Token { get; set; }
    public string CheckoutFormContent { get; set; }
    public long? TokenExpireTime { get; set; }
    public string PaymentPageUrl { get; set; }
}