using AngularNetBase.Identity.Entities;
using AngularNetBase.Identity.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AngularNetBase.Identity.Services;

public class PushTokenService
{
    private readonly IdentityContext _db;

    public PushTokenService(IdentityContext db)
    {
        _db = db;
    }

    public async Task RegisterAsync(Guid userId, string token, string platform)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Token is required.");

        var normalizedToken = token.Trim();
        var normalizedPlatform = string.IsNullOrWhiteSpace(platform) ? "unknown" : platform.Trim().ToLowerInvariant();
        var now = DateTime.UtcNow;

        var existing = await _db.PushDeviceTokens.FirstOrDefaultAsync(x => x.Token == normalizedToken);
        if (existing is null)
        {
            _db.PushDeviceTokens.Add(new PushDeviceToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = normalizedToken,
                Platform = normalizedPlatform,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            });
        }
        else
        {
            existing.UserId = userId;
            existing.Platform = normalizedPlatform;
            existing.IsActive = true;
            existing.UpdatedAt = now;
        }

        await _db.SaveChangesAsync();
    }

    public async Task UnregisterAsync(Guid userId, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;

        var normalizedToken = token.Trim();
        var existing = await _db.PushDeviceTokens
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == normalizedToken && x.IsActive);

        if (existing is null)
            return;

        existing.IsActive = false;
        existing.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
