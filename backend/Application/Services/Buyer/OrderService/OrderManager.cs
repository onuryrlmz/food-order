using Application.Services.Common.TokenService;
using Base.Enums;
using Domain.Dto.Buyer.Order;
using Domain.Dto.Payment;
using Domain.Entities.Buyer;
using Domain.Service;
using Infrastructure.Adapters.IyzicoServiceAdapter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Persistence.IRepositories;
using Persistence.IRepositories.Seller;

namespace Application.Services.Buyer.OrderService;

public class OrderManager : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IIyzicoServiceAdapter _iyzicoAdapter;
    private readonly string _callbackBaseUrl;

    public OrderManager(
        IUnitOfWork unitOfWork,
        ITokenAccessor tokenAccessor,
        IRestaurantRepository restaurantRepository,
        IMenuRepository menuRepository,
        ISubscriptionRepository subscriptionRepository,
        IIyzicoServiceAdapter iyzicoAdapter,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _restaurantRepository = restaurantRepository;
        _menuRepository = menuRepository;
        _subscriptionRepository = subscriptionRepository;
        _iyzicoAdapter = iyzicoAdapter;
        _callbackBaseUrl = configuration["SiteSettings:ServiceUrl"] ?? "http://localhost:7276";
    }

    public async Task<ServiceObjectResult<Guid>> PlaceOrder(PlaceOrderRequestDto requestDto)
    {
        var result = new ServiceObjectResult<Guid>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            if (!requestDto.Items.Any())
            {
                result.Fail("Sipariş en az bir ürün içermelidir.");
                return result;
            }

            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == requestDto.RestaurantId);
            if (restaurant == null)
            {
                result.Fail("Restoran bulunamadı.");
                return result;
            }

            if (!restaurant.IsActive)
            {
                result.Fail("Bu restoran şu an aktif değil.");
                return result;
            }

            if (!restaurant.IsOpen)
            {
                result.Fail("Bu restoran şu an kapalı.");
                return result;
            }

            // Aktif abonelik kontrolü
            var hasActiveSubscription = await _subscriptionRepository.AnyAsync(x =>
                x.RestaurantId == requestDto.RestaurantId &&
                x.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active &&
                x.EndDate >= DateTime.UtcNow);

            if (!hasActiveSubscription)
            {
                result.Fail("Bu restoran şu an siparişe kapalı.");
                return result;
            }

            await _unitOfWork.BeginTransactionAsync();

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = token.UserId,
                SellerId = restaurant.SellerId,
                RestaurantId = requestDto.RestaurantId,
                DeliveryAddressId = requestDto.DeliveryAddressId,
                InvoiceAddressId = requestDto.InvoiceAddressId,
                StatusId = (short)AuthorizationServiceEnums.OrderStatusEnums.Pending,
                PaymentStatusId = (short)AuthorizationServiceEnums.PaymentStatusEnums.Pending,
                PaymentOptionId = requestDto.PaymentOptionId,
                Notes = requestDto.Notes,
                TotalProductPrice = 0,
                ShipmentPrice = 0,
                DiscountAmount = 0,
                TotalPrice = 0,
                OrderItems = new List<OrderItem>()
            };

            decimal totalProductPrice = 0;

            foreach (var item in requestDto.Items)
            {
                var menu = await _menuRepository.GetAsync(x => x.Id == item.MenuId);
                if (menu == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    result.Fail($"Menü bulunamadı: {item.MenuId}");
                    return result;
                }

                if (menu.RestaurantId != requestDto.RestaurantId)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    result.Fail("Sipariş yalnızca tek bir restorana ait ürünler içerebilir.");
                    return result;
                }

                var menuPrice = (decimal)menu.Price;
                var itemTotal = menuPrice * item.Quantity;
                totalProductPrice += itemTotal;

                var snapshotData = new { menuName = menu.Name, unitPrice = menu.Price };
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    MenuId = item.MenuId,
                    Quantity = item.Quantity,
                    UnitPrice = menuPrice,
                    TotalPrice = itemTotal,
                    ItemSnapshotJson = JsonConvert.SerializeObject(snapshotData)
                };

                order.OrderItems.Add(orderItem);
                await _unitOfWork.OrderItemRepository.AddAsync(orderItem);
            }

            if (totalProductPrice < restaurant.MinimumOrderPrice)
            {
                await _unitOfWork.RollbackTransactionAsync();
                result.Fail($"Minimum sipariş tutarı {restaurant.MinimumOrderPrice:C2} olmalıdır.");
                return result;
            }

            order.TotalProductPrice = totalProductPrice;
            order.TotalPrice = totalProductPrice + order.ShipmentPrice - order.DiscountAmount;

            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();

            result.SetData(order.Id);
        }
        catch (Exception e)
        {
            await _unitOfWork.RollbackTransactionAsync();
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<GetOrderResponseDto>> GetOrderById(Guid orderId)
    {
        var result = new ServiceObjectResult<GetOrderResponseDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var order = await _unitOfWork.OrderRepository.GetAsync(
                x => x.Id == orderId,
                include: q => q.Include(o => o.OrderItems));

            if (order == null)
            {
                result.Fail("Sipariş bulunamadı.");
                return result;
            }

            // Admin veya kendi siparişi olmalı
            if (token.Role != AuthorizationServiceEnums.UserRoleEnums.Admin && order.UserId != token.UserId)
            {
                result.Fail("Bu siparişe erişim yetkiniz yok.");
                return result;
            }

            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == order.RestaurantId);

            var dto = MapToDto(order, restaurant?.Name ?? "Bilinmiyor");
            result.SetData(dto);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<GetOrderResponseDto>> GetOrderHistory(int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<GetOrderResponseDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            pageSize = Math.Min(pageSize, 50);
            var orders = await _unitOfWork.OrderRepository.GetListAsync(
                x => x.UserId == token.UserId,
                orderBy: q => q.OrderByDescending(o => o.CreatedDate),
                include: q => q.Include(o => o.OrderItems),
                index: page - 1,
                size: pageSize);

            var restaurantIds = orders.Items.Select(o => o.RestaurantId).Distinct().ToList();
            var restaurants = await _restaurantRepository.GetListAsync(x => restaurantIds.Contains(x.Id), size: restaurantIds.Count);
            var restaurantDict = restaurants.Items.ToDictionary(r => r.Id, r => r.Name);

            var dtos = orders.Items.Select(o => MapToDto(o, restaurantDict.GetValueOrDefault(o.RestaurantId, "Bilinmiyor"))).ToList();
            result.SetData(dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> CancelOrder(Guid orderId, string reason)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var order = await _unitOfWork.OrderRepository.GetAsync(x => x.Id == orderId, enableTracking: true);
            if (order == null || order.UserId != token.UserId)
            {
                result.Fail("Sipariş bulunamadı.");
                return result;
            }

            var cancellableStatuses = new[]
            {
                (short)AuthorizationServiceEnums.OrderStatusEnums.Pending,
                (short)AuthorizationServiceEnums.OrderStatusEnums.Confirmed
            };

            if (!cancellableStatuses.Contains(order.StatusId))
            {
                result.Fail("Bu sipariş artık iptal edilemez.");
                return result;
            }

            order.StatusId = (short)AuthorizationServiceEnums.OrderStatusEnums.Cancelled;
            order.CancellationReason = reason;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateOrderStatus(Guid orderId, short statusId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var order = await _unitOfWork.OrderRepository.GetAsync(x => x.Id == orderId, enableTracking: true);
            if (order == null)
            {
                result.Fail("Sipariş bulunamadı.");
                return result;
            }

            // Satıcı kendi restoranının siparişini güncelleyebilir
            if (token.Role is AuthorizationServiceEnums.UserRoleEnums.SellerAdmin or AuthorizationServiceEnums.UserRoleEnums.SellerUser)
            {
                if (token.RestaurantIds == null || !token.RestaurantIds.Contains(order.RestaurantId))
                {
                    result.Fail("Bu siparişi güncelleme yetkiniz yok.");
                    return result;
                }
            }

            order.StatusId = statusId;
            if (statusId == (short)AuthorizationServiceEnums.OrderStatusEnums.Confirmed)
                order.ConfirmedAt = DateTime.UtcNow;
            else if (statusId == (short)AuthorizationServiceEnums.OrderStatusEnums.Delivered)
                order.DeliveredAt = DateTime.UtcNow;

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CompleteAsync();

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<GetOrderResponseDto>> GetRestaurantOrders(Guid restaurantId, short? statusId = null, int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<GetOrderResponseDto>();
        try
        {
            pageSize = Math.Min(pageSize, 50);
            var orders = await _unitOfWork.OrderRepository.GetListAsync(
                x => x.RestaurantId == restaurantId && (statusId == null || x.StatusId == statusId),
                orderBy: q => q.OrderByDescending(o => o.CreatedDate),
                include: q => q.Include(o => o.OrderItems),
                index: page - 1,
                size: pageSize);

            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == restaurantId);
            var restaurantName = restaurant?.Name ?? "Bilinmiyor";

            var dtos = orders.Items.Select(o => MapToDto(o, restaurantName)).ToList();
            result.SetData(dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<InitiatePaymentResponseDto>> InitiatePayment(Guid orderId, InitiatePaymentRequestDto requestDto)
    {
        var result = new ServiceObjectResult<InitiatePaymentResponseDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            var order = await _unitOfWork.OrderRepository.GetAsync(
                x => x.Id == orderId && x.UserId == token!.UserId);

            if (order == null) { result.Fail("Sipariş bulunamadı."); return result; }
            if (order.PaymentStatusId != (short)AuthorizationServiceEnums.PaymentStatusEnums.Pending)
            {
                result.Fail("Bu sipariş için ödeme zaten işlenmiş."); return result;
            }
            if (order.PaymentOptionId != (short)AuthorizationServiceEnums.PaymentOptionEnums.CreditCard)
            {
                result.Fail("Bu sipariş kart ödemesi gerektirmiyor."); return result;
            }

            var callbackUrl = $"{_callbackBaseUrl}/v1/payment/iyzico/callback";

            var paymentRequest = new IyzicoPaymentRequestDto
            {
                OrderId = orderId,
                Amount = order.TotalPrice,
                BuyerEmail = token!.UserId.ToString() + "@foodorder.app",
                BuyerName = "Müşteri",
                BuyerSurname = "Kullanıcı",
                BuyerPhone = "+905000000000",
                BuyerIp = requestDto.BuyerIp,
                BuyerId = token.UserId.ToString(),
                DeliveryCity = "Istanbul",
                DeliveryAddress = "Teslimat Adresi",
                CardHolderName = requestDto.CardHolderName,
                CardNumber = requestDto.CardNumber,
                ExpireMonth = requestDto.ExpireMonth,
                ExpireYear = requestDto.ExpireYear,
                Cvc = requestDto.Cvc,
                CallbackUrl = callbackUrl
            };

            var paymentResult = await _iyzicoAdapter.InitiatePayment(paymentRequest);
            if (paymentResult.HasFailed)
            {
                result.Fail(paymentResult.Messages);
                return result;
            }

            result.SetData(paymentResult.Data);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceCollectionResult<GetOrderResponseDto>> GetAllOrdersForAdmin(int page = 1, int pageSize = 20, short? statusId = null)
    {
        var result = new ServiceCollectionResult<GetOrderResponseDto>();
        try
        {
            pageSize = Math.Min(pageSize, 100);
            var orders = await _unitOfWork.OrderRepository.GetListAsync(
                x => statusId == null || x.StatusId == statusId,
                orderBy: q => q.OrderByDescending(o => o.CreatedDate),
                include: q => q.Include(o => o.OrderItems),
                index: page - 1,
                size: pageSize);

            var restaurantIds = orders.Items.Select(o => o.RestaurantId).Distinct().ToList();
            var restaurants = await _restaurantRepository.GetListAsync(x => restaurantIds.Contains(x.Id), size: restaurantIds.Count + 1);
            var restaurantDict = restaurants.Items.ToDictionary(r => r.Id, r => r.Name);

            var dtos = orders.Items.Select(o => MapToDto(o, restaurantDict.GetValueOrDefault(o.RestaurantId, "Bilinmiyor"))).ToList();
            result.SetData(orders.Count, dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    private static GetOrderResponseDto MapToDto(Order order, string restaurantName)
    {
        return new GetOrderResponseDto
        {
            Id = order.Id,
            RestaurantId = order.RestaurantId,
            RestaurantName = restaurantName,
            StatusId = order.StatusId,
            StatusName = ((AuthorizationServiceEnums.OrderStatusEnums)order.StatusId).ToString(),
            PaymentStatusId = order.PaymentStatusId,
            PaymentOptionId = order.PaymentOptionId,
            TotalProductPrice = order.TotalProductPrice,
            ShipmentPrice = order.ShipmentPrice,
            DiscountAmount = order.DiscountAmount,
            TotalPrice = order.TotalPrice,
            Notes = order.Notes,
            CancellationReason = order.CancellationReason,
            CreatedDate = order.CreatedDate,
            ConfirmedAt = order.ConfirmedAt,
            DeliveredAt = order.DeliveredAt,
            Items = order.OrderItems?.Select(oi => new GetOrderItemResponseDto
            {
                Id = oi.Id,
                MenuId = oi.MenuId,
                MenuName = TryGetMenuName(oi.ItemSnapshotJson),
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice
            }).ToList() ?? new List<GetOrderItemResponseDto>()
        };
    }

    private static string TryGetMenuName(string? snapshotJson)
    {
        if (string.IsNullOrEmpty(snapshotJson)) return string.Empty;
        try
        {
            var snapshot = JsonConvert.DeserializeObject<dynamic>(snapshotJson);
            return snapshot?.menuName?.ToString() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}
