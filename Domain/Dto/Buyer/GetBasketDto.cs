using Base.Entities;

namespace Domain.Dto.Buyer;

public class GetBasketDto : IDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid SellerId { get; set; }
    public Guid UserShippingAddressId { get; set; }
    public Guid UserInvoiceAddressId { get; set; }
    public int PaymentOptionId { get; set; }
    public int TotalQuantity { get; set; }
    public double TotalProductPrice { get; set; }
    public double TotalShipmentPrice { get; set; }
    public double TotalShipmentDiscount { get; set; }
    public double TotalDiscount { get; set; }
    public double TotalPrice { get; set; }
    public List<GetBasketItemDto> BasketItems { get; set; }

    public class GetBasketItemDto
    {
        public Guid Id { get; set; }
        public Guid MenuId { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public List<GetBasketItemValueDto> BasketItemValues { get; set; }

        public class GetBasketItemValueDto
        {
            public Guid Id { get; set; }
            public Guid MenuOptionId { get; set; }
            public Guid MenuOptionValueId { get; set; }
            public Guid ProductId { get; set; }
            public double Quantity { get; set; }
            public double UnitPrice { get; set; }
            public double TotalPrice { get; set; }
            public List<GetBasketItemValueItemValueDto> BasketItemValueItemValues { get; set; }

            public class GetBasketItemValueItemValueDto
            {
                public Guid Id { get; set; }
                public Guid MenuOptionValueOptionId { get; set; }
                public Guid MenuOptionValueOptionValueId { get; set; }
                public Guid ProductId { get; set; }
                public double Quantity { get; set; }
                public double UnitPrice { get; set; }
                public double TotalPrice { get; set; }
            }
        }
    }
}