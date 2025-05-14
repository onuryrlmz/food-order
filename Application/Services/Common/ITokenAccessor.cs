using Base.Entities;

namespace Application.Services.Common;

public interface ITokenAccessor
{
    TokenDto? GetToken();
}