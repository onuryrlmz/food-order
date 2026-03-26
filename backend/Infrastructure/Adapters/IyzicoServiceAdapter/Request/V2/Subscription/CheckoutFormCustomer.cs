using Infrastructure.Adapters.IyzicoServiceAdapter.Model;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2.Subscription;

public class CheckoutFormCustomer
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string GsmNumber { get; set; }
    public string IdentityNumber { get; set; }
    public Address BillingAddress { get; set; }
    public Address ShippingAddress { get; set; }
}
