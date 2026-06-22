using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AngularNetBase.Practice.Services
{
    public class OpenAiDistancedJournalLlmClient : IDistancedJournalLlmClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private static readonly JsonSerializerOptions ReadJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly DistancedJournalLlmOptions _options;

        public OpenAiDistancedJournalLlmClient(
            HttpClient httpClient,
            IOptions<DistancedJournalLlmOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<DistancedJournalGeneratedQuestionResult> GenerateReflectionQuestionAsync(
            DistancedJournalQuestionInput input,
            IReadOnlyCollection<string> avoidQuestions,
            CancellationToken cancellationToken = default)
        {
            var content = await CreateChatCompletionAsync(
                BuildPrompt(input, avoidQuestions),
                cancellationToken);

            var result = JsonSerializer.Deserialize<GeneratedQuestionJson>(content, ReadJsonOptions)
                ?? throw new InvalidOperationException("The distanced journal generator returned an empty response.");

            if (string.IsNullOrWhiteSpace(result.Question))
                throw new InvalidOperationException("The distanced journal generator returned an empty question.");

            return new DistancedJournalGeneratedQuestionResult(result.Question.Trim());
        }

        private async Task<string> CreateChatCompletionAsync(
            string userPrompt,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
                throw new InvalidOperationException("OpenAI API key is not configured. Set OpenAI:ApiKey or OpenAI__ApiKey.");

            var baseUrl = string.IsNullOrWhiteSpace(_options.BaseUrl)
                ? "https://api.openai.com/v1"
                : _options.BaseUrl.TrimEnd('/');

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            var body = new
            {
                model = string.IsNullOrWhiteSpace(_options.Model) ? "gpt-4o-mini" : _options.Model,
                temperature = 0.35,
                response_format = new { type = "json_object" },
                messages = new[]
                {
                    new { role = "system", content = "You are a careful distanced-journaling assistant. Follow the user prompt exactly and return valid JSON only." },
                    new { role = "user", content = userPrompt }
                }
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(body, JsonOptions),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"OpenAI request failed with status {(int)response.StatusCode}: {responseBody}");

            using var document = JsonDocument.Parse(responseBody);
            var content = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(content))
                throw new InvalidOperationException("OpenAI returned an empty message.");

            return content;
        }

        private static string BuildPrompt(
            DistancedJournalQuestionInput input,
            IReadOnlyCollection<string> avoidQuestions)
        {
            var languageInstruction = IsEnglish(input.Language)
                ? "Write in natural English."
                : "Write in natural Serbian, Latin script, ekavian, with diacritics (š, ć, č, đ, ž). Address the user in second person plural/formal form.";

            var avoidList = avoidQuestions.Count == 0
                ? "- none"
                : string.Join(Environment.NewLine, avoidQuestions.Select(q => $"- {q}"));

            return $$"""
Generate one optional final reflection question for a distanced journal exercise.

Goal:
- Use the user's two answers to notice one self-relevant meaning they gave to the situation.
- That meaning may be painful, neutral, or positive. Positive entries should be treated as real sources of value, pride, connection, hope, growth, or strength, not as hidden problems.
- Ask one short question that helps the user look at that self-relevant meaning with a little distance.

Rules:
- {{languageInstruction}}
- Return exactly one question.
- Do not repeat, closely paraphrase, or answer any question in the avoid list.
- Do not give advice, reassurance, praise, diagnosis, interpretation of personality, or a lesson.
- Do not ask multiple questions.
- Do not use clinical language.
- Keep it concrete enough to connect to the user's words, but general enough that it does not overfit one detail.
- Prefer "what/how" questions over "why" questions.
- If the answers contain something the user told themselves, target that inner sentence or meaning.
- If there is no painful self-talk, target a value, need, hope, strength, pride, connection, or meaning the user seems to be naming.

Avoid list:
{{avoidList}}

Exercise:
Opening question: {{input.OpeningQuestion}}
Follow-up question: {{input.FollowUpQuestion}}
Reflection question, if already defined: {{input.ReflectionQuestion ?? "none"}}

User answers:
First answer: {{input.MainAnswer}}
Second answer: {{input.FollowUpAnswer}}

Return JSON only:
{ "question": "..." }
""";
        }

        private static bool IsEnglish(string? language)
        {
            return !string.IsNullOrWhiteSpace(language)
                && language.StartsWith("en", StringComparison.OrdinalIgnoreCase);
        }

        private sealed class GeneratedQuestionJson
        {
            public string? Question { get; set; }
        }
    }
}
