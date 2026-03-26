namespace Base.Enums;

public enum OrderStatusEnums : short
{
    PaymentPending = 1,
    PaymentFailed = 2,
    CancelledByBuyer = 3,
    WaitingRestaurantApproval = 4,
    RejectedByRestaurant = 5,
    Preparing = 6,
    OnTheWay = 7,
    Delivered = 8,
    CourierAssigned = 9,
    CourierPickedUp = 10
}
