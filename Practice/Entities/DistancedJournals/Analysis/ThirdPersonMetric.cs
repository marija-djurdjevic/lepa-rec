using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.DistancedJournals.Analysis
{
    public class ThirdPersonMetric
    {
        public int FirstPersonCount { get; }
        public int ThirdPersonCount { get; }
        public int Score { get; }

        public IReadOnlyCollection<string> FirstPersonMarkers { get; }
        public IReadOnlyCollection<string> ThirdPersonMarkers { get; }

        public ThirdPersonMetric(
            int firstPersonCount,
            int thirdPersonCount,
            int score,
            IReadOnlyCollection<string> firstPersonMarkers,
            IReadOnlyCollection<string> thirdPersonMarkers)
        {
            FirstPersonCount = firstPersonCount;
            ThirdPersonCount = thirdPersonCount;
            Score = score;
            FirstPersonMarkers = firstPersonMarkers;
            ThirdPersonMarkers = thirdPersonMarkers;
        }
    }
}
