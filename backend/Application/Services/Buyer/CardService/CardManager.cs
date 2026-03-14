using Application.Services.Common.TokenService;
using Domain.Entities.Common;
using Domain.Service;
using Infrastructure.Adapters.IyzicoServiceAdapter;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.IRepositories.Common;

namespace Application.Services.Buyer.CardService;

public class CardManager : ICardService
{
    private const string ProviderIyzico = "iyzico";
    private const string KeyCardUserKey = "CardUserKey";

    private readonly IIyzicoServiceAdapter _iyzicoAdapter;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly IUserRepository _userRepository;
    private readonly BaseDbContext _context;

    public CardManager(
        IIyzicoServiceAdapter iyzicoAdapter,
        ITokenAccessor tokenAccessor,
        IUserRepository userRepository,
        BaseDbContext context)
    {
        _iyzicoAdapter = iyzicoAdapter;
        _tokenAccessor = tokenAccessor;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<ServiceObjectResult<CardStorageResultDto>> CreateCard(CreateCardRequestDto requestDto)
    {
        var result = new ServiceObjectResult<CardStorageResultDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Kimlik doğrulama hatası."); return result; }

            var user = await _userRepository.GetAsync(x => x.Id == token.UserId);
            if (user == null) { result.Fail("Kullanıcı bulunamadı."); return result; }

            var cardUserKey = await GetExternalInfo(token.UserId, ProviderIyzico, KeyCardUserKey);

            var cardResult = _iyzicoAdapter.CreateCard(
                externalId: user.Id.ToString(),
                email: user.Email,
                cardUserKey: cardUserKey,
                cardAlias: requestDto.CardAlias,
                cardNumber: requestDto.CardNumber,
                expireYear: requestDto.ExpireYear,
                expireMonth: requestDto.ExpireMonth,
                cardHolderName: requestDto.CardHolderName
            );

            if (cardResult.HasFailed)
            {
                result.Fail(cardResult.Messages);
                return result;
            }

            if (string.IsNullOrEmpty(cardUserKey))
            {
                await SetExternalInfo(token.UserId, ProviderIyzico, KeyCardUserKey, cardResult.Data.CardUserKey);
            }

            // Kart alias bilgisini kaydet
            var cardInfoJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                cardToken = cardResult.Data.CardToken,
                alias = requestDto.CardAlias ?? "Kartım",
                binNumber = cardResult.Data.BinNumber,
                lastFourDigits = cardResult.Data.LastFourDigits,
                cardType = cardResult.Data.CardType,
                cardAssociation = cardResult.Data.CardAssociation
            });
            await SetExternalInfo(token.UserId, ProviderIyzico, $"SavedCard:{cardResult.Data.CardToken}", cardInfoJson);

            result.SetData(cardResult.Data);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<List<CardDetailDto>>> GetCards()
    {
        var result = new ServiceObjectResult<List<CardDetailDto>>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Kimlik doğrulama hatası."); return result; }

            var cardUserKey = await GetExternalInfo(token.UserId, ProviderIyzico, KeyCardUserKey);

            if (string.IsNullOrEmpty(cardUserKey))
            {
                result.SetData(new List<CardDetailDto>());
                return result;
            }

            var cardResult = _iyzicoAdapter.GetCards(cardUserKey);
            if (cardResult.HasFailed)
            {
                result.SetData(new List<CardDetailDto>());
                return result;
            }

            // UserExternalInfo'dan kart alias bilgilerini al ve eşleştir
            var savedCardInfos = await _context.UserExternalInfos
                .Where(x => x.UserId == token.UserId && x.Provider == ProviderIyzico && x.Key.StartsWith("SavedCard:"))
                .ToListAsync();
            var aliasMap = new Dictionary<string, string>();
            foreach (var sci in savedCardInfos)
            {
                try
                {
                    var parsed = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(sci.Value);
                    if (parsed.TryGetProperty("cardToken", out var ct) && parsed.TryGetProperty("alias", out var al))
                        aliasMap[ct.GetString()!] = al.GetString() ?? "Kartım";
                }
                catch { }
            }
            foreach (var card in cardResult.Data)
            {
                if (aliasMap.TryGetValue(card.CardToken, out var alias))
                    card.CardAlias = alias;
            }

            result.SetData(cardResult.Data);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> DeleteCard(string cardToken)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            if (token == null) { result.Fail("Kimlik doğrulama hatası."); return result; }

            var cardUserKey = await GetExternalInfo(token.UserId, ProviderIyzico, KeyCardUserKey);

            if (string.IsNullOrEmpty(cardUserKey))
            {
                result.Fail("Kayıtlı kart bulunamadı.");
                return result;
            }

            var deleteResult = _iyzicoAdapter.DeleteCard(cardUserKey, cardToken);
            if (deleteResult.HasFailed)
            {
                result.Fail(deleteResult.Messages);
                return result;
            }

            // UserExternalInfo'dan kart bilgisini sil
            var savedCardInfo = await _context.UserExternalInfos
                .FirstOrDefaultAsync(x => x.UserId == token.UserId && x.Provider == ProviderIyzico && x.Key == $"SavedCard:{cardToken}");
            if (savedCardInfo != null)
            {
                _context.UserExternalInfos.Remove(savedCardInfo);
                await _context.SaveChangesAsync();
            }

            result.SetData(true);
        }
        catch (Exception e) { result.Fail(e); }
        return result;
    }

    private async Task<string?> GetExternalInfo(Guid userId, string provider, string key)
    {
        var info = await _context.UserExternalInfos
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Provider == provider && x.Key == key);
        return info?.Value;
    }

    private async Task SetExternalInfo(Guid userId, string provider, string key, string value)
    {
        var info = await _context.UserExternalInfos
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Provider == provider && x.Key == key);
        if (info != null)
        {
            info.Value = value;
            _context.UserExternalInfos.Update(info);
        }
        else
        {
            _context.UserExternalInfos.Add(new UserExternalInfo
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Provider = provider,
                Key = key,
                Value = value
            });
        }
        await _context.SaveChangesAsync();
    }
}
