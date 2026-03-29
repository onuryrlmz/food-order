using Application.Services.Common.TokenService;
using Base.Enums;
using Domain.Dto.Buyer.Tip;
using Domain.Entities.Buyer;
using Domain.Service;
using Persistence.IRepositories;

namespace Application.Services.Buyer.TipService;

public class TipManager : ITipService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenAccessor _tokenAccessor;

    public TipManager(IUnitOfWork unitOfWork, ITokenAccessor tokenAccessor)
    {
        _unitOfWork = unitOfWork;
        _tokenAccessor = tokenAccessor;
    }

    public async Task<ServiceObjectResult<TipDto>> AddTip(AddTipRequestDto requestDto)
    {
        var result = new ServiceObjectResult<TipDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            var order = await _unitOfWork.OrderRepository.GetAsync(
                x => x.Id == requestDto.OrderId && x.UserId == token.UserId);
            if (order == null)
            {
                result.Fail("Order not found");
                return result;
            }

            // Tips allowed from WaitingRestaurantApproval(4) through CourierPickedUp(10)
            if (order.StatusId < (short)OrderStatusEnums.WaitingRestaurantApproval ||
                order.StatusId > (short)OrderStatusEnums.CourierPickedUp)
            {
                result.Fail("Tip cannot be added for this order status");
                return result;
            }

            // Check no existing tip
            var existingTip = await _unitOfWork.TipRepository.GetAsync(
                x => x.OrderId == requestDto.OrderId && x.DeletedDate == null);
            if (existingTip != null)
            {
                result.Fail("A tip has already been added to this order");
                return result;
            }

            decimal tipAmount;
            if (requestDto.PresetPercentage.HasValue)
            {
                // Preset tip percentages — kept as constants; change here AND in GetTipOptions if updated
                var validPercentages = new short[] { 10, 15, 20 };
                if (!validPercentages.Contains(requestDto.PresetPercentage.Value))
                {
                    result.Fail("Invalid tip percentage. Valid values: 10, 15, 20");
                    return result;
                }
                tipAmount = order.TotalPrice * requestDto.PresetPercentage.Value / 100m;
                tipAmount = Math.Round(tipAmount, 2);
            }
            else if (requestDto.CustomAmount.HasValue)
            {
                if (requestDto.CustomAmount.Value <= 0)
                {
                    result.Fail("Tip amount must be greater than zero");
                    return result;
                }
                tipAmount = requestDto.CustomAmount.Value;
            }
            else
            {
                result.Fail("Either custom amount or preset percentage must be provided");
                return result;
            }

            var isPreDelivery = order.StatusId < (short)OrderStatusEnums.Delivered;

            var tip = new Tip
            {
                Id = Guid.NewGuid(),
                OrderId = requestDto.OrderId,
                UserId = token.UserId,
                CourierId = order.CourierId,
                Amount = tipAmount,
                PresetPercentage = requestDto.PresetPercentage,
                IsPreDelivery = isPreDelivery,
                IsSettled = false
            };

            await _unitOfWork.TipRepository.AddAsync(tip);

            // If courier is assigned and earning exists, update tip amount
            if (order.CourierId.HasValue)
            {
                var earning = await _unitOfWork.CourierEarningRepository.GetAsync(
                    x => x.OrderId == order.Id && x.DeletedDate == null, enableTracking: true);
                if (earning != null)
                {
                    earning.TipAmount = tipAmount;
                    earning.TotalEarning = earning.DeliveryFee + (earning.BonusAmount ?? 0) + tipAmount;
                    await _unitOfWork.CourierEarningRepository.UpdateAsync(earning);
                    tip.IsSettled = true;
                }
            }

            await _unitOfWork.CompleteAsync();

            result.SetData(new TipDto
            {
                Id = tip.Id,
                OrderId = tip.OrderId,
                Amount = tip.Amount,
                PresetPercentage = tip.PresetPercentage,
                IsPreDelivery = tip.IsPreDelivery,
                CreatedDate = tip.CreatedDate
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<TipOptionsDto>> GetTipOptions(Guid orderId)
    {
        var result = new ServiceObjectResult<TipOptionsDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            var order = await _unitOfWork.OrderRepository.GetAsync(
                x => x.Id == orderId && x.UserId == token.UserId);
            if (order == null)
            {
                result.Fail("Order not found");
                return result;
            }

            // Preset tip percentages — kept as constants; change here AND in AddTip if updated
            var presetPercentages = new List<short> { 10, 15, 20 };
            var presetAmounts = presetPercentages
                .Select(p => Math.Round(order.TotalPrice * p / 100m, 2))
                .ToList();

            result.SetData(new TipOptionsDto
            {
                PresetPercentages = presetPercentages,
                OrderTotal = order.TotalPrice,
                PresetAmounts = presetAmounts
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<TipDto>> GetTipByOrder(Guid orderId)
    {
        var result = new ServiceObjectResult<TipDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null)
            {
                result.Fail("Unauthorized");
                return result;
            }

            var tip = await _unitOfWork.TipRepository.GetAsync(
                x => x.OrderId == orderId && x.UserId == token.UserId && x.DeletedDate == null);
            if (tip == null)
            {
                result.Fail("No tip found for this order");
                return result;
            }

            result.SetData(new TipDto
            {
                Id = tip.Id,
                OrderId = tip.OrderId,
                Amount = tip.Amount,
                PresetPercentage = tip.PresetPercentage,
                IsPreDelivery = tip.IsPreDelivery,
                CreatedDate = tip.CreatedDate
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}
