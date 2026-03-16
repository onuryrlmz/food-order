using Domain.Dto.Common;
using Domain.Service;

namespace Application.Services.Common.UserService;

public abstract class IUserService
{
    public abstract Task<ServiceObjectResult<bool>> Register(UserRegisterDto requestDto);
    public abstract Task<ServiceObjectResult<TokenPairDto>> Login(UserLoginDto requestDto);
    public abstract Task<ServiceObjectResult<GetUserProfileDto>> GetProfile();
    public abstract Task<ServiceObjectResult<bool>> UpdateProfile(UpdateUserProfileDto requestDto);
    public abstract Task<ServiceObjectResult<bool>> ChangePassword(ChangePasswordDto requestDto);
    public abstract Task<ServiceCollectionResult<GetUserListResponseDto>> GetUserList(int page = 1, int pageSize = 20, short? roleId = null);
}
