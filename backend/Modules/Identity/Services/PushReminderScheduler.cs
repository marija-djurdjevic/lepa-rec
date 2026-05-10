using AngularNetBase.Identity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AngularNetBase.Identity.Services;

public class PushReminderScheduler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PushReminderScheduler> _logger;

    public PushReminderScheduler(IServiceProvider serviceProvider, ILogger<PushReminderScheduler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunTickAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Push reminder scheduler tick failed.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task RunTickAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        var sender = scope.ServiceProvider.GetRequiredService<IPushNotificationSender>();
        var nowUtc = DateTime.UtcNow;

        var users = await db.Users
            .Where(x => x.OnboardingCompleted
                        && x.NotificationEnabled
                        && x.NotificationTimeLocal != null
                        && x.TimeZoneId != null)
            .Select(x => new
            {
                x.Id,
                x.PreferredLanguage,
                x.NotificationTimeLocal,
                x.TimeZoneId
            })
            .ToListAsync(cancellationToken);

        foreach (var user in users)
        {
            if (!TimeOnly.TryParse(user.NotificationTimeLocal, out var targetTime))
                continue;

            TimeZoneInfo tz;
            try
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneId!);
            }
            catch
            {
                continue;
            }

            var localNow = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, tz);
            if (localNow.Hour != targetTime.Hour || localNow.Minute != targetTime.Minute)
                continue;

            var localDate = DateOnly.FromDateTime(localNow);
            var alreadySent = await db.PushReminderDispatches
                .AnyAsync(x => x.UserId == user.Id && x.LocalDate == localDate, cancellationToken);
            if (alreadySent)
                continue;

            var tokens = await db.PushDeviceTokens
                .Where(x => x.UserId == user.Id && x.IsActive)
                .Select(x => x.Token)
                .ToListAsync(cancellationToken);

            if (tokens.Count == 0)
                continue;

            var (title, body) = GetLocalizedReminderText(user.PreferredLanguage);
            var successCount = await sender.SendAsync(
                tokens,
                title,
                body,
                cancellationToken);

            if (successCount <= 0)
                continue;

            db.PushReminderDispatches.Add(new Entities.PushReminderDispatch
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                LocalDate = localDate,
                SentAtUtc = nowUtc
            });
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    private static (string Title, string Body) GetLocalizedReminderText(string? preferredLanguage)
    {
        var isSerbian = !string.IsNullOrWhiteSpace(preferredLanguage)
            && preferredLanguage.StartsWith("sr", StringComparison.OrdinalIgnoreCase);

        if (isSerbian)
            return ("Dnevni podsetnik", "Vaš izazov vas čeka.");

        return ("Daily reminder", "Your challenge is waiting.");
    }
}
