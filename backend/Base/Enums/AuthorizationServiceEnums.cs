using System.ComponentModel;

namespace Base.Enums;

public class AuthorizationServiceEnums
{
    public enum AddressTypeEnums : short
    {
        Invoice = 1,
        Shipping = 2,
        Return = 3,
        Delivery = 4
    }

    public enum CompanyStatusEnums : short
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Blocked = 4
    }

    public enum CompanyTypeEnums : short
    {
        [Description("Şahıs")]
        Individual = 1,

        [Description("Şirket")]
        Company = 2
    }

    public enum InvoiceTypeEnums : short
    {
        Personal = 1,
        Corporate = 2
    }

    public enum SellerDetailKey1
    {
        PaymentSubMerchantKey = 1
    }

    public enum SellerDetailKey2
    {
        Iyzico = 1
    }

    public enum SexEnums : short
    {
        Male = 1,
        Female = 2
    }

    public enum UserRoleEnums : short
    {
        Admin = 1,
        User = 2,
        Anonymous = 3,
        SellerAdmin = 4,
        SellerUser = 5
    }

    public enum UserStatusEnums : short
    {
        WaitingForActivation = 0,
        Active = 1,
        Passive = 2,
        Deleted = 3
    }

    public enum OrderStatusEnums : short
    {
        Pending = 1,
        Confirmed = 2,
        Preparing = 3,
        OnTheWay = 4,
        Delivered = 5,
        Cancelled = 6,
        Rejected = 7
    }

    public enum PaymentStatusEnums : short
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Refunded = 4
    }

    public enum SubscriptionStatusEnums : short
    {
        Active = 1,
        Expired = 2,
        Cancelled = 3,
        Suspended = 4
    }

    public enum SubscriptionPlanTypeEnums : short
    {
        Basic = 1,
        Pro = 2,
        Premium = 3
    }

    public enum PaymentOptionEnums : short
    {
        CashOnDelivery = 1,
        CreditCard = 2,
        OnlineTransfer = 3
    }

    public static Dictionary<short, UserRoleEnums> UserRoleEnumList => Enum.GetValues(typeof(UserRoleEnums)).Cast<UserRoleEnums>().ToDictionary(t => (short)t, t => t);

    public static Dictionary<short, UserStatusEnums> UserStatusEnumList => Enum.GetValues(typeof(UserStatusEnums)).Cast<UserStatusEnums>().ToDictionary(t => (short)t, t => t);

    public static Dictionary<short, SexEnums> SexEnumList => Enum.GetValues(typeof(SexEnums)).Cast<SexEnums>().ToDictionary(t => (short)t, t => t);
}