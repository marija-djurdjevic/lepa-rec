namespace AngularNetBase.Identity.Services;

public interface IPushNotificationSender
{
    Task<int> SendAsync(
        IReadOnlyCollection<string> tokens,
        string title,
        string body,
        CancellationToken cancellationToken = default);
}
