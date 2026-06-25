namespace AngularNetBase.Practice.Services
{
    public class PerspectiveScenarioLlmOptions
    {
        public string? ApiKey { get; set; }
        public string Model { get; set; } = "gpt-5.4-mini";
        public string GraderModel { get; set; } = "gpt-5.4-mini";
        public string GuideModel { get; set; } = "gpt-5.4-mini";
        public string BaseUrl { get; set; } = "https://api.openai.com/v1";
        public double Temperature { get; set; } = 0.2;
    }
}
