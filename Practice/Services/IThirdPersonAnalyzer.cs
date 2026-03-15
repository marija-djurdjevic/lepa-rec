using AngularNetBase.Practice.Entities.DistancedJournals.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public interface IThirdPersonAnalyzer
    {
        ThirdPersonMetric Analyze(string text, string language, string? firstName = null);
        DistancedJournalFeedback GetFeedback(ThirdPersonMetric metric);
    }
}
