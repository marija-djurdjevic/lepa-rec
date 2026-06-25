namespace AngularNetBase.Practice.Services
{
    public class DistancedJournalLlmOptions
    {
        public string? ApiKey { get; set; }
        public string Model { get; set; } = "gpt-5.4-mini";
        public string BaseUrl { get; set; } = "https://api.openai.com/v1";
    }
}
