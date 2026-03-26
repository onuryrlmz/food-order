using System.Security.Cryptography;
using Base.Enums;
using Domain.Entities.Common;
using Domain.Service;
using Persistence.IRepositories.Common;

namespace Application.Services.Common.PasswordResetService;

public class PasswordResetManager : IPasswordResetService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;

    private const int CodeExpirationMinutes = 10;
    private const int MaxCodesPerWindow = 3;
    private const int RateLimitWindowMinutes = 10;
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 30;

    public PasswordResetManager(
        IUserRepository userRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository)
    {
        _userRepository = userRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
    }

    public async Task<ServiceObjectResult<bool>> SendResetCodeAsync(string emailOrPhone)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var user = await FindUserByEmailOrPhoneAsync(emailOrPhone);
            if (user == null)
            {
                // Don't reveal whether user exists
                response.SetData(true);
                response.AddSuccessMessage("Eğer bu bilgilerle kayıtlı bir hesap varsa, doğrulama kodu gönderildi.");
                return response;
            }

            // Rate limit check: max codes per window
            var windowStart = DateTime.UtcNow.AddMinutes(-RateLimitWindowMinutes);
            var recentCodes = await _passwordResetTokenRepository.GetListAsync(x => x.UserId == user.Id && x.CreatedDate >= windowStart);

            if (recentCodes.Items.Count >= MaxCodesPerWindow)
            {
                response.Fail("Çok fazla deneme yaptınız. Lütfen biraz bekleyin.");
                return response;
            }

            // Determine method
            var isEmail = emailOrPhone.Contains('@');
            var method = isEmail
                ? (short)PasswordResetMethodEnums.Email
                : (short)PasswordResetMethodEnums.Sms;

            // Generate 6-digit code
            var code = GenerateSixDigitCode();

            var resetToken = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Code = code,
                Method = method,
                ExpiresAt = DateTime.UtcNow.AddMinutes(CodeExpirationMinutes)
            };

            await _passwordResetTokenRepository.AddAsync(resetToken);

            // TODO: Send via email or SMS service
            // if (isEmail) await _emailService.SendResetCodeAsync(user.Email, code);
            // else await _smsService.SendResetCodeAsync(user.PhoneNumber, code);

            response.SetData(true);
            response.AddSuccessMessage("Doğrulama kodu gönderildi.");
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> VerifyCodeAsync(string emailOrPhone, string code)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var user = await FindUserByEmailOrPhoneAsync(emailOrPhone);
            if (user == null)
            {
                response.Fail("Geçersiz doğrulama kodu.");
                return response;
            }

            var token = await GetValidTokenAsync(user.Id, code);
            if (token == null)
            {
                response.Fail("Geçersiz veya süresi dolmuş doğrulama kodu.");
                return response;
            }

            response.SetData(true);
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public async Task<ServiceObjectResult<bool>> ResetPasswordAsync(string emailOrPhone, string code, string newPassword)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var user = await FindUserByEmailOrPhoneAsync(emailOrPhone);
            if (user == null)
            {
                response.Fail("Geçersiz doğrulama kodu.");
                return response;
            }

            var token = await GetValidTokenAsync(user.Id, code);
            if (token == null)
            {
                response.Fail("Geçersiz veya süresi dolmuş doğrulama kodu.");
                return response;
            }

            // Update password
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword, 12);
            await _userRepository.UpdateAsync(user);

            // Mark code as used
            token.UsedAt = DateTime.UtcNow;
            await _passwordResetTokenRepository.UpdateAsync(token);

            response.SetData(true);
            response.AddSuccessMessage("Şifreniz başarıyla değiştirildi.");
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    private async Task<User?> FindUserByEmailOrPhoneAsync(string emailOrPhone)
    {
        var isEmail = emailOrPhone.Contains('@');
        if (isEmail)
            return await _userRepository.GetAsync(x =>
                    x.Email == emailOrPhone.ToLowerInvariant() &&
                    x.UserStatusId == (short)UserStatusEnums.Active,
                enableTracking: true);

        return await _userRepository.GetAsync(x =>
                x.PhoneNumber == emailOrPhone &&
                x.UserStatusId == (short)UserStatusEnums.Active,
            enableTracking: true);
    }

    private async Task<PasswordResetToken?> GetValidTokenAsync(Guid userId, string code)
    {
        var token = await _passwordResetTokenRepository.GetAsync(
            x => x.UserId == userId &&
                 x.Code == code &&
                 !x.UsedAt.HasValue &&
                 x.ExpiresAt > DateTime.UtcNow,
            enableTracking: true);

        if (token == null)
            return null;

        // Check brute force lockout
        if (token.FailedAttempts >= MaxFailedAttempts) return null;

        return token;
    }

    private static string GenerateSixDigitCode()
    {
        var code = RandomNumberGenerator.GetInt32(100000, 999999);
        return code.ToString();
    }
}