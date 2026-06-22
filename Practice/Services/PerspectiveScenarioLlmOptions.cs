namespace AngularNetBase.Practice.Services
{
    public class PerspectiveScenarioLlmOptions
    {
        public string? ApiKey { get; set; }
        public string Model { get; set; } = "gpt-4o-mini";
        public string GraderModel { get; set; } = "gpt-4o-mini";
        public string GuideModel { get; set; } = "gpt-4o";
        public string BaseUrl { get; set; } = "https://api.openai.com/v1";
    }
}
