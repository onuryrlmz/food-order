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
        SellerUser = 5,
        Courier = 6
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
        PaymentPending = 1,
        PaymentFailed = 2,
        CancelledByBuyer = 3,
        WaitingRestaurantApproval = 4,
        RejectedByRestaurant = 5,
        Preparing = 6,
        OnTheWay = 7,
        Delivered = 8
    }

    public enum CourierStatusEnums : short
    {
        Offline = 1,
        Online = 2,
        OnDelivery = 3
    }

    public enum RestaurantCourierStatusEnums : short
    {
        PendingApproval = 0,
        Active = 1,
        RejectedByCourier = 2,
        TerminatedByCourier = 3,
        TerminatedByRestaurant = 4,
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
        CreditCard = 1,
        CashOnDelivery = 2,
        CashCreditCard = 3
    }

    public static Dictionary<short, UserRoleEnums> UserRoleEnumList => Enum.GetValues(typeof(UserRoleEnums)).Cast<UserRoleEnums>().ToDictionary(t => (short)t, t => t);

    public static Dictionary<short, UserStatusEnums> UserStatusEnumList => Enum.GetValues(typeof(UserStatusEnums)).Cast<UserStatusEnums>().ToDictionary(t => (short)t, t => t);

    public static Dictionary<short, SexEnums> SexEnumList => Enum.GetValues(typeof(SexEnums)).Cast<SexEnums>().ToDictionary(t => (short)t, t => t);
}