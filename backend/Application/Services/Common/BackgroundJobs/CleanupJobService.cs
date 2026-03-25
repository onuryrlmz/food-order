using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;

namespace Application.Services.Common.BackgroundJobs;

public class CleanupJobService : ICleanupJobService
{
    private readonly BaseDbContext _context;
    private readonly ILogger<CleanupJobService> _logger;

    public CleanupJobService(BaseDbContext context, ILogger<CleanupJobService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CleanupExpiredResetTokens()
    {
        try
        {
            var expired = await _context.Set<PasswordResetToken>()
                .Where(t => t.ExpiresAt < DateTime.UtcNow && t.UsedAt == null)
                .ToListAsync();

            _context.Set<PasswordResetToken>().RemoveRange(expired);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cleaned up {Count} expired reset tokens", expired.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired reset tokens");
        }
    }

    public async Task CleanupExpiredRefreshTokens()
    {
        try
        {
            var expired = await _context.Set<RefreshToken>()
                .Where(t => t.ExpiresAt < DateTime.UtcNow && t.RevokedAt != null)
                .ToListAsync();

            _context.Set<RefreshToken>().RemoveRange(expired);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cleaned up {Count} expired refresh tokens", expired.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired refresh tokens");
        }
    }
}