using AngularNetBase.Practice.Entities.AffirmationValues;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.GrowthMessages;
using AngularNetBase.Practice.Entities.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace AngularNetBase.Practice.Infrastructure
{
    public class PracticeContext : DbContext
    {
        public DbSet<DailySession> DailySessions { get; set; }
        public DbSet<AffirmationValue> AffirmationValues { get; set; }
        public DbSet<GrowthMessage> GrowthMessages { get; set; }
        public DbSet<DistancedJournalChallenge> DistancedJournalChallenges { get; set; }
        public DbSet<DistancedJournalExercise> DistancedJournalExercises { get; set; }

        public PracticeContext(DbContextOptions<PracticeContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("practice");

            base.OnModelCreating(modelBuilder);

            var guidListComparer = new ValueComparer<List<Guid>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())) : 0,
                c => c != null ? c.ToList() : new List<Guid>());

            modelBuilder.Entity<DailySession>(entity =>
            {
                entity.ToTable("DailySessions");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasIndex(e => new { e.UserId, e.Date })
                    .IsUnique();

                entity.OwnsOne(e => e.PrimerResult, primer =>
                {
                    primer.Property(p => p.PresentedStatementIds)
                        .HasColumnName("PrimerPresentedStatementIds")
                        .HasConversion(
                            v => string.Join(',', v),
                            v => string.IsNullOrEmpty(v)
                            ? new List<Guid>()
                            : v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList()
                            )
                            .Metadata.SetValueComparer(guidListComparer);

                    primer.Property(p => p.SelectedStatementId)
                        .HasColumnName("PrimerSelectedStatementId");

                    primer.Property(p => p.GrowthMessageId)
                        .HasColumnName("PrimerGrowthMessageId");

                    primer.Property(p => p.IsSkipped)
                        .HasColumnName("PrimerIsSkipped");

                    primer.Property(p => p.Timestamp)
                        .HasColumnName("PrimerTimestamp");
                });

                entity.HasMany(e => e.Events)
                    .WithOne()
                    .HasForeignKey("DailySessionId")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Navigation(e => e.Events)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<SessionEvent>(entity =>
            {
                entity.ToTable("SessionEvents");

                entity.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                entity.HasKey("Id");

                entity.Property(e => e.Timestamp)
                    .IsRequired();

                entity.Property<Guid>("DailySessionId")
                    .IsRequired();

                entity.HasDiscriminator<string>("Discriminator")
                    .HasValue<GeneralEvent>("General")
                    .HasValue<ExerciseRecord>("Exercise");
            });

            modelBuilder.Entity<GeneralEvent>(entity =>
            {
                entity.Property(e => e.EventType)
                    .HasMaxLength(100)
                    .IsRequired();
            });

            modelBuilder.Entity<ExerciseRecord>(entity =>
            {
                entity.Property(e => e.ExerciseId)
                    .IsRequired();

                entity.Property(e => e.Type)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();
            });

            modelBuilder.Entity<AffirmationValue>(entity =>
            {
                entity.ToTable("AffirmationValues");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasMany(e => e.Statements)
                    .WithOne()
                    .HasForeignKey(s => s.AffirmationValueId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Navigation(e => e.Statements)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<ValueStatement>(entity =>
            {
                entity.ToTable("ValueStatements");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.IsActive)
                    .IsRequired();
            });

            modelBuilder.Entity<GrowthMessage>(entity =>
            {
                entity.ToTable("GrowthMessages");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.IsActive)
                    .IsRequired();
            });

            modelBuilder.Entity<DistancedJournalChallenge>(entity =>
            {
                entity.ToTable("DistancedJournalChallenges");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.FollowUpQuestion)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.ChallengeLevel)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();
            });

            modelBuilder.Entity<DistancedJournalExercise>(entity =>
            {
                entity.ToTable("DistancedJournalExercises");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.ChallengeId)
                    .IsRequired();

                entity.HasIndex(e => new { e.UserId, e.ChallengeId });

                entity.OwnsOne(e => e.Answer, answer =>
                {
                    answer.Property(a => a.MainAnswer)
                        .HasColumnName("MainAnswer")
                        .HasMaxLength(3000);

                    answer.Property(a => a.FollowUpAnswer)
                        .HasColumnName("FollowUpAnswer")
                        .HasMaxLength(3000);

                    answer.Property(a => a.Reflection)
                        .HasColumnName("Reflection")
                        .HasMaxLength(3000);

                    answer.Property(a => a.SubmittedAt)
                        .HasColumnName("SubmittedAt");
                });
            });
        }
    }
}