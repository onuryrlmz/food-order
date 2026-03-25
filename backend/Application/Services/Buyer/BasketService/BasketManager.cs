using Application.Services.Common.RedisService;
using Application.Services.Common.TokenService;
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
                x => x
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

            var redisKey = $"basket_{_tokenAccessor.GetToken().UserId}";
            var basket = await _redisService.GetValueAsync<GetBasketDto>(redisKey);
            if (basket == null)
            {
                basket = CreateEmptyBasketDto();
                await _redisService.SetValueAsync(redisKey, basket);
            }

            // Enrich from restaurant JSON (CDN) — güncel fiyat, menü adı, kaldırılan menüler
            if (basket.RestaurantId != Guid.Empty && basket.BasketItems != null && basket.BasketItems.Count > 0) await EnrichBasketFromRestaurantJson(basket);

            // Enrich with restaurant name
            if (basket.RestaurantId != Guid.Empty)
            {
                var rest = await _unitOfWork.RestaurantRepository.GetAsync(x => x.Id == basket.RestaurantId);
                if (rest != null) basket.RestaurantName = rest.Name;
            }

            result.SetData(basket);
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

            var redisKey = $"basket_{_tokenAccessor.GetToken().UserId}";
            var existing = await _redisService.GetValueAsync<GetBasketDto>(redisKey);

            var basketDto = new GetBasketDto
            {
                Id = existing?.Id ?? Guid.NewGuid(),
                UserId = _tokenAccessor.GetToken().UserId,
                RestaurantId = dto.RestaurantId,
                SellerId = dto.SellerId,
                UserShippingAddressId = dto.UserShippingAddressId,
                UserInvoiceAddressId = dto.UserInvoiceAddressId,
                StatusId = existing?.StatusId ?? (int)BasketServiceEnums.BasketStatusEnums.Waiting,
                PaymentOptionId = dto.PaymentOptionId,
                TotalQuantity = 0,
                TotalProductPrice = 0,
                TotalShipmentPrice = 0,
                TotalShipmentDiscount = 0,
                TotalDiscount = 0,
                TotalPrice = 0,
                BasketItems = new List<GetBasketDto.GetBasketItemDto>()
            };

            foreach (var item in dto.BasketItems)
            {
                var menu = await _menuRepository.GetAsync(x => x.Id == item.MenuId);
                if (menu == null) continue; // Kaldırılmış menüyü atla

                var basketItem = new GetBasketDto.GetBasketItemDto
                {
                    Id = Guid.NewGuid(),
                    MenuId = item.MenuId,
                    Quantity = item.Quantity,
                    UnitPrice = menu.Price,
                    TotalPrice = menu.Price * item.Quantity,
                    BasketItemValues = new List<GetBasketDto.GetBasketItemDto.GetBasketItemValueDto>()
                };

                basketDto.TotalQuantity += item.Quantity;
                basketDto.TotalProductPrice += menu.Price * item.Quantity;
                basketDto.TotalPrice += menu.Price * item.Quantity;

                foreach (var value in item.BasketItemValues)
                {
                    var menuOptionValue = await _menuOptionValueRepository.GetAsync(x => x.Id == value.MenuOptionValueId);
                    if (menuOptionValue == null) continue;

                    var basketItemValue = new GetBasketDto.GetBasketItemDto.GetBasketItemValueDto
                    {
                        Id = Guid.NewGuid(),
                        MenuOptionId = value.MenuOptionId,
                        MenuOptionValueId = value.MenuOptionValueId,
                        ProductId = value.ProductId,
                        Quantity = value.Quantity,
                        UnitPrice = menuOptionValue.Price,
                        TotalPrice = menuOptionValue.Price * value.Quantity,
                        BasketItemValueItemValues = new List<GetBasketDto.GetBasketItemDto.GetBasketItemValueDto.GetBasketItemValueItemValueDto>()
                    };

                    basketDto.TotalProductPrice += menuOptionValue.Price * value.Quantity;
                    basketDto.TotalPrice += menuOptionValue.Price * value.Quantity;

                    foreach (var itemValue in value.BasketItemValueItemValues)
                    {
                        var movov = await _menuOptionValueOptionValueRepository.GetAsync(x => x.Id == itemValue.MenuOptionValueOptionValueId);
                        if (movov == null) continue;

                        var biviv = new GetBasketDto.GetBasketItemDto.GetBasketItemValueDto.GetBasketItemValueItemValueDto
                        {
                            Id = Guid.NewGuid(),
                            MenuOptionValueOptionId = itemValue.MenuOptionValueOptionId,
                            MenuOptionValueOptionValueId = itemValue.MenuOptionValueOptionValueId,
                            ProductId = itemValue.ProductId,
                            Quantity = itemValue.Quantity,
                            UnitPrice = movov.Price,
                            TotalPrice = movov.Price * itemValue.Quantity
                        };

                        basketDto.TotalProductPrice += movov.Price * itemValue.Quantity;
                        basketDto.TotalPrice += movov.Price * itemValue.Quantity;

                        basketItemValue.BasketItemValueItemValues.Add(biviv);
                    }

                    basketItem.BasketItemValues.Add(basketItemValue);
                }

                basketDto.BasketItems.Add(basketItem);
            }

            await _redisService.SetValueAsync(redisKey, basketDto);
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.SetData(false);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> ClearBasketForRedis()
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            if (_tokenAccessor.GetToken() == null)
            {
                result.SetData(false);
                return result;
            }

            var redisKey = $"basket_{_tokenAccessor.GetToken().UserId}";
            await _redisService.DeleteKeyAsync(redisKey);
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.SetData(false);
        }

        return result;
    }

    private GetBasketDto CreateEmptyBasketDto()
    {
        return new GetBasketDto
        {
            Id = Guid.NewGuid(),
            UserId = _tokenAccessor.GetToken().UserId,
            RestaurantId = Guid.Empty,
            SellerId = Guid.Empty,
            UserShippingAddressId = Guid.Empty,
            UserInvoiceAddressId = Guid.Empty,
            StatusId = (int)BasketServiceEnums.BasketStatusEnums.Waiting,
            PaymentOptionId = (int)BasketServiceEnums.PaymentOptionEnums.CreditCard,
            TotalQuantity = 0,
            TotalProductPrice = 0,
            TotalShipmentPrice = 0,
            TotalShipmentDiscount = 0,
            TotalDiscount = 0,
            TotalPrice = 0,
            BasketItems = new List<GetBasketDto.GetBasketItemDto>()
        };
    }

    private async Task EnrichBasketFromRestaurantJson(GetBasketDto basket)
    {
        try
        {
            // CDN URL'yi Redis cache'den al
            var cdnUrl = await _redisService.GetValueAsync<string>($"RestaurantInfo_{basket.RestaurantId}");
            if (string.IsNullOrEmpty(cdnUrl)) return;

            using var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(cdnUrl);
            var categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CdnCategory>>(json);
            if (categories == null) return;

            // Tüm menüleri düzleştir: menuId -> (name, price)
            var menuMap = new Dictionary<Guid, (string Name, decimal Price)>();
            foreach (var cat in categories)
            {
                if (cat.CategoriesDetail == null) continue;
                foreach (var cd in cat.CategoriesDetail)
                {
                    if (cd.Menus == null) continue;
                    foreach (var m in cd.Menus) menuMap[m.Id] = (m.Name, m.Price);
                }
            }

            // Kaldırılan menüleri sepetten çıkar, fiyatları güncelle
            var itemsToRemove = new List<GetBasketDto.GetBasketItemDto>();
            basket.TotalQuantity = 0;
            basket.TotalProductPrice = 0;
            basket.TotalPrice = 0;

            foreach (var item in basket.BasketItems)
            {
                if (!menuMap.TryGetValue(item.MenuId, out var menuInfo))
                {
                    itemsToRemove.Add(item);
                    continue;
                }

                item.MenuName = menuInfo.Name;
                item.UnitPrice = menuInfo.Price;
                item.TotalPrice = menuInfo.Price * item.Quantity;

                basket.TotalQuantity += item.Quantity;
                basket.TotalProductPrice += item.TotalPrice;
                basket.TotalPrice += item.TotalPrice;

                // Value fiyatları da topla
                if (item.BasketItemValues != null)
                    foreach (var v in item.BasketItemValues)
                    {
                        basket.TotalProductPrice += v.TotalPrice;
                        basket.TotalPrice += v.TotalPrice;

                        if (v.BasketItemValueItemValues != null)
                            foreach (var viv in v.BasketItemValueItemValues)
                            {
                                basket.TotalProductPrice += viv.TotalPrice;
                                basket.TotalPrice += viv.TotalPrice;
                            }
                    }
            }

            foreach (var item in itemsToRemove)
                basket.BasketItems.Remove(item);
        }
        catch
        {
            // CDN erişilemezse mevcut veriyle devam et
        }
    }

    // CDN JSON deserialization modelleri
    private class CdnCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<CdnCategoryDetail> CategoriesDetail { get; set; }
    }

    private class CdnCategoryDetail
    {
        public Guid Id { get; set; }
        public Guid MenuId { get; set; }
        public List<CdnMenu> Menus { get; set; }
    }

    private class CdnMenu
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}