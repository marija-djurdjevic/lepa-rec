using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.DistancedJournals.Analysis
{
    public class DistancedJournalFeedback
    {
        public ThirdPersonFeedbackType FeedbackType { get; }

        public DistancedJournalFeedback(ThirdPersonFeedbackType feedbackType)
        {
            FeedbackType = feedbackType;
        }
    }
}
