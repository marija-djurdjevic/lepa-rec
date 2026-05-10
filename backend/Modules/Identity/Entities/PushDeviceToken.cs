namespace AngularNetBase.Identity.Entities;

public class PushDeviceToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;
    public string Token { get; set; } = string.Empty;
    public string Platform { get; set; } = "unknown";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
