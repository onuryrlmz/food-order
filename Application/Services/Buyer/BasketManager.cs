using Application.Services.Common;
using Base.Entities;
using Base.Enums;
using Domain.Dto.Buyer;
using Domain.Entities.Buyer;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.IRepositories;
using Persistence.IRepositories.Buyer;
using Persistence.IRepositories.Seller;

namespace Application.Services.Buyer;

public class BasketManager : IBasketService
{
    private readonly ITokenAccessor _tokenAccessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBasketRepository _basketRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IMenuOptionValueRepository _menuOptionValueRepository;
    private readonly IMenuOptionValueOptionValueRepository _menuOptionValueOptionValueRepository;

    public BasketManager(ITokenAccessor tokenAccessor, IUnitOfWork unitOfWork,
        IBasketRepository basketRepository, IMenuRepository menuRepository,
        IMenuOptionValueRepository menuOptionValueRepository,
        IMenuOptionValueOptionValueRepository menuOptionValueOptionValueRepository)
    {
        _tokenAccessor = tokenAccessor;
        _unitOfWork = unitOfWork;
        _basketRepository = basketRepository;
        _menuRepository = menuRepository;
        _menuOptionValueRepository = menuOptionValueRepository;
        _menuOptionValueOptionValueRepository = menuOptionValueOptionValueRepository;
    }

    public async Task<ServiceObjectResult<bool>> GetBasketById()
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            if (_tokenAccessor.GetToken() == null)
            {
                result.SetData(false);
                return result;
            }

            Start:
            var basket = await _basketRepository.GetAsync(x => x.UserId == _tokenAccessor.GetToken().UserId && x.StatusId == (int)BasketServiceEnums.BasketStatusEnums.Waiting,
                include: x => x
                    .Include(x0 => x0.BasketItems)
                    .ThenInclude(x2 => x2.BasketItemValues)
                    .ThenInclude(x3 => x3.BasketItemValueItemValues));

