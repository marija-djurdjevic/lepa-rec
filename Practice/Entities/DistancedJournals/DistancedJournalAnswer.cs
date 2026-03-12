using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.DistancedJournals
{
    public class DistancedJournalAnswer
    {
        public string MainAnswer { get; private set; }
        public string FollowUpAnswer { get; private set; }
        public string? Reflection { get; private set; }
        public DateTime SubmittedAt { get; private set; }

        private DistancedJournalAnswer()
        {
            MainAnswer = string.Empty;
            FollowUpAnswer = string.Empty;
        }

        public DistancedJournalAnswer(
            string mainAnswer,
            string followUpAnswer,
            string? reflection,
            DateTime submittedAt)
        {
            if (string.IsNullOrWhiteSpace(mainAnswer))
                throw new ArgumentException("Main answer must be provided.");

            if (string.IsNullOrWhiteSpace(followUpAnswer))
                throw new ArgumentException("Follow-up answer must be provided.");

            MainAnswer = mainAnswer.Trim();
            FollowUpAnswer = followUpAnswer.Trim();
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
