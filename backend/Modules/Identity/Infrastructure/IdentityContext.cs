using AngularNetBase.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AngularNetBase.Identity.Infrastructure;

public class IdentityContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<OnboardingSession> OnboardingSessions => Set<OnboardingSession>();

    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("identity");

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.Token).IsUnique();
            entity.Property(r => r.Token).IsRequired();
            entity.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OnboardingSession>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.PreferredLanguage).HasMaxLength(10);
            entity.Property(x => x.HookType).HasMaxLength(50);
            entity.Property(x => x.DeviceFingerprint).HasMaxLength(200);
            entity.Property(x => x.PerspectiveAnswersJson).HasColumnType("text");
            entity.HasIndex(x => x.ExpiresAt);
            entity.HasIndex(x => x.UsedAt);
        });
    }
}
