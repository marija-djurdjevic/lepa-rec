using Microsoft.Extensions.Logging;

namespace AngularNetBase.Identity.Services;

public class LoggingPushNotificationSender : IPushNotificationSender
{
    private readonly ILogger<LoggingPushNotificationSender> _logger;

    public LoggingPushNotificationSender(ILogger<LoggingPushNotificationSender> logger)
    {
        _logger = logger;
    }

    public Task<int> SendAsync(
        IReadOnlyCollection<string> tokens,
        string title,
        string body,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Push reminder queued for {Count} tokens. Title: {Title}",
            tokens.Count,
            title);
        return Task.FromResult(tokens.Count);
    }
}
