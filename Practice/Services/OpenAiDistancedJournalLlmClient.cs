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
            if (IsLowSignalAnswer(input.MainAnswer, input.FollowUpAnswer))
                return new DistancedJournalGeneratedQuestionResult(GetClarificationQuestion(input.Language));

            var content = await CreateChatCompletionAsync(
                BuildPrompt(input, avoidQuestions),
                cancellationToken);

            var result = JsonSerializer.Deserialize<GeneratedQuestionJson>(content, ReadJsonOptions)
                ?? throw new InvalidOperationException("The distanced journal generator returned an empty response.");

            if (string.IsNullOrWhiteSpace(result.Question))
                throw new InvalidOperationException("The distanced journal generator returned an empty question.");

            return new DistancedJournalGeneratedQuestionResult(result.Question.Trim());
        }

        private static bool IsLowSignalAnswer(string? mainAnswer, string? followUpAnswer)
        {
            var combined = $"{mainAnswer} {followUpAnswer}".Trim();
            if (combined.Length < 8)
                return true;

            var words = combined
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(word => new string(word.Where(char.IsLetter).ToArray()))
                .Where(word => word.Length >= 2)
                .ToArray();

            if (words.Length < 3)
                return true;

            var meaningfulWords = words.Count(HasVowel);
            return meaningfulWords < 2;
        }

        private static bool HasVowel(string value)
        {
            return value.Any(c => "aeiouAEIOUаеиоуАЕИОУ".Contains(c));
        }

        private static string GetClarificationQuestion(string? language)
        {
            return IsEnglish(language)
                ? "Could you add one concrete sentence about what happened or how the person felt?"
                : "Možete li dodati jednu konkretnu rečenicu o tome šta se desilo ili kako se osoba osećala?";
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
                : "Write in natural Serbian, Latin script, ekavian, with diacritics (š, ć, č, đ, ž).";

            var avoidList = avoidQuestions.Count == 0
                ? "- none"
                : string.Join(Environment.NewLine, avoidQuestions.Select(q => $"- {q}"));

            return $$"""
Generate one optional follow-up question for a distanced journal entry.

Your job:
- Read the user's answers and ask one human, supportive question that follows naturally from what they wrote.
- The question should help the user gently continue the reflection, not analyze the entry like a school text.
- {{languageInstruction}}

First decide whether the answers are meaningful:
- If the answers are one-letter text, keyboard mashing, random syllables, punctuation, timestamps, or do not describe any situation, do not infer meaning from them.
- For not meaningful Serbian answers, return: "Možete li dodati jednu konkretnu rečenicu o tome šta se desilo ili kako se osoba osećala?"
- For not meaningful English answers, return: "Could you add one concrete sentence about what happened or how the person felt?"

Anchor the question:
- Identify whose experience the entry is mainly describing.
- Ask about that central experience, feeling, action, need, value, or moment.
- If several people are mentioned and it is unclear whose perspective is central, do not guess. Ask about the scene, the feeling, the action, or "the person" instead.
- Do not shift the question to another mentioned person unless the entry is clearly centered on that person.
- Use a person's name or role only when it is clearly the natural subject of the entry.
- When two or more people could plausibly be the subject, neutral wording is better than choosing a name.
- Preserve the user's distance: if the answers are written in third person, ask in third person. Use direct address only when the user's own answers are clearly written in first or second person.

Style:
- Return exactly one question.
- Keep it short, natural, and concrete.
- Prefer what/how questions.
- Do not ask why.
- Do not ask multiple questions.
- Do not offer choices or use colon-separated options.
- Do not give advice, reassurance, praise, diagnosis, or a lesson.
- Do not judge anyone in the story.
- Do not repeat or closely paraphrase a question in the avoid list.
- Do not quote malformed or random text from the user's answers.
- Do not use detached observer language, conclusion language, or abstract "what does this show" phrasing.
- Do not default to stock phrases about preserving, keeping, or carrying something forward unless the user's answer is explicitly about memory, preservation, or continuity.
- Avoid generic "what does it mean" questions when a concrete feeling, action, need, or moment from the answer can be used.
- For painful entries, ask about what could support the central person, what hurt, what they needed, what boundary mattered, or what would make the moment easier to understand.
- For positive entries, ask about what felt good, meaningful, connecting, relieving, encouraging, or worth noticing in that moment.

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
