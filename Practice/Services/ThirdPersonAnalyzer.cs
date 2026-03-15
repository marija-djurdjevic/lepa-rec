using AngularNetBase.Practice.Entities.DistancedJournals.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public class ThirdPersonAnalyzer : IThirdPersonAnalyzer
    {
        private static readonly string[] SerbianFirstPerson =
        {
            "ja", "meni", "mene", "moj", "moja", "moje", "mi", "me", "sam"
        };

        private static readonly string[] SerbianThirdPerson =
        {
            "on", "ona", "oni", "njemu", "njoj", "njega", "nju", "sebe", "sebi"
        };

        private static readonly string[] EnglishFirstPerson =
        {
            "i", "me", "my", "mine", "myself"
        };

        private static readonly string[] EnglishThirdPerson =
        {
            "he", "she", "they", "him", "her", "them", "his", "hers", "themselves"
        };

        public ThirdPersonMetric Analyze(string text, string language, string? firstName = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new ThirdPersonMetric(
                    0,
                    0,
                    0,
                    Array.Empty<string>(),
                    Array.Empty<string>());
            }

            var words = text
                .ToLower()
                .Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '\n', '\r', '(', ')', '"' }, StringSplitOptions.RemoveEmptyEntries);

            var firstMarkers = GetFirstPersonMarkers(language);
            var thirdMarkers = GetThirdPersonMarkers(language);

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                thirdMarkers = thirdMarkers.Append(firstName.ToLower()).ToArray();
            }

            var firstHits = words.Where(w => firstMarkers.Contains(w)).ToList();
            var thirdHits = words.Where(w => thirdMarkers.Contains(w)).ToList();

            var firstCount = firstHits.Count;
            var thirdCount = thirdHits.Count;
            var score = thirdCount - firstCount;

            return new ThirdPersonMetric(
                firstCount,
                thirdCount,
                score,
                firstHits,
                thirdHits);
        }

        public DistancedJournalFeedback GetFeedback(ThirdPersonMetric metric)
        {
            if (metric.Score >= 2)
            {
                return new DistancedJournalFeedback(ThirdPersonFeedbackType.GoodDistancing);
            }

            if (metric.Score >= -1)
            {
                return new DistancedJournalFeedback(ThirdPersonFeedbackType.MixedDistancing);
            }

            return new DistancedJournalFeedback(ThirdPersonFeedbackType.NeedsMoreDistancing);
        }

        private static string[] GetFirstPersonMarkers(string language)
        {
            return language switch
            {
                "sr" => SerbianFirstPerson,
                "en" => EnglishFirstPerson,
                _ => EnglishFirstPerson
            };
        }

        private static string[] GetThirdPersonMarkers(string language)
        {
            return language switch
            {
                "sr" => SerbianThirdPerson,
                "en" => EnglishThirdPerson,
                _ => EnglishThirdPerson
            };
        }
    }
}
