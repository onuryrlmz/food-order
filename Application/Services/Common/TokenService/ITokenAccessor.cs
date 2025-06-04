using Base.Entities;

namespace Application.Services.Common.TokenService;

public interface ITokenAccessor
{
    TokenDto? GetToken();
}