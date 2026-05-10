namespace AngularNetBase.Identity.Entities;

public class PushReminderDispatch
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;
    public DateOnly LocalDate { get; set; }
    public DateTime SentAtUtc { get; set; }
}
