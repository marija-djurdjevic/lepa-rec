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

    private static readonly (string Title, string Body)[] SerbianReminderMessages =
    [
        ("Dnevna ve\u017eba", "Zastani na trenutak i sagledaj \u0161iru sliku. Tvoj dana\u0161nji izazov je spreman."),
        ("Dnevna ve\u017eba", "Iza\u0111i iz svog okvira i vidi stvari jasnije. Vreme je za dana\u0161nju ve\u017ebu."),
        ("Dnevna ve\u017eba", "Promeni ugao gledanja i donesi bolju odluku. Pogledaj novi zadatak."),
        ("Dnevna ve\u017eba", "Ne dozvoli da emocije u afektu odlu\u010duju umesto tebe. Re\u0161i dana\u0161nji izazov."),
        ("Dnevna ve\u017eba", "Sa\u010duvaj hladnu glavu i izbegni impulsivne reakcije. \u010ceka te nova ve\u017eba."),
        ("Dnevna ve\u017eba", "Oslobodi se pritiska i povrati unutra\u0161nji mir. Otvori svoj dana\u0161nji zadatak."),
        ("Dnevna ve\u017eba", "Budi danas sebi najbolji savetnik. Vreme je za tvoj novi izazov."),
        ("Dnevna ve\u017eba", "Razumi malo bolje i sebe i druge. Pogledaj \u0161ta te \u010deka u dana\u0161njoj ve\u017ebi."),
        ("Dnevna ve\u017eba", "Sa\u010duvaj odnose koji ti zna\u010de kroz promenu perspektive. Otvori zadatak.")
    ];

    private static readonly (string Title, string Body)[] EnglishReminderMessages =
    [
        ("Today's exercise", "Pause for a moment and take in the bigger picture. Your challenge for today is ready."),
        ("Today's exercise", "Step outside your usual frame and see things more clearly. It's time for today's exercise."),
        ("Today's exercise", "Shift your perspective and make a better decision. Take a look at the new task."),
        ("Today's exercise", "Don't let intense emotions make decisions for you. Complete today's challenge."),
        ("Today's exercise", "Keep a cool head and avoid impulsive reactions. A new exercise is waiting for you."),
        ("Today's exercise", "Let go of the pressure and regain your inner calm. Open today's task."),
        ("Today's exercise", "Be your own best advisor today. It's time for your new challenge."),
        ("Today's exercise", "Understand yourself and others a little better. See what's waiting in today's exercise."),
        ("Today's exercise", "Protect the relationships that matter to you through a shift in perspective. Open the task.")
    ];

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

            var (title, body) = GetLocalizedReminderText(user.PreferredLanguage, localDate);
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

    private static (string Title, string Body) GetLocalizedReminderText(string? preferredLanguage, DateOnly localDate)
    {
        var isSerbian = !string.IsNullOrWhiteSpace(preferredLanguage)
            && preferredLanguage.StartsWith("sr", StringComparison.OrdinalIgnoreCase);

        var messages = isSerbian ? SerbianReminderMessages : EnglishReminderMessages;
        var messageIndex = Math.Abs(localDate.DayNumber) % messages.Length;

        return messages[messageIndex];
    }
}
