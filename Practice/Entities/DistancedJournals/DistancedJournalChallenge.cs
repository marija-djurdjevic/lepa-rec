using AngularNetBase.Shared.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.DistancedJournals
{
    public class DistancedJournalChallenge : Entity<Guid>
    {
        private readonly List<DistancedJournalQuestion> _questions = new();

        public string Theme { get; private set; } = string.Empty;
        public string Content { get; private set; } = string.Empty;
        public string? ContentEn { get; private set; }
        public string FollowUpQuestion { get; private set; } = string.Empty;
        public string? FollowUpQuestionEn { get; private set; }
        public ChallengeLevel ChallengeLevel { get; private set; }
        public DistancedJournalVariant Variant { get; private set; }
        public DistancedJournalPhase Phase { get; private set; }
        public Guid? SkillId { get; private set; }
        public bool IsOnboardingHook { get; private set; }
        public string? OnboardingHookKey { get; private set; }
        public IReadOnlyCollection<DistancedJournalQuestion> Questions => _questions;

        private DistancedJournalChallenge() : base()
        {
            Theme = string.Empty;
            Content = string.Empty;
            FollowUpQuestion = string.Empty;
        }

        public DistancedJournalChallenge(
            Guid id,
            string content,
            string followUpQuestion,
            ChallengeLevel challengeLevel,
            Guid? skillId = null,
            string? contentEn = null,
            string? followUpQuestionEn = null,
            bool isOnboardingHook = false,
            string? onboardingHookKey = null)
            : base(id)
        {
            Initialize(
                id,
                content,
                followUpQuestion,
                challengeLevel,
                skillId,
                contentEn,
                followUpQuestionEn,
                isOnboardingHook,
                onboardingHookKey,
                theme: null,
                variant: DistancedJournalVariant.A,
                phase: DistancedJournalPhase.Single);

            AddQuestion(Guid.NewGuid(), DistancedJournalQuestionKind.Opening, 1, content, skillId, contentEn);
            AddQuestion(Guid.NewGuid(), DistancedJournalQuestionKind.FollowUp, 2, followUpQuestion, null, followUpQuestionEn);
        }

        public DistancedJournalChallenge(
            Guid id,
            string theme,
            string content,
            string openingQuestion,
            string followUpQuestion,
            string? reflectionQuestion,
            ChallengeLevel challengeLevel,
            DistancedJournalVariant variant,
            DistancedJournalPhase phase,
            Guid? openingSkillId = null,
            Guid? followUpSkillId = null,
            Guid? reflectionSkillId = null,
            string? contentEn = null,
            string? openingQuestionEn = null,
            string? followUpQuestionEn = null,
            string? reflectionQuestionEn = null,
            bool isOnboardingHook = false,
            string? onboardingHookKey = null)
            : base(id)
        {
            Initialize(
                id,
                content,
                followUpQuestion,
                challengeLevel,
                openingSkillId,
                contentEn,
                followUpQuestionEn,
                isOnboardingHook,
                onboardingHookKey,
                theme,
                variant,
                phase);

            AddQuestion(Guid.NewGuid(), DistancedJournalQuestionKind.Opening, 1, openingQuestion, openingSkillId, openingQuestionEn);
            AddQuestion(Guid.NewGuid(), DistancedJournalQuestionKind.FollowUp, 2, followUpQuestion, followUpSkillId, followUpQuestionEn);

            if (!string.IsNullOrWhiteSpace(reflectionQuestion))
                AddQuestion(Guid.NewGuid(), DistancedJournalQuestionKind.Reflection, 3, reflectionQuestion, reflectionSkillId, reflectionQuestionEn);
        }

        private void Initialize(
            Guid id,
            string content,
            string followUpQuestion,
            ChallengeLevel challengeLevel,
            Guid? skillId,
            string? contentEn,
            string? followUpQuestionEn,
            bool isOnboardingHook,
            string? onboardingHookKey,
            string? theme,
            DistancedJournalVariant variant,
            DistancedJournalPhase phase)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be a valid GUID.");

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content must be provided.");

            if (string.IsNullOrWhiteSpace(followUpQuestion))
                throw new ArgumentException("Follow-up question must be provided.");

            Theme = string.IsNullOrWhiteSpace(theme) ? content.Trim() : theme.Trim();
            Content = content.Trim();
            ContentEn = NormalizeOptional(contentEn);
            FollowUpQuestion = followUpQuestion.Trim();
            FollowUpQuestionEn = NormalizeOptional(followUpQuestionEn);
            ChallengeLevel = challengeLevel;
            Variant = variant;
            Phase = phase;
            SkillId = skillId;
            IsOnboardingHook = isOnboardingHook;
            OnboardingHookKey = NormalizeOptional(onboardingHookKey);
        }

        public void Update(
            string content,
            string followUpQuestion,
            ChallengeLevel challengeLevel,
            Guid? skillId = null,
            string? contentEn = null,
            string? followUpQuestionEn = null)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content must be provided.");

            if (string.IsNullOrWhiteSpace(followUpQuestion))
                throw new ArgumentException("Follow-up question must be provided.");

            Content = content.Trim();
            ContentEn = NormalizeOptional(contentEn);
            FollowUpQuestion = followUpQuestion.Trim();
            FollowUpQuestionEn = NormalizeOptional(followUpQuestionEn);
            ChallengeLevel = challengeLevel;
            SkillId = skillId;
        }

        public DistancedJournalQuestion AddQuestion(
            Guid id,
            DistancedJournalQuestionKind kind,
            int order,
            string text,
            Guid? skillId = null,
            string? textEn = null)
        {
            if (_questions.Any(x => x.Kind == kind))
                throw new InvalidOperationException($"A {kind} question already exists for this challenge.");

            var question = new DistancedJournalQuestion(id, Id, kind, order, text, skillId, textEn);
            _questions.Add(question);
            return question;
        }

        public void MarkAsOnboardingHook(string hookKey)
        {
            if (string.IsNullOrWhiteSpace(hookKey))
                throw new ArgumentException("Hook key must be provided.", nameof(hookKey));

            IsOnboardingHook = true;
            OnboardingHookKey = hookKey.Trim();
        }

        private static string? NormalizeOptional(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim();
        }
    }
}
