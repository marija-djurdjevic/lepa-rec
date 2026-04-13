using AngularNetBase.Practice.Entities.AffirmationValues;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.GrowthMessages;
using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.Sessions;
using AngularNetBase.Practice.Entities.Skills;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using AngularNetBase.Practice.Entities.Scheduling;
using AngularNetBase.Shared.Core.Domain;
using AngularNetBase.Practice.Infrastructure.Extensions;

namespace AngularNetBase.Practice.Infrastructure
{
    public class PracticeContext : DbContext
    {
        public DbSet<DailySession> DailySessions { get; set; }
        public DbSet<AffirmationValue> AffirmationValues { get; set; }
        public DbSet<GrowthMessage> GrowthMessages { get; set; }
        public DbSet<DistancedJournalChallenge> DistancedJournalChallenges { get; set; }
        public DbSet<DistancedJournalExercise> DistancedJournalExercises { get; set; }
        public DbSet<PerspectiveScenarioChallenge> PerspectiveScenarioChallenges { get; set; }
        public DbSet<PerspectiveScenarioExercise> PerspectiveScenarioExercises { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<SkillMastery> SkillMasteries { get; set; }
        public DbSet<DailyChallengeAssignment> DailyChallengeAssignments { get; set; }

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


            modelBuilder.Entity<DailyChallengeAssignment>(entity =>
            {
                entity.ToTable("DailyChallengeAssignments");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(e => e.DistancedJournalChallengeId)
                    .IsRequired();

                entity.Property(e => e.DistancedJournalChallengeId2)
                    .IsRequired();

                entity.Property(e => e.PerspectiveScenarioChallengeId)
                    .IsRequired();

                entity.Property(e => e.PerspectiveScenarioChallengeId2)
                    .IsRequired();

                entity.Property(e => e.AssignedAt)
                    .IsRequired();

                entity.HasIndex(e => e.Date)
                    .IsUnique();
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

                entity.Property(e => e.Type)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

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

                entity.OwnsMany(e => e.Photos, photo =>
                {
                    photo.ToTable("DistancedJournalPhotos");

                    photo.WithOwner().HasForeignKey("DistancedJournalExerciseId");

                    photo.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    photo.HasKey("Id");

                    photo.Property(p => p.ObjectKey)
                        .IsRequired()
                        .HasMaxLength(500);

                    photo.Property(p => p.FileName)
                        .IsRequired()
                        .HasMaxLength(255);

                    photo.Property(p => p.ContentType)
                        .IsRequired()
                        .HasMaxLength(100);

                    photo.Property(p => p.SizeBytes)
                        .IsRequired();

                    photo.Property(p => p.UploadedAt)
                        .IsRequired();
                });

                entity.Navigation(e => e.Photos)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("Skills");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.OwnsMany(e => e.Levels, level =>
                {
                    level.ToTable("SkillLevelDefinitions");

                    level.WithOwner().HasForeignKey("SkillId");

                    level.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    level.HasKey("Id");

                    level.Property(l => l.LevelNumber)
                        .IsRequired();

                    level.Property(l => l.Title)
                        .IsRequired()
                        .HasMaxLength(200);

                    level.Property(l => l.Description)
                        .IsRequired()
                        .HasMaxLength(1000);
                });

                entity.Navigation(e => e.Levels)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<SkillMastery>(entity =>
            {
                entity.ToTable("SkillMasteries");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.SkillId)
                    .IsRequired();

                entity.Property(e => e.CurrentLevel)
                    .IsRequired();

                entity.HasIndex(e => new { e.UserId, e.SkillId })
                    .IsUnique();
            });

            modelBuilder.Entity<PerspectiveScenarioChallenge>(entity =>
            {
                entity.ToTable("PerspectiveScenarioChallenges");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.ScenarioText)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Reveal)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Context)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.ActorCount)
                    .IsRequired();

                entity.Property(e => e.ChallengeLevel)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasMany(e => e.Questions)
                    .WithOne()
                    .HasForeignKey(q => q.PerspectiveScenarioChallengeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Navigation(e => e.Questions)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<PerspectiveScenarioQuestion>(entity =>
            {
                entity.ToTable("PerspectiveScenarioQuestions");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.SkillId)
                    .IsRequired();

                entity.Property(e => e.QuestionText)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<PerspectiveScenarioExercise>(entity =>
            {
                entity.ToTable("PerspectiveScenarioExercises");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.ChallengeId)
                    .IsRequired();

                entity.Property(e => e.SubmittedAt);

                entity.HasIndex(e => new { e.UserId, e.ChallengeId });

                entity.OwnsMany(e => e.Answers, answer =>
                {
                    answer.ToTable("PerspectiveScenarioAnswers");

                    answer.WithOwner().HasForeignKey("PerspectiveScenarioExerciseId");

                    answer.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    answer.HasKey("Id");

                    answer.Property(a => a.QuestionId)
                        .IsRequired();

                    answer.Property(a => a.AnswerText)
                        .IsRequired()
                        .HasMaxLength(3000);
                });

                entity.Navigation(e => e.Answers)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            ApplyXminConcurrency(modelBuilder);
        }

        private static void ApplyXminConcurrency(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                if (clrType is null)
                    continue;

                if (!typeof(Entity<Guid>).IsAssignableFrom(clrType))
                    continue;

                var builder = modelBuilder.Entity(clrType);
                builder.Ignore(nameof(Entity<Guid>.Version));
                builder.UseXminAsConcurrencyToken();
            }
        }
    }
}
