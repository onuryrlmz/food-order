using Base.Entities;

namespace Domain.Dto.Buyer;

public class GetBasketDto : IDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }
    public string? RestaurantName { get; set; }
    public Guid SellerId { get; set; }
    public Guid UserShippingAddressId { get; set; }
    public Guid UserInvoiceAddressId { get; set; }
    public int StatusId { get; set; }
    public int PaymentOptionId { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalProductPrice { get; set; }
    public decimal TotalShipmentPrice { get; set; }
    public decimal TotalShipmentDiscount { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalPrice { get; set; }
    public List<GetBasketItemDto> BasketItems { get; set; }

    public class GetBasketItemDto
    {
        public Guid Id { get; set; }
        public Guid MenuId { get; set; }
        public string? MenuName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public List<GetBasketItemValueDto> BasketItemValues { get; set; }

        public class GetBasketItemValueDto
        {
            public Guid Id { get; set; }
            public Guid MenuOptionId { get; set; }
            public Guid MenuOptionValueId { get; set; }
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
            public List<GetBasketItemValueItemValueDto> BasketItemValueItemValues { get; set; }

            public class GetBasketItemValueItemValueDto
            {
                public Guid Id { get; set; }
                public Guid MenuOptionValueOptionId { get; set; }
                public Guid MenuOptionValueOptionValueId { get; set; }
                public Guid ProductId { get; set; }
                public int Quantity { get; set; }
                public decimal UnitPrice { get; set; }
                public decimal TotalPrice { get; set; }
            }
        }
    }
}