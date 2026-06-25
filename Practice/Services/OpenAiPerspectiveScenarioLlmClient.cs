using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AngularNetBase.Practice.Services
{
    public class OpenAiPerspectiveScenarioLlmClient : IPerspectiveScenarioLlmClient
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
        private readonly PerspectiveScenarioLlmOptions _options;

        public OpenAiPerspectiveScenarioLlmClient(
            HttpClient httpClient,
            IOptions<PerspectiveScenarioLlmOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<PerspectiveScenarioGradeResult> GradeAnswerAsync(
            PerspectiveScenarioLlmInput input,
            CancellationToken cancellationToken = default)
        {
            var content = await CreateChatCompletionAsync(
                BuildGraderPrompt(input),
                ResolveGraderModel(),
                stream: false,
                cancellationToken);

            var result = JsonSerializer.Deserialize<GradeJson>(content, ReadJsonOptions)
                ?? throw new InvalidOperationException("The grader returned an empty response.");

            return new PerspectiveScenarioGradeResult(
                Math.Clamp(result.Score, 1, 5),
                NormalizeIssues(result.Issues),
                NormalizeStrings(result.Strengths),
                NormalizeLanguage(result.Language));
        }

        public async Task<PerspectiveScenarioGuideResult> GenerateGuideQuestionAsync(
            PerspectiveScenarioLlmInput input,
            PerspectiveScenarioGradeResult grade,
            int iterationIndex,
            CancellationToken cancellationToken = default)
        {
            var content = await CreateChatCompletionAsync(
                BuildGuidePrompt(input, grade, iterationIndex),
                ResolveGuideModel(),
                stream: false,
                cancellationToken);

            return ParseGuideResult(content);
        }

        public async Task<PerspectiveScenarioGuideResult> StreamGuideQuestionAsync(
            PerspectiveScenarioLlmInput input,
            PerspectiveScenarioGradeResult grade,
            int iterationIndex,
            Func<string, CancellationToken, Task> onQuestionChunk,
            CancellationToken cancellationToken = default)
        {
            var prompt = BuildGuidePrompt(input, grade, iterationIndex);
            var rawJson = await StreamChatCompletionAsync(
                prompt,
                ResolveGuideModel(),
                "next_question",
                onQuestionChunk,
                cancellationToken);

            return ParseGuideResult(rawJson);
        }

        private async Task<string> CreateChatCompletionAsync(
            string userPrompt,
            string model,
            bool stream,
            CancellationToken cancellationToken)
        {
            var request = CreateRequest(userPrompt, model, stream);

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

        private async Task<string> StreamChatCompletionAsync(
            string userPrompt,
            string model,
            string streamedJsonProperty,
            Func<string, CancellationToken, Task> onPropertyChunk,
            CancellationToken cancellationToken)
        {
            var request = CreateRequest(userPrompt, model, stream: true);

            using var response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"OpenAI streaming request failed with status {(int)response.StatusCode}: {responseBody}");
            }

            var buffer = new StringBuilder();
            var propertyStreamer = new JsonStringPropertyStreamer(streamedJsonProperty);

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var line = await reader.ReadLineAsync(cancellationToken);
                if (line is null)
                    break;

                if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data: ", StringComparison.Ordinal))
                    continue;

                var data = line["data: ".Length..];
                if (data == "[DONE]")
                    break;

                using var chunkDocument = JsonDocument.Parse(data);
                var choices = chunkDocument.RootElement.GetProperty("choices");
                if (choices.GetArrayLength() == 0)
                    continue;

                var delta = choices[0].GetProperty("delta");
                if (!delta.TryGetProperty("content", out var contentElement))
                    continue;

                var content = contentElement.GetString();
                if (string.IsNullOrEmpty(content))
                    continue;

                buffer.Append(content);

                foreach (var questionChunk in propertyStreamer.Push(content))
                {
                    await onPropertyChunk(questionChunk, cancellationToken);
                }
            }

            return buffer.ToString();
        }

        private HttpRequestMessage CreateRequest(string userPrompt, string model, bool stream)
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
                model,
                temperature = _options.Temperature,
                response_format = new { type = "json_object" },
                stream,
                messages = new[]
                {
                    new { role = "system", content = "You are a careful empathy-exercise assistant. Follow the user prompt exactly and return valid JSON only." },
                    new { role = "user", content = userPrompt }
                }
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(body, JsonOptions),
                Encoding.UTF8,
                "application/json");

            return request;
        }

        private string ResolveGraderModel()
        {
            if (!string.IsNullOrWhiteSpace(_options.GraderModel))
                return _options.GraderModel;

            return string.IsNullOrWhiteSpace(_options.Model)
                ? "gpt-4o-mini"
                : _options.Model;
        }

        private string ResolveGuideModel()
        {
            if (!string.IsNullOrWhiteSpace(_options.GuideModel))
                return _options.GuideModel;

            return string.IsNullOrWhiteSpace(_options.Model)
                ? "gpt-4o"
                : _options.Model;
        }

        private static PerspectiveScenarioGuideResult ParseGuideResult(string content)
        {
            var result = JsonSerializer.Deserialize<GuideJson>(content, ReadJsonOptions)
                ?? throw new InvalidOperationException("The guide returned an empty response.");

            if (string.IsNullOrWhiteSpace(result.NextQuestion))
                throw new InvalidOperationException("The guide did not return a next question.");

            return new PerspectiveScenarioGuideResult(
                result.NextQuestion.Trim(),
                string.IsNullOrWhiteSpace(result.WhyThisQuestion) ? null : result.WhyThisQuestion.Trim());
        }

        private static string BuildGraderPrompt(PerspectiveScenarioLlmInput input)
        {
            return $$"""
            You are grading one learner response in a reveal-based empathy and perspective-taking exercise.

            Inputs:
            - scenario: situation shown to the learner
            - question: question the learner answered
            - reveal: hidden context or perspective that explains the situation
            - learner_answer: learner response
            - answer_language: optional language hint such as "sr", "en", "mixed", or "unknown"

            Task:
            Grade how well learner_answer demonstrates reveal-aligned perspective-taking.

            Evaluate silently:
            1. What is the requested target of the original question: who or what is the learner being asked about?
            2. What is the requested dimension of the original question: feeling/emotion, personal meaning, cause/explanation, past experience or learned association, belief or assumption, need or concern, constraint or pressure, blind spot or misunderstanding, effect or impact, relationship dynamic, or behavior explanation?
            3. What is the reveal's core perspective shift for that requested target and requested dimension?
            4. What minimum insight would show that the learner understood that shift at the right level of abstraction?
            5. Interpret the learner's answer charitably, in its strongest reasonable form.
            Do not include this reasoning in the JSON.

            General grading principles:
            - Grade functional meaning, not keyword overlap.
            - Grade perspective-taking, not writing quality.
            - Ignore grammar, spelling, punctuation, fluency, length, dialect, script, and code-switching.
            - Accept Serbian, English, mixed Serbian/English, Latin script, or Cyrillic script.
            - Treat answer_language only as a hint.
            - Do not require every reveal detail.
            - Do not reward unsupported mind-reading or invented backstory.
            - Short answers can score high if precise and grounded.
            - Long answers should not score high if generic or padded.
            - Be fair, but do not complete missing reasoning for the learner.

            Requested-dimension rule:
            - Grade learner_answer against the requested target and requested dimension of the original question.
            - Do not penalize the learner for not answering a different dimension, even if that other dimension appears in the reveal.
            - Accept answers that capture the requested dimension at the right level of abstraction.
            - The learner does not need to reproduce exact reveal details, names, events, diagnoses, or wording if they identify the same functional meaning.
            - If the answer gives a good answer to a nearby but different dimension, score it lower than an answer to the requested dimension, but preserve credit for any reveal-aligned insight.

            Compact-answer rule:
            - Short answers can score high.
            - A compact phrase is acceptable when it captures the requested insight clearly enough for a facilitator to understand what the learner means.
            - Do not require full sentences, polished wording, or explanation chains.
            - If a compact answer only names a broad category without connecting it to the requested person's perspective in this scenario, score at most 3 unless the original question only asks for a simple emotion label.
            - Very short answers can score 4 only when the original question asks for a simple emotion or label and the answer precisely names the core feeling or label.
            - For simple emotion questions, a very short answer may score 4 only if it names the requested person's inner feeling, not merely another person's apparent behavior, attitude, or reaction.
            - If a short answer says what someone else seems to be doing without clearly stating how the requested person feels because of it, score at most 3.
            - For questions asking why, what prevents, what is behind, what effect something has, what message is sent, or what a person does not understand, one-word or category-only answers are capped at 3.
            - For score 4, a compact answer must still give a usable inner model or clear scenario-specific connection.

            Invalid-answer rule:
            If learner_answer is empty, punctuation-only, a filler phrase, a clarification question, or not an attempt to answer the question, score 1.
            Use issue "insufficient_answer" for no real answer attempt.
            Use issue "off_topic" for comments/questions about the task rather than an answer.
            Invalid answers must have strengths [].

            Score rubric:
            - 5 = Excellent. Captures the core hidden perspective precisely and with useful nuance. Well-grounded, not overclaimed.
            - 4 = Good/accepted. Gives a usable inner model that explains the person's feeling, reaction, belief, choice, avoidance, or misunderstanding. May miss secondary nuance.
            - 3 = Partial. Directionally aligned, but the main why is incomplete, vague, or not clearly connected to the question.
            - 2 = Weak. Some plausible relevance, but mostly generic, shallow, aimed at the wrong concern, or only minimally reveal-aligned.
            - 1 = Not meaningful. Off-topic, not an answer, hostile, blaming, nonsensical, surface-level, directly contradictory, or not perspective-taking.

            Boundary rules:
            - Scores 4 and 5 mean the answer is acceptable without another guide question.
            - If a serious issue remains, such as misses_perspective, contradicts_reveal, judgmental_or_blaming, invalidating, hostile_or_unsafe, or generic_or_vague, score at most 3 unless the issue is clearly minor and does not affect the requested insight.
            - Never return score 4 or 5 with misses_perspective, generic_or_vague, judgmental_or_blaming, contradicts_reveal, invalidating, or hostile_or_unsafe.
            - If a human facilitator could reasonably say "yes, they understood the person's perspective well enough to move on," score at least 4.
            - Score 4 only when the answer includes the central hidden perspective for the requested target and dimension, not just a plausible surface interpretation.
            - If the answer names only an observable reaction, a broad emotion, a social role, or one partial motive while missing the deeper mechanism in the reveal, score at most 3.
            - If the answer lists multiple guesses and only one is reveal-aligned, score at most 3 unless it clearly commits to the aligned interpretation as the main answer.
            - If the answer contains several alternative guesses, grade the whole answer, not only the best phrase. A correct guess mixed with unrelated or contradictory guesses should not score 4 unless the learner clearly marks the correct interpretation as the main answer.
            - If the question asks for a feeling but the answer mostly gives a motive, action, intention, or behavior explanation, score at most 3 unless the feeling is also clearly named. Apply the same principle to other requested dimensions.
            - If the answer has the right general direction but does not explain the core shift, score 3.
            - If the answer contains both a correct insight and a wrong assumption, grade the central interpretation, not one isolated phrase.
            - If the answer contradicts the reveal's core meaning, score 1 unless there is still a meaningful partial perspective insight.
            - If the answer identifies a relevant member of the requested dimension, score at least 3 unless it is mostly wrong or misleading.
            - If the answer identifies the central insight of the requested dimension in everyday language with enough scenario-specific meaning, score at least 4 even if it omits secondary reveal details.
            - If the answer introduces a concrete unsupported life event, diagnosis, motive, relationship history, or backstory that is not in the scenario or reveal, and that detail is central to the answer, score at most 3.
            - Do not default to 3. Choose the highest score fully supported by the answer.

            Allowed issue codes:
            - off_topic
            - insufficient_answer
            - misses_core_feeling
            - misses_perspective
            - contradicts_reveal
            - generic_or_vague
            - judgmental_or_blaming
            - invalidating
            - hostile_or_unsafe

            Return valid JSON only.
            Do not return markdown.
            Do not return any text before or after the JSON.
            Return this exact shape:
            {
              "score": 1,
              "issues": ["misses_perspective"],
              "strengths": ["recognizes one relevant concern"],
              "language": "sr"
            }

            Output rules:
            - score must be an integer from 1 to 5
            - issues must be an array and use only allowed issue codes
            - strengths must be short and concrete
            - language must be one of: "sr", "en", "mixed", "unknown"
            - excellent answers may have issues []
            - scores 4 and 5 must have issues [] for all issue codes that signal a missed, vague, blaming, contradictory, invalidating, hostile, or unsafe answer
            - do not include explanations outside JSON

            Input:
            {
              "scenario": {{JsonSerializer.Serialize(input.Scenario)}},
              "question": {{JsonSerializer.Serialize(input.Question)}},
              "reveal": {{JsonSerializer.Serialize(input.Reveal)}},
              "learner_answer": {{JsonSerializer.Serialize(input.LearnerAnswer)}},
              "answer_language": {{JsonSerializer.Serialize(input.TargetLanguage)}}
            }
            """;
        }

        private static string BuildGuidePrompt(
            PerspectiveScenarioLlmInput input,
            PerspectiveScenarioGradeResult grade,
            int iterationIndex)
        {
            return $$"""
            You are generating one follow-up question for a reveal-based perspective-taking exercise.

            The reveal is teacher-only. Use it to choose a helpful scaffold, but do not disclose hidden facts from it.

            Inputs:
            - scenario: situation shown to the learner
            - question: original question the learner answered
            - reveal: hidden context or perspective
            - learner_answer: learner's latest answer
            - grade: current score from 1 to 3
            - issues: grading issue codes
            - history: previous learner answers and guide questions
            - iteration_index: current guide attempt
            - target_language: "sr" or "en"

            Task:
            Generate exactly one follow-up question that helps the learner improve their answer to the original question.

            Evaluate silently:
            1. What is the requested target of the original question: who or what is the learner being asked about?
            2. What is the requested dimension of the original question: feeling/emotion, personal meaning, cause/explanation, past experience or learned association, belief or assumption, need or concern, constraint or pressure, blind spot or misunderstanding, effect or impact, relationship dynamic, or behavior explanation?
            3. What is the reveal's core perspective shift for that requested target and requested dimension?
            4. What did the learner already understand?
            5. What is the single most useful missing step inside the same requested target and requested dimension?
            6. Has that missing step already been asked or answered in history?
            Do not include this reasoning in the JSON.

            Rules:
            - Ask one question only.
            - The follow-up must preserve both the requested target and requested dimension of the original question.
            - Do not switch to another person, relationship, action, constraint, feeling, blind spot, or dynamic just because that appears in the reveal.
            - Do not switch to another kind of insight just because that other dimension appears in the reveal.
            - Do not switch to another person's inner state unless that is the requested target or requested dimension.
            - Respond to the learner's actual answer, not only to the scenario.
            - Preserve any correct part of the learner's answer.
            - Ask for one missing mental step, not the whole explanation.
            - Make the question narrower than the original question.
            - Do not answer the question for the learner.
            - Do not put the likely correct answer inside the guide question.
            - Ask open-ended questions that require the learner to explain their thinking.
            - Do not ask questions that can be answered with only yes or no.
            - Do not ask confirmation questions where the learner can simply agree with the suggested interpretation.
            - Do not ask multiple-choice or either/or questions; the learner should generate the missing explanation, not choose between options.
            - Do not name the missing reveal concept directly if the learner has not already named it.
            - Ask for the missing type of reasoning instead, such as a feeling, assumption, meaning, concern, pressure, missing information, or visible clue.
            - Prefer question forms that ask what, how, why, or which visible clue the learner is using.

            Question shape:
            - The guide question must ask the learner to produce an explanation in their own words.
            - It must not ask the learner to confirm, reject, compare, or choose between suggested interpretations.
            - It must not contain the correct interpretation, a near-answer, or a contrast where one side is clearly the better answer.
            - It must not use a structure equivalent to "Could it be that...", "Is it more X than Y?", "Is the problem that...", "Is it because...", or "Do you think...".
            - Do not use yes/no framing anywhere in the guide question, even embedded inside a larger open-ended question.
            - Avoid asking what something says about whether an idea is true, welcome, allowed, justified, correct, or possible.
            - Rephrase yes/no-framed drafts as open questions about what message something sends, what it might mean to the person, what need, fear, assumption, or pressure is underneath, or which visible clue points there.
            - Prefer open-ended structures equivalent to asking what something might mean to the requested person, how the person might be interpreting the situation, what the person might be assuming, what the person could be missing, which visible clue points to the answer, or what might make the situation feel difficult for the person.
            - Do not ask for advice, repair, solutions, or behavior change unless the original question asks for that.
            - Do not repeat an idea already asked or answered in history.
            - If history exists, move one step narrower or toward a different missing piece.

            Reveal protection:
            - Do not reveal facts, motives, events, relationships, preferences, identities, or background details that appear only in the reveal.
            - Do not turn a hidden reveal detail into a leading question.
            - You may point abstractly toward the type of missing idea, such as an assumption, pressure, concern, misunderstanding, meaning, constraint, missing information, or tension.
            - If the learner already named a reveal detail, you may refer to it in their own wording.
            - Use only visible scenario facts, the learner's own answer, and abstract categories of missing insight.
            - A guide question leaks the reveal if it mentions a concrete fact, place, action, event, diagnosis, motive, relationship history, or background detail that appears only in the reveal and not in the scenario or learner answer.
            - A guide question also leaks the reveal if it states a reveal-only emotional label, motive, relationship interpretation, hidden pressure, system explanation, or background event as the suggested answer.
            - Before returning the guide question, silently compare it against the visible scenario and learner answer. If it contains reveal-only concrete information, rewrite it more abstractly.
            - If your draft question contains the answer, a near-synonym of the missing insight, or a suggested interpretation the learner could simply accept, rewrite it one level more abstractly.

            Guide strategy:
            - If there is no real answer, the answer is unclear, or the answer is mostly about the learner instead of the scenario, ask them to answer the original question directly in simpler words.
            - For off-task or unclear answers, do not scaffold a hidden reveal detail yet.
            - If the learner answered the wrong target, redirect to the requested target.
            - If the learner answered the wrong dimension, redirect to the requested dimension.
            - If the learner gave a generic answer, ask for a more specific clue from the scenario.
            - If the learner gave a partially correct answer, preserve the correct part and ask for the missing layer.
            - If the learner made a blaming or judgmental interpretation, ask for a non-blaming explanation within the requested target and requested dimension.
            - If the learner already named the main missing idea in everyday words, do not ask for the same idea again.

            Iteration:
            - iteration 1: broad but targeted
            - iteration 2: narrower and more diagnostic
            - iteration 3: clearest non-revealing question about the remaining misunderstanding

            Language:
            - Write only in target_language.
            - For Serbian, use natural ekavian Latin script.
            - For Serbian, use simple everyday wording and avoid literal English sentence structure.
            - For Serbian, avoid awkward or unnatural phrasing.
            - For Serbian, avoid ungrammatical constructions such as "Šta mu je X normalna" or "a ne samo da je Dušan smeta"; rewrite them naturally.
            - For Serbian, prefer natural forms like "Kako on doživljava...", "Šta ona možda pretpostavlja...", "Šta njemu tu deluje...", and "Šta bi njoj u tome moglo da smeta...".
            - For Serbian, write like a careful teacher or facilitator, not like translated English.
            - Before returning Serbian text, silently rewrite it once for naturalness and grammar.
            - Use simple everyday wording.
            - Prefer one short sentence.
            - Check grammar before returning.

            Return valid JSON only.
            Do not return markdown.
            Do not return any text before or after the JSON.
            Return this exact shape:
            {
              "next_question": "string",
              "why_this_question": "short string"
            }
            Output rules:
            - next_question must contain exactly one question
            - why_this_question must briefly name the missing step being targeted
            - why_this_question must not reveal hidden facts

            Final self-check before returning:
            1. If next_question can be answered with only yes or no, rewrite it.
            2. If next_question uses embedded yes/no framing, rewrite it as an open meaning, message, assumption, need, fear, pressure, or visible-clue question.
            3. If next_question contains a phrase equivalent to asking whether something is welcome, true, allowed, justified, possible, correct, or acceptable, rewrite it without a yes/no concept.
            4. If next_question gives the learner the answer or a strong hint, rewrite it.
            5. If next_question offers options to choose from, rewrite it.
            6. If next_question mentions reveal-only concepts, rewrite it more abstractly.

            Input:
            {
              "scenario": {{JsonSerializer.Serialize(input.Scenario)}},
              "question": {{JsonSerializer.Serialize(input.Question)}},
              "reveal": {{JsonSerializer.Serialize(input.Reveal)}},
              "learner_answer": {{JsonSerializer.Serialize(input.LearnerAnswer)}},
              "grade": {{grade.Score}},
              "issues": {{JsonSerializer.Serialize(grade.Issues)}},
              "history": {{JsonSerializer.Serialize(MapHistory(input.History))}},
              "iteration_index": {{iterationIndex}},
              "target_language": {{JsonSerializer.Serialize(input.TargetLanguage)}}
            }
            """;
        }

        private static IReadOnlyCollection<object> MapHistory(IReadOnlyCollection<ConversationTurn> history)
        {
            return history
                .OrderBy(x => x.CreatedAt)
                .Select(x => new
                {
                    role = x.Role == ConversationTurnRole.Learner ? "learner" : "guide",
                    message = x.Message,
                    mark = x.EvaluationSummary?.Mark,
                    issues = x.EvaluationSummary?.Issues ?? Array.Empty<string>()
                })
                .ToList();
        }

        private static IReadOnlyCollection<string> NormalizeIssues(IReadOnlyCollection<string>? issues)
        {
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "off_topic",
                "insufficient_answer",
                "misses_core_feeling",
                "misses_perspective",
                "contradicts_reveal",
                "generic_or_vague",
                "judgmental_or_blaming",
                "invalidating",
                "hostile_or_unsafe"
            };

            return NormalizeStrings(issues)
                .Where(allowed.Contains)
                .ToList();
        }

        private static IReadOnlyCollection<string> NormalizeStrings(IReadOnlyCollection<string>? values)
        {
            if (values is null)
                return Array.Empty<string>();

            return values
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string NormalizeLanguage(string? language)
        {
            if (string.IsNullOrWhiteSpace(language))
                return "unknown";

            var normalized = language.Trim().ToLowerInvariant();
            return normalized is "sr" or "en" or "mixed" or "unknown"
                ? normalized
                : "unknown";
        }

        private sealed class GradeJson
        {
            [JsonPropertyName("score")]
            public int Score { get; set; }

            [JsonPropertyName("issues")]
            public IReadOnlyCollection<string>? Issues { get; set; }

            [JsonPropertyName("strengths")]
            public IReadOnlyCollection<string>? Strengths { get; set; }

            [JsonPropertyName("language")]
            public string? Language { get; set; }
        }

        private sealed class GuideJson
        {
            [JsonPropertyName("next_question")]
            public string? NextQuestion { get; set; }

            [JsonPropertyName("why_this_question")]
            public string? WhyThisQuestion { get; set; }
        }

        private sealed class JsonStringPropertyStreamer
        {
            private readonly string _propertyName;
            private readonly StringBuilder _searchBuffer = new();
            private readonly StringBuilder _unicodeEscape = new();
            private bool _insideTargetValue;
            private bool _targetFound;
            private bool _escape;
            private bool _unicodeMode;

            public JsonStringPropertyStreamer(string propertyName)
            {
                _propertyName = $"\"{propertyName}\"";
            }

            public IEnumerable<string> Push(string content)
            {
                var emitted = new List<string>();

                foreach (var ch in content)
                {
                    if (!_targetFound)
                    {
                        _searchBuffer.Append(ch);
                        if (_searchBuffer.Length > _propertyName.Length + 20)
                            _searchBuffer.Remove(0, _searchBuffer.Length - (_propertyName.Length + 20));

                        if (_searchBuffer.ToString().Contains(_propertyName, StringComparison.Ordinal))
                            _targetFound = true;

                        continue;
                    }

                    if (!_insideTargetValue)
                    {
                        if (ch == '"')
                        {
                            _insideTargetValue = true;
                        }

                        continue;
                    }

                    if (_escape)
                    {
                        if (ch == 'u')
                        {
                            _unicodeMode = true;
                            _unicodeEscape.Clear();
                            _escape = false;
                            continue;
                        }

                        emitted.Add(ch switch
                        {
                            '"' => "\"",
                            '\\' => "\\",
                            '/' => "/",
                            'b' => "\b",
                            'f' => "\f",
                            'n' => "\n",
                            'r' => "\r",
                            't' => "\t",
                            _ => ch.ToString()
                        });
                        _escape = false;
                        continue;
                    }

                    if (_unicodeMode)
                    {
                        _unicodeEscape.Append(ch);
                        if (_unicodeEscape.Length < 4)
                            continue;

                        if (ushort.TryParse(
                            _unicodeEscape.ToString(),
                            System.Globalization.NumberStyles.HexNumber,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out var codePoint))
                        {
                            emitted.Add(((char)codePoint).ToString());
                        }

                        _unicodeMode = false;
                        _unicodeEscape.Clear();
                        continue;
                    }

                    if (ch == '\\')
                    {
                        _escape = true;
                        continue;
                    }

                    if (ch == '"')
                    {
                        _insideTargetValue = false;
                        _targetFound = false;
                        continue;
                    }

                    emitted.Add(ch.ToString());
                }

                return emitted;
            }
        }
    }
}
