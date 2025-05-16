using Application.Services.Common;
using AutoMapper;
using Base.Enums;
using Domain.Dto.Buyer;
using Domain.Entities.Buyer;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.IRepositories;
using Persistence.IRepositories.Buyer;
using Persistence.IRepositories.Seller;

namespace Application.Services.Buyer.BasketService;

public class BasketManager : IBasketService
{
    private readonly IMapper _mapper;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisService _redisService;
    private readonly IBasketRepository _basketRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IMenuOptionValueRepository _menuOptionValueRepository;
    private readonly IMenuOptionValueOptionValueRepository _menuOptionValueOptionValueRepository;

    public BasketManager(
        IMapper mapper,
        ITokenAccessor tokenAccessor,
        IUnitOfWork unitOfWork,
        IRedisService redisService,
        IBasketRepository basketRepository,
        IMenuRepository menuRepository,
        IMenuOptionValueRepository menuOptionValueRepository,
        IMenuOptionValueOptionValueRepository menuOptionValueOptionValueRepository)
    {
        _mapper = mapper;
        _tokenAccessor = tokenAccessor;
        _unitOfWork = unitOfWork;
        _redisService = redisService;
        _basketRepository = basketRepository;
        _menuRepository = menuRepository;
        _menuOptionValueRepository = menuOptionValueRepository;
        _menuOptionValueOptionValueRepository = menuOptionValueOptionValueRepository;
    }

