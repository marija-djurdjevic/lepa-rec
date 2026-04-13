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
    }
}
