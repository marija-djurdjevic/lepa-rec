using AngularNetBase.Practice.Dtos.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.Sessions;
using AngularNetBase.Practice.Entities.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public class PerspectiveScenarioService : IPerspectiveScenarioService
    {
        private readonly IPerspectiveScenarioChallengeRepository _challengeRepository;
        private readonly IPerspectiveScenarioExerciseRepository _exerciseRepository;
        private readonly IAnswerConversationRepository _conversationRepository;
        private readonly ISessionRepository _dailySessionRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPerspectiveScenarioLlmClient _llmClient;

        public PerspectiveScenarioService(
            IPerspectiveScenarioChallengeRepository challengeRepository,
            IPerspectiveScenarioExerciseRepository exerciseRepository,
            IAnswerConversationRepository conversationRepository,
            ISessionRepository dailySessionRepository,
            ISkillRepository skillRepository,
            IDateTimeProvider dateTimeProvider,
            IPerspectiveScenarioLlmClient llmClient)
        {
            _challengeRepository = challengeRepository;
            _exerciseRepository = exerciseRepository;
            _conversationRepository = conversationRepository;
            _dailySessionRepository = dailySessionRepository;
            _skillRepository = skillRepository;
            _dateTimeProvider = dateTimeProvider;
            _llmClient = llmClient;
        }

        public async Task<PerspectiveScenarioChallengeDto> CreateChallengeAsync(
            CreatePerspectiveScenarioChallengeDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto.Questions is null || !dto.Questions.Any())
                throw new ArgumentException("At least one question must be provided.", nameof(dto));

            var skillIds = dto.Questions.Select(x => x.SkillId).Distinct().ToList();
            var skills = await _skillRepository.GetByIdsAsync(skillIds, cancellationToken);

            if (skills.Count != skillIds.Count)
                throw new InvalidOperationException("One or more referenced skills were not found.");

            var challenge = new PerspectiveScenarioChallenge(
                Guid.NewGuid(),
                dto.Context,
                dto.ActorCount,
                dto.ScenarioText,
                dto.ChallengeLevel,
                dto.Questions.Select(x => (Guid.NewGuid(), x.SkillId, x.Order, x.QuestionText, x.Reveal, x.QuestionTextEn, x.RevealEn)),
                dto.ScenarioTextEn);

            await _challengeRepository.AddAsync(challenge, cancellationToken);
            await _challengeRepository.SaveChangesAsync(cancellationToken);

            return MapChallenge(challenge, null);
        }

        public async Task<IEnumerable<PerspectiveScenarioChallengeDto>> GetAllChallengesAsync(
            string? language = null,
            CancellationToken cancellationToken = default)
        {
            var challenges = await _challengeRepository.GetAllAsync(cancellationToken);
            return challenges.Select(challenge => MapChallenge(challenge, language));
        }

        public async Task<IEnumerable<PerspectiveScenarioChallengeDto>> GetChallengesByLevelAsync(
            ChallengeLevel challengeLevel,
            string? language = null,
            CancellationToken cancellationToken = default)
        {
            var challenges = await _challengeRepository.GetByChallengeLevelAsync(challengeLevel, cancellationToken);
            return challenges.Select(challenge => MapChallenge(challenge, language));
        }

        public async Task<PerspectiveScenarioPromptDto> GetRandomChallengeAsync(
            ChallengeLevel level,
            string? language = null,
            CancellationToken cancellationToken = default)
        {
            var challenge = await _challengeRepository.GetRandomByLevelAsync(level, cancellationToken);

            if (challenge is null)
                throw new InvalidOperationException("No perspective scenarios found for the selected level.");

            return MapPrompt(challenge, language);
        }

        public async Task<PerspectiveScenarioPromptDto> GetOnboardingHookChallengeAsync(
            string? language = null,
            CancellationToken cancellationToken = default)
        {
            var challenge = await _challengeRepository
                .GetOnboardingHookByKeyAsync("perspectivescenario.default", cancellationToken);

            if (challenge is null)
                throw new InvalidOperationException("Onboarding perspective scenario hook challenge is not configured.");

            return MapPrompt(challenge, language);
        }

        public async Task<PerspectiveScenarioExerciseDto> StartExerciseAsync(
            StartPerspectiveScenarioExerciseDto dto,
            bool isOnboardingHookRun = false,
            CancellationToken cancellationToken = default)
        {
            if (dto.UserId == Guid.Empty)
                throw new ArgumentException("UserId must be provided.", nameof(dto));

            if (dto.ChallengeId == Guid.Empty)
                throw new ArgumentException("ChallengeId must be provided.", nameof(dto));

            var challenge = await _challengeRepository.GetByIdAsync(dto.ChallengeId, cancellationToken);
            if (challenge is null)
                throw new InvalidOperationException("Perspective scenario challenge was not found.");
            if (isOnboardingHookRun && !challenge.IsOnboardingHook)
                throw new InvalidOperationException("Challenge is not configured as onboarding hook.");

            var exercise = new PerspectiveScenarioExercise(
                Guid.NewGuid(),
                dto.UserId,
                dto.ChallengeId,
                isOnboardingHookRun);

            await _exerciseRepository.AddAsync(exercise, cancellationToken);
            await _exerciseRepository.SaveChangesAsync(cancellationToken);

            return MapExercise(exercise);
        }

        public async Task<PerspectiveScenarioExerciseDto?> GetExerciseByIdAsync(
            Guid userId,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Exercise id must be provided.", nameof(id));

            var exercise = await _exerciseRepository.GetByIdAsync(id, cancellationToken);
            if (exercise is null || exercise.UserId != userId)
                return null;

            return exercise is null ? null : MapExercise(exercise);
        }

        public async Task<IEnumerable<PerspectiveScenarioExerciseDto>> GetExercisesByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id must be provided.", nameof(userId));

            var exercises = await _exerciseRepository.GetByUserIdAsync(userId, cancellationToken);
            return exercises.Select(MapExercise);
        }

        public async Task<SubmitPerspectiveScenarioResultDto> SubmitAnswersAsync(
            Guid userId,
            SubmitPerspectiveScenarioAnswerDto dto,
            string? language = null,
            bool trackInDailySession = true,
            CancellationToken cancellationToken = default)
        {
            if (dto.ExerciseId == Guid.Empty)
                throw new ArgumentException("ExerciseId must be provided.", nameof(dto));

            if (dto.Answers is null || !dto.Answers.Any())
                throw new ArgumentException("At least one answer must be provided.", nameof(dto));

            var exercise = await _exerciseRepository.GetByIdAsync(dto.ExerciseId, cancellationToken);
            if (exercise is null)
                throw new InvalidOperationException("Perspective scenario exercise was not found.");
            if (exercise.UserId != userId)
                throw new UnauthorizedAccessException("Exercise does not belong to the current user.");

            var challenge = await _challengeRepository.GetByIdAsync(exercise.ChallengeId, cancellationToken);
            if (challenge is null)
                throw new InvalidOperationException("Perspective scenario challenge was not found.");

            EnsureAnswersMatchChallengeQuestions(challenge, dto.Answers);

            var submittedAt = _dateTimeProvider.UtcNow;
            var answers = dto.Answers
                .Select(x => new ScenarioAnswer(x.QuestionId, x.AnswerText))
                .ToList();

            exercise.SubmitAnswers(answers, submittedAt);

            if (trackInDailySession)
            {
                var dailySession = await GetOrCreateTodaySessionAsync(exercise.UserId, cancellationToken);
                dailySession.RecordExercise(
                    exercise.Id,
                    ExerciseType.PerspectiveScenario,
                    submittedAt);

                await _dailySessionRepository.UpdateAsync(dailySession, cancellationToken);
                await _dailySessionRepository.SaveChangesAsync(cancellationToken);
            }
            else
            {
                await _exerciseRepository.UpdateAsync(exercise, cancellationToken);
                await _exerciseRepository.SaveChangesAsync(cancellationToken);
            }

            return new SubmitPerspectiveScenarioResultDto(
                MapExercise(exercise),
                challenge.Questions
                    .OrderBy(x => x.Order)
                    .Select(question => MapReveal(question, language))
                    .ToList());
        }

        public async Task<AnswerPerspectiveScenarioQuestionResultDto> AnswerQuestionAndGetRevealAsync(
            Guid userId,
            AnswerPerspectiveScenarioQuestionDto dto,
            string? language = null,
            bool trackInDailySession = true,
            CancellationToken cancellationToken = default)
        {
            return await HandleGuidedAnswerAsync(
                userId,
                dto,
                onGrade: null,
                onGuideQuestionChunk: null,
                language,
                trackInDailySession,
                cancellationToken);
        }

        public async Task<AnswerPerspectiveScenarioQuestionResultDto> AnswerQuestionAndStreamGuidanceAsync(
            Guid userId,
            AnswerPerspectiveScenarioQuestionDto dto,
            Func<PerspectiveScenarioGradeResult, CancellationToken, Task> onGrade,
            Func<string, CancellationToken, Task> onGuideQuestionChunk,
            string? language = null,
            bool trackInDailySession = true,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(onGrade);
            ArgumentNullException.ThrowIfNull(onGuideQuestionChunk);

            return await HandleGuidedAnswerAsync(
                userId,
                dto,
                onGrade,
                onGuideQuestionChunk,
                language,
                trackInDailySession,
                cancellationToken);
        }

        private async Task<AnswerPerspectiveScenarioQuestionResultDto> HandleGuidedAnswerAsync(
            Guid userId,
            AnswerPerspectiveScenarioQuestionDto dto,
            Func<PerspectiveScenarioGradeResult, CancellationToken, Task>? onGrade,
            Func<string, CancellationToken, Task>? onGuideQuestionChunk,
            string? language,
            bool trackInDailySession,
            CancellationToken cancellationToken)
        {
            if (dto.ExerciseId == Guid.Empty)
                throw new ArgumentException("ExerciseId must be provided.", nameof(dto));

            if (dto.QuestionId == Guid.Empty)
                throw new ArgumentException("QuestionId must be provided.", nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.AnswerText))
                throw new ArgumentException("AnswerText must be provided.", nameof(dto));

            var exercise = await _exerciseRepository.GetByIdAsync(dto.ExerciseId, cancellationToken);
            if (exercise is null)
                throw new InvalidOperationException("Perspective scenario exercise was not found.");
            if (exercise.UserId != userId)
                throw new UnauthorizedAccessException("Exercise does not belong to the current user.");

            var challenge = await _challengeRepository.GetByIdAsync(exercise.ChallengeId, cancellationToken);
            if (challenge is null)
                throw new InvalidOperationException("Perspective scenario challenge was not found.");

            var question = challenge.Questions.FirstOrDefault(q => q.Id == dto.QuestionId);
            if (question is null)
                throw new InvalidOperationException("Question does not belong to exercise challenge.");

            var existingAnswer = exercise.Answers.FirstOrDefault(x => x.QuestionId == dto.QuestionId);

            var conversation = await _conversationRepository.GetByExerciseQuestionAsync(
                userId,
                dto.ExerciseId,
                dto.QuestionId,
                cancellationToken);

            if (conversation is null)
            {
                conversation = new AnswerConversation(
                    Guid.NewGuid(),
                    userId,
                    dto.ExerciseId,
                    dto.QuestionId);

                await _conversationRepository.AddAsync(conversation, cancellationToken);
            }

            if (conversation.IsClosed())
            {
                return BuildClosedConversationResult(exercise, challenge, question, conversation, language);
            }

            if (conversation.HasProcessedIdempotencyKey(dto.IdempotencyKey))
            {
                return BuildCurrentConversationResult(exercise, challenge, question, conversation, language);
            }

            if (exercise.IsCompleted() && existingAnswer is null)
                throw new InvalidOperationException("Exercise has already been completed.");

            var scenarioText = SelectLocalized(challenge.ScenarioText, challenge.ScenarioTextEn, IsEnglish(language));
            var questionText = SelectLocalized(question.QuestionText, question.QuestionTextEn, IsEnglish(language));
            var revealText = SelectLocalized(question.Reveal, question.RevealEn, IsEnglish(language));
            var targetLanguage = ResolveTargetLanguage(language);

            var gradeInput = new PerspectiveScenarioLlmInput(
                scenarioText,
                questionText,
                revealText,
                dto.AnswerText,
                targetLanguage,
                conversation.Turns.ToList());

            var grade = await _llmClient.GradeAnswerAsync(gradeInput, cancellationToken);
            if (onGrade is not null)
                await onGrade(grade, cancellationToken);

            var evaluation = new EvaluationSummary(
                grade.Score,
                grade.Issues,
                grade.Strengths,
                grade.Language);

            conversation.RegisterLearnerAnswer(
                Guid.NewGuid(),
                dto.AnswerText,
                evaluation,
                _dateTimeProvider.UtcNow,
                dto.IdempotencyKey);

            if (grade.Score >= 4)
            {
                await AcceptAnswerAndMaybeCompleteAsync(
                    exercise,
                    challenge,
                    dto,
                    trackInDailySession,
                    cancellationToken);

                conversation.MarkCompleted();
                await _exerciseRepository.UpdateAsync(exercise, cancellationToken);
                await _conversationRepository.UpdateAsync(conversation, cancellationToken);
                await _conversationRepository.SaveChangesAsync(cancellationToken);

                return BuildFinalResult(
                    exercise,
                    challenge,
                    question,
                    conversation,
                    language,
                    PerspectiveScenarioAnswerStatus.Completed,
                    grade,
                    BuildPositiveFeedback(grade, language));
            }

            if (!conversation.CanAskAnotherGuideQuestion())
            {
                await AcceptAnswerAndMaybeCompleteAsync(
                    exercise,
                    challenge,
                    dto,
                    trackInDailySession,
                    cancellationToken);

                conversation.MarkMaxIterationsReached();
                await _exerciseRepository.UpdateAsync(exercise, cancellationToken);
                await _conversationRepository.UpdateAsync(conversation, cancellationToken);
                await _conversationRepository.SaveChangesAsync(cancellationToken);

                return BuildFinalResult(
                    exercise,
                    challenge,
                    question,
                    conversation,
                    language,
                    PerspectiveScenarioAnswerStatus.MaxIterationsReached,
                    grade,
                    BuildConstructiveFeedback(grade, language));
            }

            var guideInput = new PerspectiveScenarioLlmInput(
                scenarioText,
                questionText,
                revealText,
                dto.AnswerText,
                targetLanguage,
                conversation.Turns.ToList());

            var iterationIndex = conversation.GuideIterationCount + 1;
            var guide = onGuideQuestionChunk is null
                ? await _llmClient.GenerateGuideQuestionAsync(guideInput, grade, iterationIndex, cancellationToken)
                : await _llmClient.StreamGuideQuestionAsync(guideInput, grade, iterationIndex, onGuideQuestionChunk, cancellationToken);

            conversation.RegisterGuideQuestion(
                Guid.NewGuid(),
                guide.NextQuestion,
                guide.WhyThisQuestion,
                _dateTimeProvider.UtcNow);

            await _conversationRepository.UpdateAsync(conversation, cancellationToken);
            await _conversationRepository.SaveChangesAsync(cancellationToken);

            return BuildGuidanceResult(exercise, challenge, conversation, grade, guide.NextQuestion);
        }

        private async Task AcceptAnswerAndMaybeCompleteAsync(
            PerspectiveScenarioExercise exercise,
            PerspectiveScenarioChallenge challenge,
            AnswerPerspectiveScenarioQuestionDto dto,
            bool trackInDailySession,
            CancellationToken cancellationToken)
        {
            if (!exercise.IsCompleted())
            {
                exercise.SubmitOrUpdateAnswer(new ScenarioAnswer(dto.QuestionId, dto.AnswerText));
            }

            var totalQuestions = challenge.Questions.Count;
            var answeredQuestionsCount = exercise.Answers.Select(x => x.QuestionId).Distinct().Count();
            var shouldComplete = !exercise.IsCompleted() && answeredQuestionsCount >= totalQuestions;

            if (shouldComplete)
            {
                var completedAt = _dateTimeProvider.UtcNow;
                exercise.MarkSubmitted(completedAt);

                if (trackInDailySession)
                {
                    var dailySession = await GetOrCreateTodaySessionAsync(exercise.UserId, cancellationToken);
                    dailySession.RecordExercise(
                        exercise.Id,
                        ExerciseType.PerspectiveScenario,
                        completedAt);

                    await _dailySessionRepository.UpdateAsync(dailySession, cancellationToken);
                }
            }
        }

        private static AnswerPerspectiveScenarioQuestionResultDto BuildGuidanceResult(
            PerspectiveScenarioExercise exercise,
            PerspectiveScenarioChallenge challenge,
            AnswerConversation conversation,
            PerspectiveScenarioGradeResult grade,
            string guideQuestion)
        {
            var totalQuestions = challenge.Questions.Count;
            var answeredQuestionsCount = exercise.Answers.Select(x => x.QuestionId).Distinct().Count();

            return new AnswerPerspectiveScenarioQuestionResultDto(
                MapExercise(exercise),
                null,
                exercise.IsCompleted(),
                answeredQuestionsCount,
                totalQuestions,
                PerspectiveScenarioAnswerStatus.NeedsGuidance,
                grade.Score,
                grade.Issues,
                grade.Strengths,
                new PerspectiveScenarioGuideQuestionDto(guideQuestion),
                conversation.GuideIterationCount,
                null);
        }

        private static AnswerPerspectiveScenarioQuestionResultDto BuildFinalResult(
            PerspectiveScenarioExercise exercise,
            PerspectiveScenarioChallenge challenge,
            PerspectiveScenarioQuestion question,
            AnswerConversation conversation,
            string? language,
            string status,
            PerspectiveScenarioGradeResult grade,
            string feedback)
        {
            var totalQuestions = challenge.Questions.Count;
            var answeredQuestionsCount = exercise.Answers.Select(x => x.QuestionId).Distinct().Count();

            return new AnswerPerspectiveScenarioQuestionResultDto(
                MapExercise(exercise),
                MapReveal(question, language),
                exercise.IsCompleted(),
                answeredQuestionsCount,
                totalQuestions,
                status,
                grade.Score,
                grade.Issues,
                grade.Strengths,
                null,
                conversation.GuideIterationCount,
                feedback);
        }

        private static AnswerPerspectiveScenarioQuestionResultDto BuildClosedConversationResult(
            PerspectiveScenarioExercise exercise,
            PerspectiveScenarioChallenge challenge,
            PerspectiveScenarioQuestion question,
            AnswerConversation conversation,
            string? language)
        {
            var grade = ToGradeResult(conversation.LastLearnerTurn()?.EvaluationSummary);
            var status = conversation.Status == ConversationStatus.MaxIterationsReached
                ? PerspectiveScenarioAnswerStatus.MaxIterationsReached
                : PerspectiveScenarioAnswerStatus.Completed;

            var feedback = status == PerspectiveScenarioAnswerStatus.MaxIterationsReached
                ? BuildConstructiveFeedback(grade, language)
                : BuildPositiveFeedback(grade, language);

            return BuildFinalResult(
                exercise,
                challenge,
                question,
                conversation,
                language,
                status,
                grade,
                feedback);
        }

        private static AnswerPerspectiveScenarioQuestionResultDto BuildCurrentConversationResult(
            PerspectiveScenarioExercise exercise,
            PerspectiveScenarioChallenge challenge,
            PerspectiveScenarioQuestion question,
            AnswerConversation conversation,
            string? language)
        {
            if (conversation.IsClosed())
                return BuildClosedConversationResult(exercise, challenge, question, conversation, language);

            var grade = ToGradeResult(conversation.LastLearnerTurn()?.EvaluationSummary);
            var lastGuide = conversation.LastGuideTurn();

            if (lastGuide is not null)
            {
                return BuildGuidanceResult(
                    exercise,
                    challenge,
                    conversation,
                    grade,
                    lastGuide.Message);
            }

            var totalQuestions = challenge.Questions.Count;
            var answeredQuestionsCount = exercise.Answers.Select(x => x.QuestionId).Distinct().Count();

            return new AnswerPerspectiveScenarioQuestionResultDto(
                MapExercise(exercise),
                null,
                exercise.IsCompleted(),
                answeredQuestionsCount,
                totalQuestions,
                PerspectiveScenarioAnswerStatus.NeedsGuidance,
                grade.Score,
                grade.Issues,
                grade.Strengths,
                null,
                conversation.GuideIterationCount,
                null);
        }

        private static PerspectiveScenarioGradeResult ToGradeResult(EvaluationSummary? evaluation)
        {
            if (evaluation is null)
            {
                return new PerspectiveScenarioGradeResult(
                    1,
                    Array.Empty<string>(),
                    Array.Empty<string>(),
                    "unknown");
            }

            return new PerspectiveScenarioGradeResult(
                evaluation.Mark,
                evaluation.Issues.ToList(),
                evaluation.Strengths.ToList(),
                evaluation.Language);
        }

        private static string BuildPositiveFeedback(PerspectiveScenarioGradeResult grade, string? language)
        {
            if (IsEnglish(language))
                return grade.Score >= 5
                    ? "Nicely seen. You caught the deeper perspective with care."
                    : "Good work. You understood the main perspective well enough to continue.";

            return grade.Score >= 5
                ? "Baš dobar uvid. Lepo ste uhvatili šta je stajalo iza reakcije."
                : "Dobro ste to razumeli. Možemo dalje.";
        }

        private static string BuildConstructiveFeedback(PerspectiveScenarioGradeResult grade, string? language)
        {
            var mainIssue = grade.Issues.FirstOrDefault();

            if (IsEnglish(language))
            {
                return mainIssue switch
                {
                    "judgmental_or_blaming" => "Take a look at the reveal and try to notice the softer explanation behind the behavior.",
                    "generic_or_vague" => "The reveal adds a more specific layer. Notice what pressure, worry, or meaning was shaping the moment.",
                    "misses_core_feeling" => "The reveal can help you spot the feeling underneath the reaction.",
                    "misses_perspective" => "Compare your answer with the reveal and notice what the situation looked like from their side.",
                    _ => "Read the reveal and notice the piece of perspective that was still missing."
                };
            }

            return mainIssue switch
            {
                "judgmental_or_blaming" => "Pročitajte objašnjenje i probajte da primetite drugi mogući razlog iza tog ponašanja.",
                "generic_or_vague" => "Pokušajte da povežete odgovor sa konkretnim detaljima iz situacije i objašnjenja.",
                "misses_core_feeling" => "Objašnjenje vam može pomoći da prepoznate osećaj koji je bio ispod reakcije.",
                "misses_perspective" => "Pogledajte objašnjenje i proverite šta je toj osobi bilo važno u toj situaciji.",
                _ => "Pročitajte objašnjenje i obratite pažnju na deo perspektive koji je još nedostajao."
            };
        }

        private static string ResolveTargetLanguage(string? language)
        {
            if (string.IsNullOrWhiteSpace(language))
                return "sr";

            return IsEnglish(language) ? "en" : "sr";
        }

        private static void EnsureAnswersMatchChallengeQuestions(
            PerspectiveScenarioChallenge challenge,
            IReadOnlyCollection<SubmitPerspectiveScenarioAnswerItemDto> answers)
        {
            var expectedQuestionIds = challenge.Questions
                .Select(x => x.Id)
                .OrderBy(x => x)
                .ToList();

            var submittedQuestionIds = answers
                .Select(x => x.QuestionId)
                .OrderBy(x => x)
                .ToList();

            if (expectedQuestionIds.Count != submittedQuestionIds.Count)
                throw new InvalidOperationException("All scenario questions must be answered exactly once.");

            if (!expectedQuestionIds.SequenceEqual(submittedQuestionIds))
                throw new InvalidOperationException("Submitted answers do not match the scenario questions.");
        }

        private static PerspectiveScenarioChallengeDto MapChallenge(PerspectiveScenarioChallenge challenge, string? language)
        {
            var isEnglish = IsEnglish(language);
            return new PerspectiveScenarioChallengeDto(
                challenge.Id,
                challenge.ActorCount,
                challenge.Context,
                SelectLocalized(challenge.ScenarioText, challenge.ScenarioTextEn, isEnglish),
                challenge.ChallengeLevel,
                challenge.Questions.OrderBy(x => x.Order).Select(question => MapQuestion(question, language)).ToList());
        }

        private static PerspectiveScenarioPromptDto MapPrompt(PerspectiveScenarioChallenge challenge, string? language)
        {
            var isEnglish = IsEnglish(language);
            return new PerspectiveScenarioPromptDto(
                challenge.Id,
                challenge.ActorCount,
                challenge.Context,
                SelectLocalized(challenge.ScenarioText, challenge.ScenarioTextEn, isEnglish),
                challenge.ChallengeLevel,
                challenge.Questions.OrderBy(x => x.Order).Select(question => MapPromptQuestion(question, language)).ToList());
        }

        private static PerspectiveScenarioQuestionDto MapQuestion(PerspectiveScenarioQuestion question, string? language)
        {
            var isEnglish = IsEnglish(language);
            return new PerspectiveScenarioQuestionDto(
                question.Id,
                question.SkillId,
                question.Order,
                SelectLocalized(question.QuestionText, question.QuestionTextEn, isEnglish),
                SelectLocalized(question.Reveal, question.RevealEn, isEnglish));
        }

        private static PerspectiveScenarioPromptQuestionDto MapPromptQuestion(PerspectiveScenarioQuestion question, string? language)
        {
            var isEnglish = IsEnglish(language);
            return new PerspectiveScenarioPromptQuestionDto(
                question.Id,
                question.SkillId,
                question.Order,
                SelectLocalized(question.QuestionText, question.QuestionTextEn, isEnglish));
        }

        private static PerspectiveScenarioRevealDto MapReveal(PerspectiveScenarioQuestion question, string? language)
        {
            var isEnglish = IsEnglish(language);
            return new PerspectiveScenarioRevealDto(
                question.Id,
                question.Order,
                SelectLocalized(question.Reveal, question.RevealEn, isEnglish));
        }

        private static PerspectiveScenarioExerciseDto MapExercise(PerspectiveScenarioExercise exercise)
        {
            return new PerspectiveScenarioExerciseDto(
                exercise.Id,
                exercise.UserId,
                exercise.ChallengeId,
                exercise.Answers.Select(x => new ScenarioAnswerDto(x.QuestionId, x.AnswerText)).ToList(),
                exercise.SubmittedAt,
                exercise.IsCompleted(),
                exercise.IsOnboardingHookRun);
        }

        private static bool IsEnglish(string? language)
        {
            return !string.IsNullOrWhiteSpace(language)
                && language.StartsWith("en", StringComparison.OrdinalIgnoreCase);
        }

        private static string SelectLocalized(string sr, string? en, bool isEnglish)
        {
            if (isEnglish && !string.IsNullOrWhiteSpace(en))
                return en;

            return sr;
        }

        private async Task<DailySession> GetOrCreateTodaySessionAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var today = _dateTimeProvider.BusinessDate;

            var session = await _dailySessionRepository.GetByUserAndDateAsync(
                userId,
                today,
                cancellationToken);

            if (session is not null)
                return session;

            session = new DailySession(Guid.NewGuid(), userId, today);
            await _dailySessionRepository.AddAsync(session, cancellationToken);
            await _dailySessionRepository.SaveChangesAsync(cancellationToken);

            return session;
        }
    }
}
