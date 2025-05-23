using Domain.Dto.Common;
using Domain.Service;

namespace Application.Services.Common;

public abstract class IUserService
{
    public abstract Task<ServiceObjectResult<bool>> Register(UserRegisterDto requestDto);
    public abstract Task<ServiceObjectResult<bool>> Login(UserLoginDto requestDto);
}