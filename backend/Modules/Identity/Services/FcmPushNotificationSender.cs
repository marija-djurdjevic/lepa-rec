using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AngularNetBase.Identity.Services;

public class FcmPushNotificationSender : IPushNotificationSender
{
    private static readonly string[] Scopes = ["https://www.googleapis.com/auth/firebase.messaging"];

    private readonly FirebaseOptions _options;
    private readonly ILogger<FcmPushNotificationSender> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public FcmPushNotificationSender(
        IOptions<FirebaseOptions> options,
        ILogger<FcmPushNotificationSender> logger,
        IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<int> SendAsync(
        IReadOnlyCollection<string> tokens,
        string title,
        string body,
        CancellationToken cancellationToken = default)
    {
        if (tokens.Count == 0)
            return 0;

        var (credential, projectId) = await TryBuildCredentialAsync(cancellationToken);
        if (credential is null || string.IsNullOrWhiteSpace(projectId))
        {
            _logger.LogWarning("FCM config missing. Push send skipped for {Count} tokens.", tokens.Count);
            return 0;
        }

        var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync(null, cancellationToken);
        var url = $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send";
        var client = _httpClientFactory.CreateClient(nameof(FcmPushNotificationSender));

        var successCount = 0;
        foreach (var token in tokens.Distinct())
        {
            var payload = new
            {
                message = new
                {
                    token,
                    notification = new
                    {
                        title,
                        body
                    }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "FCM send failed for one token. Status: {Status}. Body: {Body}",
                    (int)response.StatusCode,
                    error);
            }
            else
            {
                successCount++;
            }
        }

        return successCount;
    }

    private async Task<(GoogleCredential? Credential, string? ProjectId)> TryBuildCredentialAsync(CancellationToken cancellationToken)
    {
        GoogleCredential? credential = null;
        string? projectId = _options.ProjectId;

        if (!string.IsNullOrWhiteSpace(_options.ServiceAccountJson))
        {
            credential = GoogleCredential.FromJson(_options.ServiceAccountJson).CreateScoped(Scopes);
            if (string.IsNullOrWhiteSpace(projectId))
                projectId = ExtractProjectId(_options.ServiceAccountJson);
        }
        else if (!string.IsNullOrWhiteSpace(_options.ServiceAccountPath) && File.Exists(_options.ServiceAccountPath))
        {
            var json = await File.ReadAllTextAsync(_options.ServiceAccountPath, cancellationToken);
            credential = GoogleCredential.FromJson(json).CreateScoped(Scopes);
            if (string.IsNullOrWhiteSpace(projectId))
                projectId = ExtractProjectId(json);
        }

        return (credential, projectId);
    }

    private static string? ExtractProjectId(string json)
    {
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("project_id", out var prop))
            return prop.GetString();

        return null;
    }
}
