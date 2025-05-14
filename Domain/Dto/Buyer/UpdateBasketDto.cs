using Base.Entities;

namespace Domain.Dto.Buyer;

public class UpdateBasketDto : IDto
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
    public List<BasketItemDto> BasketItems { get; set; }

    public class BasketItemDto
    {
        public Guid Id { get; set; }
        public Guid MenuId { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public List<BasketItemValueDto> BasketItemValues { get; set; }

        public class BasketItemValueDto
        {
            public Guid Id { get; set; }
            public Guid MenuOptionId { get; set; }
            public Guid MenuOptionValueId { get; set; }
            public Guid ProductId { get; set; }
            public double Quantity { get; set; }
            public double UnitPrice { get; set; }
            public double TotalPrice { get; set; }
            public List<BasketItemValueItemValueDto> BasketItemValueItemValues { get; set; }

            public class BasketItemValueItemValueDto
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