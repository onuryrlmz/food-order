using Application.Services.Buyer.BasketService;
using Application.Services.Buyer.PaymentService;
using Application.Services.Common.TokenService;
using Application.Services.Courier.DeliveryAssignmentService;
using Application.Utils;
using Base.Enums;
using Domain.Dto.Admin.Order;
using Domain.Dto.Buyer;
using Domain.Dto.Buyer.Order;
using Domain.Dto.Payment;
using Domain.Entities.Buyer;
using Domain.Entities.Common;
using Domain.Entities.Seller;
using Domain.Service;
using Application.Services.Common.NotificationService;
using Infrastructure.Adapters.IyzicoServiceAdapter;
using Infrastructure.Adapters.OneSignalAdapter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Persistence.Contexts;
using Persistence.IRepositories;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Seller;

namespace Application.Services.Buyer.OrderService;

public class OrderManager : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IIyzicoServiceAdapter _iyzicoAdapter;
    private readonly IUserRepository _userRepository;
    private readonly IPaymentService _paymentService;
    private readonly BaseDbContext _context;
    private readonly string _callbackBaseUrl;
    private readonly INotificationService _notificationService;
    private readonly IRealtimeNotifier _realtimeNotifier;
    private readonly IBasketService _basketService;
    private readonly IDeliveryAssignmentService _deliveryAssignmentService;

    public OrderManager(
        IUnitOfWork unitOfWork,
        ITokenAccessor tokenAccessor,
        IRestaurantRepository restaurantRepository,
        IMenuRepository menuRepository,
        IIyzicoServiceAdapter iyzicoAdapter,
        IUserRepository userRepository,
        IPaymentService paymentService,
        BaseDbContext context,
        IConfiguration configuration,
        INotificationService notificationService,
        IRealtimeNotifier realtimeNotifier,
        IBasketService basketService,
        IDeliveryAssignmentService deliveryAssignmentService)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
        _restaurantRepository = restaurantRepository;
        _menuRepository = menuRepository;
        _iyzicoAdapter = iyzicoAdapter;
        _userRepository = userRepository;
        _paymentService = paymentService;
        _context = context;
        _callbackBaseUrl = configuration["SiteSettings:ServiceUrl"] ?? "http://localhost:7276";
        _notificationService = notificationService;
        _realtimeNotifier = realtimeNotifier;
        _basketService = basketService;
        _deliveryAssignmentService = deliveryAssignmentService;
    }

    public async Task<ServiceObjectResult<PlaceOrderResponseDto>> PlaceOrder(PlaceOrderRequestDto requestDto)
    {
        var result = new ServiceObjectResult<PlaceOrderResponseDto>();
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

            await _unitOfWork.BeginTransactionAsync();

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = token.UserId,
                SellerId = restaurant.SellerId,
                RestaurantId = requestDto.RestaurantId,
                DeliveryAddressId = requestDto.DeliveryAddressId,
                InvoiceAddressId = requestDto.InvoiceAddressId,
                StatusId = (short)OrderStatusEnums.PaymentPending,
                PaymentStatusId = (short)PaymentStatusEnums.Pending,
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

                var menuBasePrice = (decimal)menu.Price;

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    MenuId = item.MenuId,
                    Quantity = item.Quantity,
                    OrderItemValues = new List<OrderItemValue>()
                };

                // Seçenekleri işle, fiyatları topla ve snapshot için topla
                decimal itemOptionsTotal = 0;
                var snapshotValues = new List<object>();
                foreach (var val in item.Values ?? new List<PlaceOrderItemValueDto>())
                {
                    var menuOption = await _context.Set<MenuOption>().FirstOrDefaultAsync(x => x.Id == val.MenuOptionId);
                    var menuOptionValue = await _context.Set<MenuOptionValue>().Include(v => v.Product).FirstOrDefaultAsync(x => x.Id == val.MenuOptionValueId);
                    if (menuOption == null || menuOptionValue == null) continue;

                    var valPrice = menuOptionValue.Price;
                    var valTotal = valPrice * val.Quantity;
                    itemOptionsTotal += valTotal;

                    var orderItemValue = new OrderItemValue
                    {
                        Id = Guid.NewGuid(),
                        OrderItemId = orderItem.Id,
                        MenuOptionId = val.MenuOptionId,
                        MenuOptionValueId = val.MenuOptionValueId,
                        ProductId = menuOptionValue.ProductId,
                        Quantity = val.Quantity,
                        UnitPrice = valPrice,
                        TotalPrice = valTotal,
                        OrderItemValueOptions = new List<OrderItemValueOption>()
                    };

                    var snapshotOptions = new List<object>();
                    foreach (var opt in val.Options ?? new List<PlaceOrderItemValueOptionDto>())
                    {
                        var movOption = await _context.Set<MenuOptionValueOption>().FirstOrDefaultAsync(x => x.Id == opt.MenuOptionValueOptionId);
                        var movOptionValue = await _context.Set<MenuOptionValueOptionValue>().Include(v => v.Product).FirstOrDefaultAsync(x => x.Id == opt.MenuOptionValueOptionValueId);
                        if (movOption == null || movOptionValue == null) continue;

                        var optPrice = movOptionValue.Price;
                        var optTotal = optPrice * opt.Quantity;
                        itemOptionsTotal += optTotal;

                        orderItemValue.OrderItemValueOptions.Add(new OrderItemValueOption
                        {
                            Id = Guid.NewGuid(),
                            OrderItemValueId = orderItemValue.Id,
                            MenuOptionValueOptionId = opt.MenuOptionValueOptionId,
                            MenuOptionValueOptionValueId = opt.MenuOptionValueOptionValueId,
                            ProductId = movOptionValue.ProductId,
                            Quantity = opt.Quantity,
                            UnitPrice = optPrice,
                            TotalPrice = optTotal
                        });

                        snapshotOptions.Add(new
                        {
                            optionName = movOption.Name,
                            valueName = movOptionValue.Product?.Name,
                            quantity = opt.Quantity,
                            unitPrice = optPrice,
                            totalPrice = optTotal
                        });
                    }

                    orderItem.OrderItemValues.Add(orderItemValue);

                    snapshotValues.Add(new
                    {
                        optionName = menuOption.Name,
                        valueName = menuOptionValue.Product?.Name,
                        quantity = val.Quantity,
                        unitPrice = valPrice,
                        totalPrice = valTotal,
                        options = snapshotOptions
                    });
                }

                // Birim fiyat = menü base + seçenek fiyatları
                var unitPrice = menuBasePrice + itemOptionsTotal;
                var itemTotal = unitPrice * item.Quantity;
                orderItem.UnitPrice = unitPrice;
                orderItem.TotalPrice = itemTotal;
                totalProductPrice += itemTotal;

                var snapshotData = new
                {
                    menuName = menu.Name,
                    description = menu.Description,
                    unitPrice = unitPrice,
                    menuBasePrice = menuBasePrice,
                    optionsTotal = itemOptionsTotal,
                    values = snapshotValues
                };
                orderItem.ItemSnapshotJson = JsonConvert.SerializeObject(snapshotData);

                order.OrderItems.Add(orderItem);
            }

            if (totalProductPrice < restaurant.MinimumOrderPrice)
            {
                await _unitOfWork.RollbackTransactionAsync();
                result.Fail($"Minimum sipariş tutarı {restaurant.MinimumOrderPrice:C2} olmalıdır.");
                return result;
            }

            order.TotalProductPrice = totalProductPrice;
            decimal discountAmount = 0;

            // Apply coupon if provided
            if (requestDto.CouponId.HasValue && requestDto.CouponId.Value != Guid.Empty)
            {
                var coupon = await _context.Coupons
                    .Include(c => c.CouponMenus)
                    .Include(c => c.CouponCategories)
                    .FirstOrDefaultAsync(c => c.Id == requestDto.CouponId.Value && c.DeletedDate == null);

                if (coupon != null)
                {
                    // Validate coupon is still valid
                    var now = DateTime.UtcNow;
                    var isValid = now >= coupon.StartDate && now <= coupon.EndDate
                                                          && (!coupon.UsageLimit.HasValue || coupon.CurrentUsageCount < coupon.UsageLimit.Value)
                                                          && (coupon.RestaurantId == null || coupon.RestaurantId == requestDto.RestaurantId)
                                                          && totalProductPrice >= coupon.MinOrderAmount;

                    if (isValid && coupon.UsagePerUser.HasValue)
                    {
                        var userUsage = await _context.Set<UserCoupon>()
                            .Where(uc => uc.UserId == token.UserId && uc.CouponId == coupon.Id)
                            .SumAsync(uc => uc.UsageCount);
                        if (userUsage >= coupon.UsagePerUser.Value)
                            isValid = false;
                    }

                    if (isValid)
                    {
                        // Calculate discount
                        var cartItems = requestDto.Items.Select(i =>
                        {
                            var menu = order.OrderItems.FirstOrDefault(oi => oi.MenuId == i.MenuId);
                            return new { i.MenuId, i.Quantity, UnitPrice = menu?.UnitPrice ?? 0 };
                        }).ToList();

                        discountAmount = coupon.Type switch
                        {
                            CouponServiceEnums.CouponTypeEnums.Percentage => totalProductPrice * (coupon.Value / 100m),
                            CouponServiceEnums.CouponTypeEnums.FixedAmount => coupon.Value,
                            CouponServiceEnums.CouponTypeEnums.BuyXGetY => 0, // simplified
                            _ => 0
                        };

                        if (coupon.Type == CouponServiceEnums.CouponTypeEnums.Percentage && coupon.MaxDiscountAmount.HasValue)
                            discountAmount = Math.Min(discountAmount, coupon.MaxDiscountAmount.Value);

                        discountAmount = Math.Min(discountAmount, totalProductPrice);

                        // Handle SpecificMenus / SpecificCategories scope
                        if (coupon.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.SpecificMenus)
                        {
                            var menuIds = coupon.CouponMenus?.Select(cm => cm.MenuId).ToHashSet() ?? new HashSet<Guid>();
                            var applicableTotal = order.OrderItems.Where(oi => menuIds.Contains(oi.MenuId)).Sum(oi => oi.TotalPrice);
                            discountAmount = coupon.Type switch
                            {
                                CouponServiceEnums.CouponTypeEnums.Percentage => applicableTotal * (coupon.Value / 100m),
                                CouponServiceEnums.CouponTypeEnums.FixedAmount => Math.Min(coupon.Value, applicableTotal),
                                _ => 0
                            };
                            if (coupon.Type == CouponServiceEnums.CouponTypeEnums.Percentage && coupon.MaxDiscountAmount.HasValue)
                                discountAmount = Math.Min(discountAmount, coupon.MaxDiscountAmount.Value);
                        }
                        else if (coupon.ApplicableType == CouponServiceEnums.CouponApplicableTypeEnums.SpecificCategories)
                        {
                            var couponCategoryIds = coupon.CouponCategories?.Select(cc => cc.CategoryId).ToHashSet() ?? new HashSet<Guid>();
                            var orderMenuIds = order.OrderItems.Select(oi => oi.MenuId).ToList();
                            var applicableMenuIds = await _context.Set<CategoryDetail>()
                                .Where(cd => orderMenuIds.Contains(cd.MenuId) && couponCategoryIds.Contains(cd.CategoryId))
                                .Select(cd => cd.MenuId)
                                .Distinct()
                                .ToListAsync();
                            var applicableTotal = order.OrderItems.Where(oi => applicableMenuIds.Contains(oi.MenuId)).Sum(oi => oi.TotalPrice);
                            discountAmount = coupon.Type switch
                            {
                                CouponServiceEnums.CouponTypeEnums.Percentage => applicableTotal * (coupon.Value / 100m),
                                CouponServiceEnums.CouponTypeEnums.FixedAmount => Math.Min(coupon.Value, applicableTotal),
                                _ => 0
                            };
                            if (coupon.Type == CouponServiceEnums.CouponTypeEnums.Percentage && coupon.MaxDiscountAmount.HasValue)
                                discountAmount = Math.Min(discountAmount, coupon.MaxDiscountAmount.Value);
                        }

                        discountAmount = Math.Round(discountAmount, 2);

                        // Save coupon info on order
                        order.CouponId = coupon.Id;
                        order.CouponCode = coupon.Code;

                        // Increment usage
                        coupon.CurrentUsageCount += 1;
                        _context.Coupons.Update(coupon);

                        // Track user coupon usage
                        var existingUserCoupon = await _context.Set<UserCoupon>()
                            .FirstOrDefaultAsync(uc => uc.UserId == token.UserId && uc.CouponId == coupon.Id);

                        if (existingUserCoupon != null)
                        {
                            existingUserCoupon.UsageCount += 1;
                            existingUserCoupon.OrderId = order.Id;
                            existingUserCoupon.UsedAt = DateTime.UtcNow;
                        }
                        else
                        {
                            _context.Set<UserCoupon>().Add(new UserCoupon
                            {
                                Id = Guid.NewGuid(),
                                UserId = token.UserId,
                                CouponId = coupon.Id,
                                OrderId = order.Id,
                                UsedAt = DateTime.UtcNow,
                                UsageCount = 1
                            });
                        }
                    }
                }
            }

            order.DiscountAmount = discountAmount;
            order.TotalPrice = totalProductPrice + order.ShipmentPrice - discountAmount;

            await _unitOfWork.OrderRepository.AddAsync(order);
            await AddStatusHistory(order.Id, OrderStatusEnums.PaymentPending);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();

            var response = new PlaceOrderResponseDto
            {
                OrderId = order.Id,
                RequiresPayment = false,
                RequiresThreeDs = false
            };

            // Online ödeme ise otomatik ödeme başlat
            if (order.PaymentOptionId == (short)PaymentOptionEnums.CreditCard)
            {
                response.RequiresPayment = true;

                var user = await _userRepository.GetAsync(x => x.Id == token.UserId);
                var callbackUrl = $"{_callbackBaseUrl}/v1/payment/iyzico/callback";

                var paymentRequest = new IyzicoPaymentRequestDto
                {
                    OrderId = order.Id,
                    Amount = order.TotalPrice,
                    BuyerEmail = user?.Email ?? $"{token.UserId}@foodorder.app",
                    BuyerName = user?.FirstName ?? "Müşteri",
                    BuyerSurname = user?.LastName ?? "Kullanıcı",
                    BuyerPhone = user?.PhoneNumber ?? "+905000000000",
                    BuyerIp = "85.34.78.112",
                    BuyerId = token.UserId.ToString(),
                    DeliveryCity = "Istanbul",
                    DeliveryAddress = "Teslimat Adresi",
                    CallbackUrl = callbackUrl,
                    SaveCard = requestDto.SaveCard,
                    CardAlias = requestDto.CardAlias
                };

                // Kayıtlı kart mı yoksa yeni kart mı?
                if (!string.IsNullOrEmpty(requestDto.CardToken))
                {
                    paymentRequest.CardToken = requestDto.CardToken;
                    var cardUserKeyInfo = await _context.Set<UserExternalInfo>()
                        .FirstOrDefaultAsync(x => x.UserId == token.UserId && x.Provider == "iyzico" && x.Key == "CardUserKey");
                    paymentRequest.CardUserKey = cardUserKeyInfo?.Value;
                }
                else
                {
                    paymentRequest.CardHolderName = requestDto.CardHolderName;
                    paymentRequest.CardNumber = requestDto.CardNumber;
                    paymentRequest.ExpireMonth = requestDto.ExpireMonth;
                    paymentRequest.ExpireYear = requestDto.ExpireYear;
                    paymentRequest.Cvc = requestDto.Cvc;
                }

                var paymentResult = await _iyzicoAdapter.InitiatePayment(paymentRequest);
                if (!paymentResult.HasFailed && paymentResult.Data != null && paymentResult.Data.IsSuccess)
                {
                    // Payment kaydı oluştur (Pending)
                    await _paymentService.CreatePendingPayment(
                        order.Id, token.UserId, order.SellerId, order.TotalPrice,
                        order.PaymentOptionId, order.Id.ToString(),
                        requestDto.SaveCard ? requestDto.CardAlias : null);

                    response.RequiresThreeDs = paymentResult.Data.RequiresThreeDs;
                    response.ThreeDsHtmlContent = paymentResult.Data.ThreeDsHtmlContent;
                }
                else
                {
                    response.PaymentError = paymentResult.Data?.ErrorMessage ?? paymentResult.Messages?.FirstOrDefault()?.Description ?? "Ödeme başlatılamadı.";
                }
            }

            result.SetData(response);
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
                q => q.Include(o => o.OrderItems));

            if (order == null)
            {
                result.Fail("Sipariş bulunamadı.");
                return result;
            }

            // Admin veya kendi siparişi olmalı
            if (token.Role != UserRoleEnums.Admin && order.UserId != token.UserId)
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

    public async Task<ServiceCollectionResult<GetOrderResponseDto>> GetActiveOrders()
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

            var activeStatuses = new List<short>
            {
                (short)OrderStatusEnums.WaitingRestaurantApproval,
                (short)OrderStatusEnums.Preparing,
                (short)OrderStatusEnums.OnTheWay
            };

            var orders = await _unitOfWork.OrderRepository.GetListAsync(
                x => x.UserId == token.UserId && activeStatuses.Contains(x.StatusId),
                q => q.OrderByDescending(o => o.CreatedDate),
                q => q.Include(o => o.OrderItems),
                size: 5);

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
            var allowedStatuses = new List<short>
            {
                (short)OrderStatusEnums.WaitingRestaurantApproval,
                (short)OrderStatusEnums.Preparing,
                (short)OrderStatusEnums.OnTheWay,
                (short)OrderStatusEnums.Delivered,
                (short)OrderStatusEnums.CancelledByBuyer,
                (short)OrderStatusEnums.RejectedByRestaurant
            };

            var orders = await _unitOfWork.OrderRepository.GetListAsync(
                x => x.UserId == token.UserId && allowedStatuses.Contains(x.StatusId),
                q => q.OrderByDescending(o => o.CreatedDate),
                q => q.Include(o => o.OrderItems),
                page - 1,
                pageSize);

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

            var cancellableStatuses = new List<short>
            {
                (short)OrderStatusEnums.WaitingRestaurantApproval
            };

            if (!cancellableStatuses.Contains(order.StatusId))
            {
                result.Fail("Bu sipariş artık iptal edilemez.");
                return result;
            }

            order.StatusId = (short)OrderStatusEnums.CancelledByBuyer;
            order.CancellationReason = reason;
            _unitOfWork.OrderRepository.Update(order);
            await AddStatusHistory(order.Id, OrderStatusEnums.CancelledByBuyer, reason);
            await _unitOfWork.CompleteAsync();

            // Auto-refund on cancellation
            _ = Task.Run(async () =>
            {
                try
                {
                    await _paymentService.RefundOrderAsync(order.Id, "Müşteri tarafından iptal");
                }
                catch
                {
                    /* best effort */
                }
            });

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
            var isSeller = token.Role is UserRoleEnums.SellerAdmin or UserRoleEnums.SellerUser;
            if (isSeller)
            {
                if (token.RestaurantIds == null || !token.RestaurantIds.Contains(order.RestaurantId))
                {
                    result.Fail("Bu siparişi güncelleme yetkiniz yok.");
                    return result;
                }

                // Satıcı geçiş kuralları
                var sellerTransitions = new Dictionary<short, List<short>>
                {
                    {
                        (short)OrderStatusEnums.WaitingRestaurantApproval, new List<short>
                        {
                            (short)OrderStatusEnums.Preparing,
                            (short)OrderStatusEnums.RejectedByRestaurant
                        }
                    },
                    {
                        (short)OrderStatusEnums.Preparing, new List<short>
                        {
                            (short)OrderStatusEnums.OnTheWay // Fallback for restaurants without courier system
                        }
                    }
                };

                if (!sellerTransitions.TryGetValue(order.StatusId, out var allowedNext) || !allowedNext.Contains(statusId))
                {
                    result.Fail("Bu durum geçişi için yetkiniz yok.");
                    return result;
                }
            }

            order.StatusId = statusId;
            if (statusId == (short)OrderStatusEnums.RejectedByRestaurant)
                order.CancellationReason = "Restoran tarafından reddedildi.";
            _unitOfWork.OrderRepository.Update(order);
            await AddStatusHistory(order.Id, (OrderStatusEnums)statusId);
            await _unitOfWork.CompleteAsync();

            // Auto-trigger courier assignment when order moves to Preparing
            if (statusId == (short)OrderStatusEnums.Preparing)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _deliveryAssignmentService.CreateAssignment(order.Id);
                    }
                    catch
                    {
                        /* best effort - manual assignment still possible */
                    }
                });

            // Push notification + SignalR
            _ = Task.Run(async () =>
            {
                try
                {
                    var statusName = (OrderStatusEnums)statusId switch
                    {
                        OrderStatusEnums.Preparing => "Siparişiniz hazırlanıyor",
                        OrderStatusEnums.OnTheWay => "Siparişiniz yola çıktı",
                        OrderStatusEnums.Delivered => "Siparişiniz teslim edildi",
                        OrderStatusEnums.CourierAssigned => "Siparişinize kurye atandı",
                        OrderStatusEnums.CourierPickedUp => "Siparişiniz kurye tarafından teslim alındı",
                        OrderStatusEnums.RejectedByRestaurant => "Siparişiniz restoran tarafından reddedildi",
                        _ => "Sipariş durumu güncellendi"
                    };

                    await _notificationService.SendToUserAsync(order.UserId, "Sipariş Güncelleme", statusName,
                        new Dictionary<string, string> { { "orderId", order.Id.ToString() } });

                    await _realtimeNotifier.NotifyOrderStatusChanged(order.Id, statusId);

                    // Auto-refund on rejection
                    if (statusId == (short)OrderStatusEnums.RejectedByRestaurant)
                        await _paymentService.RefundOrderAsync(order.Id, "Restoran tarafından reddedildi");
                }
                catch
                {
                    /* best effort */
                }
            });

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
            var sellerVisibleStatuses = new List<short>
            {
                (short)OrderStatusEnums.WaitingRestaurantApproval,
                (short)OrderStatusEnums.RejectedByRestaurant,
                (short)OrderStatusEnums.Preparing,
                (short)OrderStatusEnums.OnTheWay,
                (short)OrderStatusEnums.Delivered
            };

            var orders = await _unitOfWork.OrderRepository.GetListAsync(
                x => x.RestaurantId == restaurantId
                     && sellerVisibleStatuses.Contains(x.StatusId)
                     && (statusId == null || x.StatusId == statusId),
                q => q.OrderByDescending(o => o.CreatedDate),
                q => q.Include(o => o.OrderItems),
                page - 1,
                pageSize);

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
            var order = await _unitOfWork.OrderRepository.GetAsync(x => x.Id == orderId && x.UserId == token!.UserId);

            if (order == null)
            {
                result.Fail("Sipariş bulunamadı.");
                return result;
            }

            if (order.PaymentStatusId != (short)PaymentStatusEnums.Pending)
            {
                result.Fail("Bu sipariş için ödeme zaten işlenmiş.");
                return result;
            }

            if (order.PaymentOptionId != (short)PaymentOptionEnums.CreditCard)
            {
                result.Fail("Bu sipariş kart ödemesi gerektirmiyor.");
                return result;
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
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<AdminGetOrderResponseDto>> GetAllOrdersForAdmin(int page = 1, int pageSize = 20, short? statusId = null)
    {
        var result = new ServiceCollectionResult<AdminGetOrderResponseDto>();
        try
        {
            pageSize = Math.Min(pageSize, 100);
            var orders = await _unitOfWork.OrderRepository.GetListAsync(
                x => statusId == null || x.StatusId == statusId,
                q => q.OrderByDescending(o => o.CreatedDate),
                q => q.Include(o => o.OrderItems)
                    .Include(o => o.Payments)
                    .Include(o => o.StatusHistory),
                page - 1,
                pageSize);

            var restaurantIds = orders.Items.Select(o => o.RestaurantId).Distinct().ToList();
            var restaurants = await _restaurantRepository.GetListAsync(x => restaurantIds.Contains(x.Id), size: restaurantIds.Count + 1);
            var restaurantDict = restaurants.Items.ToDictionary(r => r.Id, r => r.Name);

            var userIds = orders.Items.Select(o => o.UserId).Distinct().ToList();
            var users = await _userRepository.GetListAsync(x => userIds.Contains(x.Id), size: userIds.Count + 1);
            var userDict = users.Items.ToDictionary(u => u.Id);

            var dtos = orders.Items.Select(o => MapToAdminDto(o,
                restaurantDict.GetValueOrDefault(o.RestaurantId, "Bilinmiyor"),
                userDict.GetValueOrDefault(o.UserId))).ToList();
            result.SetData(orders.Count, dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<AdminGetOrderResponseDto>> GetOrderDetailForAdmin(Guid orderId)
    {
        var result = new ServiceObjectResult<AdminGetOrderResponseDto>();
        try
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(
                x => x.Id == orderId,
                q => q.Include(o => o.OrderItems)
                    .Include(o => o.Payments)
                    .Include(o => o.StatusHistory));

            if (order == null)
            {
                result.Fail("Sipariş bulunamadı.");
                return result;
            }

            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == order.RestaurantId);
            var user = await _userRepository.GetAsync(x => x.Id == order.UserId);
            var address = await _unitOfWork.OrderRepository.GetAsync(x => x.Id == orderId) != null
                ? await _context.Set<Address>().FirstOrDefaultAsync(a => a.Id == order.DeliveryAddressId)
                : null;

            var dto = MapToAdminDto(order, restaurant?.Name ?? "Bilinmiyor", user);
            if (address != null)
                dto.DeliveryAddress = $"{address.AddressLine1} {address.AddressLine2}".Trim();

            result.SetData(dto);
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
            StatusName = ((OrderStatusEnums)order.StatusId).ToString(),
            PaymentStatusId = order.PaymentStatusId,
            PaymentOptionId = order.PaymentOptionId,
            TotalProductPrice = order.TotalProductPrice,
            ShipmentPrice = order.ShipmentPrice,
            DiscountAmount = order.DiscountAmount,
            TotalPrice = order.TotalPrice,
            CouponId = order.CouponId,
            CouponCode = order.CouponCode,
            Notes = order.Notes,
            CancellationReason = order.CancellationReason,
            CreatedDate = order.CreatedDate,
            Items = order.OrderItems?.Select(oi =>
            {
                var snap = TryGetSnapshot(oi.ItemSnapshotJson);
                return new GetOrderItemResponseDto
                {
                    Id = oi.Id,
                    MenuId = oi.MenuId,
                    MenuName = snap.menuName,
                    Description = snap.description,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice,
                    Values = snap.values
                };
            }).ToList() ?? new List<GetOrderItemResponseDto>()
        };
    }

    private static AdminGetOrderResponseDto MapToAdminDto(Order order, string restaurantName, User? user)
    {
        return new AdminGetOrderResponseDto
        {
            Id = order.Id,
            UserId = order.UserId,
            CustomerName = user != null ? $"{user.FirstName} {user.LastName}" : null,
            CustomerEmail = user?.Email,
            CustomerPhone = user?.PhoneNumber,
            RestaurantId = order.RestaurantId,
            RestaurantName = restaurantName,
            SellerId = order.SellerId,
            StatusId = order.StatusId,
            StatusName = ((OrderStatusEnums)order.StatusId).ToString(),
            PaymentStatusId = order.PaymentStatusId,
            PaymentStatusName = ((PaymentStatusEnums)order.PaymentStatusId).ToString(),
            PaymentOptionId = order.PaymentOptionId,
            PaymentOptionName = Enum.IsDefined(typeof(PaymentOptionEnums), (short)order.PaymentOptionId)
                ? ((PaymentOptionEnums)(short)order.PaymentOptionId).ToString()
                : order.PaymentOptionId.ToString(),
            TotalProductPrice = order.TotalProductPrice,
            ShipmentPrice = order.ShipmentPrice,
            DiscountAmount = order.DiscountAmount,
            TotalPrice = order.TotalPrice,
            CouponId = order.CouponId,
            CouponCode = order.CouponCode,
            Notes = order.Notes,
            CancellationReason = order.CancellationReason,
            CreatedDate = order.CreatedDate,
            Items = order.OrderItems?.Select(oi =>
            {
                var snap = TryGetSnapshot(oi.ItemSnapshotJson);
                return new AdminGetOrderItemDto
                {
                    Id = oi.Id,
                    MenuId = oi.MenuId,
                    MenuName = snap.menuName,
                    Description = snap.description,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice,
                    Values = snap.values
                };
            }).ToList() ?? new List<AdminGetOrderItemDto>(),
            Payments = order.Payments?.Select(p => new AdminPaymentDto
            {
                Id = p.Id,
                Amount = p.Amount,
                SellerPayoutAmount = p.SellerPayoutAmount,
                CommissionAmount = p.CommissionAmount,
                StatusId = p.StatusId,
                StatusName = ((PaymentStatusEnums)p.StatusId).ToString(),
                ProviderPaymentId = p.ProviderPaymentId,
                ProviderConversationId = p.ProviderConversationId,
                ProviderTransactionId = p.ProviderTransactionId,
                PaymentOptionId = p.PaymentOptionId,
                CardLastFourDigits = p.CardLastFourDigits,
                CardType = p.CardType,
                CardAssociation = p.CardAssociation,
                ErrorMessage = p.ErrorMessage,
                ErrorCode = p.ErrorCode,
                CompletedAt = p.CompletedAt,
                FailedAt = p.FailedAt,
                CreatedDate = p.CreatedDate
            }).OrderByDescending(p => p.CreatedDate).ToList() ?? new List<AdminPaymentDto>(),
            StatusHistory = order.StatusHistory?.Select(sh => new AdminStatusHistoryDto
            {
                Id = sh.Id,
                StatusId = sh.StatusId,
                StatusName = ((OrderStatusEnums)sh.StatusId).ToString(),
                Note = sh.Note,
                OccurredAt = sh.OccurredAt
            }).OrderByDescending(sh => sh.OccurredAt).ToList() ?? new List<AdminStatusHistoryDto>()
        };
    }

    private static (string menuName, string? description, List<OrderItemValueDto> values) TryGetSnapshot(string? snapshotJson)
    {
        if (string.IsNullOrEmpty(snapshotJson)) return (string.Empty, null, new List<OrderItemValueDto>());
        try
        {
            var snapshot = JsonConvert.DeserializeObject<dynamic>(snapshotJson);
            var values = new List<OrderItemValueDto>();
            if (snapshot?.values != null)
                foreach (var v in snapshot.values)
                {
                    var dto = new OrderItemValueDto
                    {
                        OptionName = v.optionName?.ToString() ?? "",
                        ValueName = v.valueName?.ToString() ?? "",
                        Quantity = (int)(v.quantity ?? 1),
                        UnitPrice = (decimal)(v.unitPrice ?? 0),
                        TotalPrice = (decimal)(v.totalPrice ?? 0),
                        Options = new List<OrderItemValueOptionDto>()
                    };
                    if (v.options != null)
                        foreach (var o in v.options)
                            dto.Options.Add(new OrderItemValueOptionDto
                            {
                                OptionName = o.optionName?.ToString() ?? "",
                                ValueName = o.valueName?.ToString() ?? "",
                                Quantity = (int)(o.quantity ?? 1),
                                UnitPrice = (decimal)(o.unitPrice ?? 0),
                                TotalPrice = (decimal)(o.totalPrice ?? 0)
                            });

                    values.Add(dto);
                }

            return (
                snapshot?.menuName?.ToString() ?? string.Empty,
                snapshot?.description?.ToString(),
                values
            );
        }
        catch
        {
            return (string.Empty, null, new List<OrderItemValueDto>());
        }
    }

    private async Task AddStatusHistory(Guid orderId, OrderStatusEnums status, string? note = null)
    {
        var history = new OrderStatusHistory
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            StatusId = (short)status,
            Note = note,
            OccurredAt = DateTime.UtcNow
        };
        await _unitOfWork.OrderStatusHistoryRepository.AddAsync(history);
    }

    public async Task<ServiceCollectionResult<AdminGetOrderResponseDto>> GetOverdueOrdersForAdmin(int page = 1, int pageSize = 20)
    {
        var result = new ServiceCollectionResult<AdminGetOrderResponseDto>();
        try
        {
            var now = DateTime.UtcNow;

            var overdueOrders = await _context.Orders
                .Where(o => o.StatusId == (short)OrderStatusEnums.OnTheWay
                            && o.DeletedDate == null)
                .Join(
                    _context.Set<Restaurant>(),
                    o => o.RestaurantId,
                    r => r.Id,
                    (o, r) => new { Order = o, Restaurant = r })
                .Where(x => x.Order.CreatedDate.AddMinutes(x.Restaurant.MaxDeliveryTime) < now)
                .OrderByDescending(x => x.Order.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.Orders
                .Where(o => o.StatusId == (short)OrderStatusEnums.OnTheWay
                            && o.DeletedDate == null)
                .Join(
                    _context.Set<Restaurant>(),
                    o => o.RestaurantId,
                    r => r.Id,
                    (o, r) => new { Order = o, Restaurant = r })
                .Where(x => x.Order.CreatedDate.AddMinutes(x.Restaurant.MaxDeliveryTime) < now)
                .CountAsync();

            var userIds = overdueOrders.Select(x => x.Order.UserId).Distinct().ToList();
            var users = await _context.Set<User>()
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var dtos = overdueOrders.Select(x =>
            {
                var user = users.GetValueOrDefault(x.Order.UserId);
                return new AdminGetOrderResponseDto
                {
                    Id = x.Order.Id,
                    UserId = x.Order.UserId,
                    CustomerName = user != null ? $"{user.FirstName} {user.LastName}" : null,
                    CustomerEmail = user?.Email,
                    CustomerPhone = user?.PhoneNumber,
                    RestaurantId = x.Order.RestaurantId,
                    RestaurantName = x.Restaurant.Name,
                    SellerId = x.Order.SellerId,
                    StatusId = x.Order.StatusId,
                    StatusName = ((OrderStatusEnums)x.Order.StatusId).ToString(),
                    PaymentStatusId = x.Order.PaymentStatusId,
                    PaymentStatusName = ((PaymentStatusEnums)x.Order.PaymentStatusId).ToString(),
                    PaymentOptionId = x.Order.PaymentOptionId,
                    PaymentOptionName = ((PaymentOptionEnums)x.Order.PaymentOptionId).ToString(),
                    TotalProductPrice = x.Order.TotalProductPrice,
                    ShipmentPrice = x.Order.ShipmentPrice,
                    DiscountAmount = x.Order.DiscountAmount,
                    TotalPrice = x.Order.TotalPrice,
                    CouponId = x.Order.CouponId,
                    CouponCode = x.Order.CouponCode,
                    Notes = x.Order.Notes,
                    CreatedDate = x.Order.CreatedDate
                };
            }).ToList();

            result.SetData(totalCount, dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<ReorderResponseDto>> ReorderAsync(Guid orderId)
    {
        var result = new ServiceObjectResult<ReorderResponseDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Kimlik doğrulama hatası.");
                return result;
            }

            var order = await _unitOfWork.OrderRepository.GetAsync(
                o => o.Id == orderId && o.UserId == token.UserId,
                q => q.Include(o => o.OrderItems));

            if (order == null)
            {
                result.Fail("Sipariş bulunamadı.");
                return result;
            }

            if (order.StatusId != (short)OrderStatusEnums.Delivered)
            {
                result.Fail("Sadece teslim edilmiş siparişler tekrar sipariş edilebilir.");
                return result;
            }

            var restaurant = await _restaurantRepository.GetAsync(r => r.Id == order.RestaurantId);
            if (restaurant == null || !restaurant.IsActive)
            {
                result.Fail("Restoran artık aktif değil.");
                return result;
            }

            var response = new ReorderResponseDto { Success = true };
            var basketItems = new List<UpdateBasketDto.UpdateBasketItemDto>();

            foreach (var item in order.OrderItems)
            {
                var menu = await _menuRepository.GetAsync(m => m.Id == item.MenuId);
                if (menu == null || menu.DeletedDate != null)
                {
                    response.Warnings.Add(new ReorderWarningDto
                    {
                        MenuId = item.MenuId,
                        MenuName = item.MenuId.ToString(),
                        WarningType = "Unavailable",
                        Message = "Bu ürün artık mevcut değil."
                    });
                    continue;
                }

                if (menu.Price != item.UnitPrice)
                    response.Warnings.Add(new ReorderWarningDto
                    {
                        MenuId = item.MenuId,
                        MenuName = menu.Name,
                        WarningType = "PriceChanged",
                        Message = $"Fiyat değişti: {item.UnitPrice:C2} → {menu.Price:C2}"
                    });

                basketItems.Add(new UpdateBasketDto.UpdateBasketItemDto
                {
                    MenuId = item.MenuId,
                    Quantity = item.Quantity,
                    BasketItemValues = new List<UpdateBasketDto.UpdateBasketItemDto.UpdateBasketItemValueDto>()
                });
            }

            if (basketItems.Count == 0)
            {
                result.Fail("Siparişin hiçbir ürünü artık mevcut değil.");
                return result;
            }

            // Build basket update DTO and add to Redis basket
            var basketDto = new UpdateBasketDto
            {
                RestaurantId = order.RestaurantId,
                SellerId = order.SellerId,
                UserShippingAddressId = order.DeliveryAddressId,
                UserInvoiceAddressId = order.InvoiceAddressId ?? Guid.Empty,
                PaymentOptionId = order.PaymentOptionId,
                BasketItems = basketItems
            };

            var updateResult = await _basketService.UpdateBasketForRedis(basketDto);
            if (!updateResult.Data)
            {
                result.Fail("Sepet güncellenirken bir hata oluştu.");
                return result;
            }

            result.SetData(response);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}