    public async Task<ServiceObjectResult<Basket>> GetBasketByIdForDb()
    {
        var result = new ServiceObjectResult<Basket>();
        try
        {
            if (_tokenAccessor.GetToken() == null)
            {
                result.AddErrorMessage("Kullanıcı bulunamadı.");
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

            result.SetData(basket);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<GetBasketDto>> GetBasketByIdForRedis()
    {
        var result = new ServiceObjectResult<GetBasketDto>();
        try
        {
            if (_tokenAccessor.GetToken() == null)
            {
                result.AddErrorMessage("Kullanıcı bulunamadı.");
                return result;
            }

            Start:
            var basket = await _redisService.GetValueAsync<Basket>("basket_" + _tokenAccessor.GetToken().UserId);
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

                await _redisService.SetValueAsync("basket_" + _tokenAccessor.GetToken().UserId, basket);

                goto Start;
            }

            var basketDto = _mapper.Map<GetBasketDto>(basket);
            result.SetData(basketDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateBasketForDb(UpdateBasketDto dto)
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
                    UnitPrice = menu.Price,
                    TotalPrice = menu.Price * item.Quantity,
                    BasketItemValues = new List<BasketItemValue>()
                };

                basket.TotalQuantity += item.Quantity;
                basket.TotalProductPrice += menu.Price * item.Quantity;
                basket.TotalPrice += menu.Price * item.Quantity;

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
                        UnitPrice = menuOptionValue.Price,
                        TotalPrice = menuOptionValue.Price * value.Quantity,
                        BasketItemValueItemValues = new List<BasketItemValueItemValue>()
                    };

                    await _unitOfWork.BasketItemValueRepository.AddAsync(basketItemValue);

                    basket.TotalProductPrice += menuOptionValue.Price * value.Quantity;
                    basket.TotalPrice += menuOptionValue.Price * value.Quantity;

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
                            UnitPrice = menuOptionValueOptionValue.Price,
                            TotalPrice = menuOptionValueOptionValue.Price * itemValue.Quantity
                        };

                        await _unitOfWork.BasketItemValueItemValueRepository.AddAsync(basketItemValueItemValue);

                        basket.TotalProductPrice += menuOptionValueOptionValue.Price * itemValue.Quantity;
                        basket.TotalPrice += menuOptionValueOptionValue.Price * itemValue.Quantity;

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

    public async Task<ServiceObjectResult<bool>> UpdateBasketForRedis(UpdateBasketDto dto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            if (_tokenAccessor.GetToken() == null)
            {
                result.SetData(false);
                return result;
            }

            var basket = await _redisService.GetValueAsync<Basket>($"basket_{_tokenAccessor.GetToken().UserId}");
            if (basket == null)
            {
                result.AddErrorMessage("Sepet bulunamadı.");
                return result;
            }

            var newBasket = await ConvertDtoToBasketEntitiy(dto, basket);
            if (newBasket?.Data == null)
            {
                result.AddErrorMessage("Sepet güncellenemedi.");
                return result;
            }

            await _redisService.SetValueAsync($"basket_{_tokenAccessor.GetToken().UserId}", newBasket.Data);

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.SetData(false);
        }

        return result;
    }

    private async Task<ServiceObjectResult<Basket>?> ConvertDtoToBasketEntitiy(UpdateBasketDto dto, Basket oldBasket)
    {
        var result = new ServiceObjectResult<Basket>();
        try
        {
            var basket = new Basket
            {
                Id = oldBasket.Id,
                CreatedDate = oldBasket.CreatedDate,
                UpdatedDate = oldBasket.UpdatedDate,
                UserId = oldBasket.UserId,
                UserShippingAddressId = dto.UserShippingAddressId,
                UserInvoiceAddressId = dto.UserInvoiceAddressId,
                RestaurantId = dto.RestaurantId,
                StatusId = oldBasket.StatusId,
                SellerId = dto.SellerId,
                PaymentOptionId = dto.PaymentOptionId,
                TotalQuantity = 0,
                TotalProductPrice = 0,
                TotalShipmentPrice = 0,
                TotalShipmentDiscount = 0,
                TotalDiscount = 0,
                TotalPrice = 0,
                BasketItems = new List<BasketItem>()
            };

            foreach (var item in dto.BasketItems)
            {
                var menu = await _menuRepository.GetAsync(x => x.Id == item.MenuId);
                if (menu == null)
                {
                    result.AddErrorMessage("Menu bulunamadı.");
                    return result;
                }

                var basketItem = new BasketItem
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    BasketId = basket.Id,
                    MenuId = item.MenuId,
                    Quantity = item.Quantity,
                    UnitPrice = menu.Price,
                    TotalPrice = menu.Price * item.Quantity,
                    BasketItemValues = new List<BasketItemValue>()
                };

                basket.TotalQuantity += item.Quantity;
                basket.TotalProductPrice += menu.Price * item.Quantity;
                basket.TotalPrice += menu.Price * item.Quantity;

                foreach (var value in item.BasketItemValues)
                {
                    var menuOptionValue = await _menuOptionValueRepository.GetAsync(x => x.Id == value.MenuOptionValueId);
                    if (menuOptionValue == null)
                    {
                        result.AddErrorMessage("Menu option value bulunamadı.");
                        return result;
                    }

                    var basketItemValue = new BasketItemValue
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        BasketItemId = basketItem.Id,
                        MenuOptionId = value.MenuOptionId,
                        MenuOptionValueId = value.MenuOptionValueId,
                        ProductId = value.ProductId,
                        Quantity = value.Quantity,
                        UnitPrice = menuOptionValue.Price,
                        TotalPrice = menuOptionValue.Price * value.Quantity,
                        BasketItemValueItemValues = new List<BasketItemValueItemValue>()
                    };

                    basket.TotalProductPrice += menuOptionValue.Price * value.Quantity;
                    basket.TotalPrice += menuOptionValue.Price * value.Quantity;

                    foreach (var itemValue in value.BasketItemValueItemValues)
                    {
                        var menuOptionValueOptionValue = await _menuOptionValueOptionValueRepository
                            .GetAsync(x => x.Id == itemValue.MenuOptionValueOptionValueId);
                        if (menuOptionValueOptionValue == null)
                        {
                            result.AddErrorMessage("Menu option value option value bulunamadı.");
                            return result;
                        }

                        var basketItemValueItemValue = new BasketItemValueItemValue
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            BasketItemValueId = basketItemValue.Id,
                            MenuOptionValueOptionId = itemValue.MenuOptionValueOptionId,
                            MenuOptionValueOptionValueId = itemValue.MenuOptionValueOptionValueId,
                            ProductId = itemValue.ProductId,
                            Quantity = itemValue.Quantity,
                            UnitPrice = menuOptionValueOptionValue.Price,
                            TotalPrice = menuOptionValueOptionValue.Price * itemValue.Quantity
                        };

                        basket.TotalProductPrice += menuOptionValueOptionValue.Price * itemValue.Quantity;
                        basket.TotalPrice += menuOptionValueOptionValue.Price * itemValue.Quantity;

                        basketItemValue.BasketItemValueItemValues.Add(basketItemValueItemValue);
                    }

                    basketItem.BasketItemValues.Add(basketItemValue);
                }

                basket.BasketItems.Add(basketItem);
            }

            result.SetData(basket);
            return result;
        }
        catch (Exception e)
        {
            result.AddErrorMessage(e.Message);
            return result;
        }
    }
}