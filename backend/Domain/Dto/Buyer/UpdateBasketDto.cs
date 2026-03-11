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
    public List<UpdateBasketItemDto> BasketItems { get; set; }

    public class UpdateBasketItemDto
    {
        public Guid MenuId { get; set; }
        public int Quantity { get; set; }
        public List<UpdateBasketItemValueDto> BasketItemValues { get; set; }

        public class UpdateBasketItemValueDto
        {
            public Guid MenuOptionId { get; set; }
            public Guid MenuOptionValueId { get; set; }
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
            public List<UpdateBasketItemValueItemValueDto> BasketItemValueItemValues { get; set; }

            public class UpdateBasketItemValueItemValueDto
            {
                public Guid MenuOptionValueOptionId { get; set; }
                public Guid MenuOptionValueOptionValueId { get; set; }
                public Guid ProductId { get; set; }
                public int Quantity { get; set; }
            }
        }
    }
}
