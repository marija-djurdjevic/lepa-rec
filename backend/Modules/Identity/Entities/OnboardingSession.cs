namespace AngularNetBase.Identity.Entities;

public class OnboardingSession
{
    public Guid Id { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public string? DeviceFingerprint { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? HookType { get; set; }
    public Guid? HookChallengeId { get; set; }
    public bool HookExerciseCompleted { get; set; }

    public Guid? DistancedExerciseId { get; set; }
    public DateTime? DistancedSessionDate { get; set; }
    public string? DistancedMainAnswer { get; set; }
    public string? DistancedFollowUpAnswer { get; set; }
    public string? DistancedReflection { get; set; }

    public Guid? PerspectiveExerciseId { get; set; }
    public string? PerspectiveAnswersJson { get; set; }
    public string? PerspectiveLang { get; set; }
}
