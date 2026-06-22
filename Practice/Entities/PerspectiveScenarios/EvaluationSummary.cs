namespace AngularNetBase.Practice.Entities.PerspectiveScenarios
{
    public class EvaluationSummary
    {
        private readonly List<string> _issues = new();
        private readonly List<string> _strengths = new();

        public int Mark { get; private set; }
        public IReadOnlyCollection<string> Issues => _issues;
        public IReadOnlyCollection<string> Strengths => _strengths;
        public string Language { get; private set; } = "unknown";

        private EvaluationSummary() { }

        public EvaluationSummary(
            int mark,
            IEnumerable<string>? issues,
            IEnumerable<string>? strengths,
            string? language)
        {
            if (mark < 1 || mark > 5)
                throw new ArgumentOutOfRangeException(nameof(mark), "Mark must be between 1 and 5.");

            Mark = mark;
            _issues.AddRange(NormalizeItems(issues));
            _strengths.AddRange(NormalizeItems(strengths));
            Language = NormalizeLanguage(language);
        }

        private static IEnumerable<string> NormalizeItems(IEnumerable<string>? items)
        {
            return items?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
                ?? Enumerable.Empty<string>();
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
    }
}
