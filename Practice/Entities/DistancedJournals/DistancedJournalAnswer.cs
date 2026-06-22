using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.DistancedJournals
{
    public class DistancedJournalAnswer
    {
        public string? MainAnswer { get; private set; }
        public string? FollowUpAnswer { get; private set; }
        public string? Reflection { get; private set; }
        public string? GeneratedReflectionQuestion { get; private set; }
        public string? GeneratedReflectionAnswer { get; private set; }
        public DateTime SubmittedAt { get; private set; }

        private DistancedJournalAnswer()
        {
        }

        public DistancedJournalAnswer(
            string? mainAnswer,
            string? followUpAnswer,
            string? reflection,
            DateTime submittedAt)
        {
            var hasMain = !string.IsNullOrWhiteSpace(mainAnswer);
            var hasFollowUp = !string.IsNullOrWhiteSpace(followUpAnswer);

            if (hasMain != hasFollowUp)
                throw new ArgumentException("Main and follow-up answers must be provided together.");

            MainAnswer = hasMain ? mainAnswer!.Trim() : null;
            FollowUpAnswer = hasFollowUp ? followUpAnswer!.Trim() : null;
            Reflection = string.IsNullOrWhiteSpace(reflection) ? null : reflection.Trim();
            SubmittedAt = submittedAt;
        }

        public void AddReflection(string reflection)
        {
            if (string.IsNullOrWhiteSpace(reflection))
                throw new ArgumentException("Reflection must be provided.");

            Reflection = reflection.Trim();
        }

        public void SetGeneratedReflectionQuestion(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new ArgumentException("Generated reflection question must be provided.");

            GeneratedReflectionQuestion = question.Trim();
        }

        public void AddGeneratedReflectionAnswer(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                throw new ArgumentException("Generated reflection answer must be provided.");

            if (string.IsNullOrWhiteSpace(GeneratedReflectionQuestion))
                throw new InvalidOperationException("Cannot answer a generated reflection before a question exists.");

            GeneratedReflectionAnswer = answer.Trim();
        }
    }
}
