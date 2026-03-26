using Domain.Dto.Buyer.Tip;
using Domain.Service;

namespace Application.Services.Buyer.TipService;

public interface ITipService
{
    Task<ServiceObjectResult<TipDto>> AddTip(AddTipRequestDto requestDto);
    Task<ServiceObjectResult<TipOptionsDto>> GetTipOptions(Guid orderId);
    Task<ServiceObjectResult<TipDto>> GetTipByOrder(Guid orderId);
}
