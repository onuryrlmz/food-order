using Domain.Service;
using Infrastructure.Adapters.IyzicoServiceAdapter;

namespace Application.Services.Buyer.CardService;

public interface ICardService
{
    Task<ServiceObjectResult<CardStorageResultDto>> CreateCard(CreateCardRequestDto requestDto);
    Task<ServiceObjectResult<List<CardDetailDto>>> GetCards();
    Task<ServiceObjectResult<bool>> DeleteCard(string cardToken);
}

public class CreateCardRequestDto
{
    public string CardAlias { get; set; }
    public string CardNumber { get; set; }
    public string ExpireYear { get; set; }
    public string ExpireMonth { get; set; }
    public string CardHolderName { get; set; }
}