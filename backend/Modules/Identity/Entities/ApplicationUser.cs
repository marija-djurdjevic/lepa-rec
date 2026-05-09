using Microsoft.AspNetCore.Identity;

namespace AngularNetBase.Identity.Entities;

public class ApplicationUser : IdentityUser<Guid> 
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "en";
    public bool NotificationEnabled { get; set; }
    public string? NotificationTimeLocal { get; set; }
    public string? TimeZoneId { get; set; }
    public string? HookType { get; set; }
    public Guid? HookChallengeId { get; set; }
    public bool OnboardingCompleted { get; set; }
    public DateTime? OnboardingCompletedAt { get; set; }
}
