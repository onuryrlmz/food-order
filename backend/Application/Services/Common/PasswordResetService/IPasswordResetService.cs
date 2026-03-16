using Domain.Service;

namespace Application.Services.Common.PasswordResetService;

public interface IPasswordResetService
{
    Task<ServiceObjectResult<bool>> SendResetCodeAsync(string emailOrPhone);
    Task<ServiceObjectResult<bool>> VerifyCodeAsync(string emailOrPhone, string code);
    Task<ServiceObjectResult<bool>> ResetPasswordAsync(string emailOrPhone, string code, string newPassword);
}