            if (basket == null)
            {
                basket = new Basket
                {
                    Id = Guid.NewGuid(),
                    UserId = _tokenAccessor.GetToken()
                        .UserId,
                    UserShippingAddressId = Guid.Empty,
                    UserInvoiceAddressId = Guid.Empty,
                    SellerId = Guid.Empty,
                    RestaurantId = Guid.Empty,
                    StatusId = (int)BasketServiceEnums.BasketStatusEnums.Waiting,
                    PaymentOptionId = (int)BasketServiceEnums.PaymentOptionEnums.CreditCard,
                    TotalQuantity = 0,
                    TotalProductPrice = 0,
                    TotalShipmentPrice = 0,
                    TotalShipmentDiscount = 0,
                    TotalDiscount = 0,
                    TotalPrice = 0
                };

                await _basketRepository.AddAsync(basket);

                goto Start;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateBasket(UpdateBasketDto dto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            if (_tokenAccessor.GetToken() == null)
            {
                result.SetData(false);
                return result;
            }

            await _unitOfWork.BeginTransactionAsync();

            var basket = await _unitOfWork.BasketRepository.GetAsync(x => x.Id == dto.Id, enableTracking: true);
            if (basket == null)
            {
                result.SetData(false);
                return result;
            }

            var oldItemValueItemValues = await _unitOfWork.BasketItemValueItemValueRepository
                .GetListAsync(x => x.BasketItemValue.BasketItem.BasketId == basket.Id, size: 999);

            foreach (var item in oldItemValueItemValues.Items)
                await _unitOfWork.BasketItemValueItemValueRepository.DeleteAsync(item);

            var oldItemValues = await _unitOfWork.BasketItemValueRepository
                .GetListAsync(x => x.BasketItem.BasketId == basket.Id, size: 999);

            foreach (var item in oldItemValues.Items)
                await _unitOfWork.BasketItemValueRepository.DeleteAsync(item);

            var oldItems = await _unitOfWork.BasketItemRepository
                .GetListAsync(x => x.BasketId == basket.Id, size: 999);

            foreach (var item in oldItems.Items)
                await _unitOfWork.BasketItemRepository.DeleteAsync(item);

            basket.UserShippingAddressId = dto.UserShippingAddressId;
            basket.UserInvoiceAddressId = dto.UserInvoiceAddressId;
            basket.RestaurantId = dto.RestaurantId;
            basket.SellerId = dto.SellerId;
            basket.PaymentOptionId = dto.PaymentOptionId;
            basket.TotalQuantity = 0;
            basket.TotalProductPrice = 0;
            basket.TotalShipmentPrice = 0;
            basket.TotalShipmentDiscount = 0;
            basket.TotalDiscount = 0;
            basket.TotalPrice = 0;
            basket.BasketItems = new List<BasketItem>();

            foreach (var item in dto.BasketItems)
            {
                var menu = await _menuRepository.GetAsync(x => x.Id == item.MenuId);
                var basketItem = new BasketItem
                {
                    Id = Guid.NewGuid(),
                    BasketId = basket.Id,
                    MenuId = item.MenuId,
                    Quantity = item.Quantity,
                    UnitPrice = menu?.Price ?? item.UnitPrice,
                    TotalPrice = (menu?.Price ?? item.UnitPrice) * item.Quantity,
                    BasketItemValues = new List<BasketItemValue>()
                };

                basket.TotalQuantity += item.Quantity;
                basket.TotalProductPrice += (menu?.Price ?? item.UnitPrice) * item.Quantity;
                basket.TotalPrice += (menu?.Price ?? item.UnitPrice) * item.Quantity;

                await _unitOfWork.BasketItemRepository.AddAsync(basketItem);

                foreach (var value in item.BasketItemValues)
                {
                    var menuOptionValue = await _menuOptionValueRepository.GetAsync(x => x.Id == value.MenuOptionValueId);

                    var basketItemValue = new BasketItemValue
                    {
                        Id = Guid.NewGuid(),
                        BasketItemId = basketItem.Id,
                        MenuOptionId = value.MenuOptionId,
                        MenuOptionValueId = value.MenuOptionValueId,
                        ProductId = value.ProductId,
                        Quantity = value.Quantity,
                        UnitPrice = menuOptionValue?.Price ?? value.UnitPrice,
                        TotalPrice = (menuOptionValue?.Price ?? value.UnitPrice) * value.Quantity,
                        BasketItemValueItemValues = new List<BasketItemValueItemValue>()
                    };

                    await _unitOfWork.BasketItemValueRepository.AddAsync(basketItemValue);

                    basket.TotalProductPrice += (menuOptionValue?.Price ?? value.UnitPrice) * value.Quantity;
                    basket.TotalPrice += (menuOptionValue?.Price ?? value.UnitPrice) * value.Quantity;

                    foreach (var itemValue in value.BasketItemValueItemValues)
                    {
                        var menuOptionValueOptionValue = await _menuOptionValueOptionValueRepository
                            .GetAsync(x => x.Id == itemValue.MenuOptionValueOptionValueId);
                        var basketItemValueItemValue = new BasketItemValueItemValue
                        {
                            Id = Guid.NewGuid(),
                            BasketItemValueId = basketItemValue.Id,
                            MenuOptionValueOptionId = itemValue.MenuOptionValueOptionId,
                            MenuOptionValueOptionValueId = itemValue.MenuOptionValueOptionValueId,
                            ProductId = itemValue.ProductId,
                            Quantity = itemValue.Quantity,
                            UnitPrice = menuOptionValueOptionValue?.Price ?? itemValue.UnitPrice,
                            TotalPrice = (menuOptionValueOptionValue?.Price ?? itemValue.UnitPrice) * itemValue.Quantity
                        };

                        await _unitOfWork.BasketItemValueItemValueRepository.AddAsync(basketItemValueItemValue);

                        basket.TotalProductPrice += (menuOptionValueOptionValue?.Price ?? itemValue.UnitPrice) * itemValue.Quantity;
                        basket.TotalPrice += (menuOptionValueOptionValue?.Price ?? itemValue.UnitPrice) * itemValue.Quantity;

                        basketItemValue.BasketItemValueItemValues.Add(basketItemValueItemValue);
                    }

                    basketItem.BasketItemValues.Add(basketItemValue);
                }

                basket.BasketItems.Add(basketItem);
            }

            _unitOfWork.BasketRepository.Update(basket);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.SetData(false);
        }

        return result;
    }
}