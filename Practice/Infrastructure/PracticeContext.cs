using AngularNetBase.Practice.Entities.Sessions;
using Microsoft.EntityFrameworkCore;
using System;

namespace AngularNetBase.Practice.Infrastructure
{
    public class PracticeContext : DbContext
    {
        public DbSet<DailySession> DailySessions { get; set; }

        public PracticeContext(DbContextOptions<PracticeContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("practice");

            base.OnModelCreating(modelBuilder);

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
                    primer.Property(p => p.AffirmationValueId)
                        .HasColumnName("PrimerAffirmationId");

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
        }
    }
}