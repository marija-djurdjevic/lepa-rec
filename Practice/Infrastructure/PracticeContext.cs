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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DailySession>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.OwnsOne(e => e.PrimerResult, primer =>
                {
                    primer.Property(p => p.AffirmationValueId).HasColumnName("PrimerAffirmationId");
                    primer.Property(p => p.GrowthMessageId).HasColumnName("PrimerGrowthMessageId");
                    primer.Property(p => p.IsSkipped).HasColumnName("PrimerIsSkipped");
                    primer.Property(p => p.Timestamp).HasColumnName("PrimerTimestamp");
                });

                entity.HasMany(e => e.Events)
                      .WithOne() 
                      .HasForeignKey("DailySessionId") 
                      .OnDelete(DeleteBehavior.Cascade); 
            });

            modelBuilder.Entity<SessionEvent>(entity =>
            {
                entity.ToTable("SessionEvents");

                entity.Property<Guid>("Id").ValueGeneratedOnAdd();
                entity.HasKey("Id");

                entity.HasDiscriminator<string>("EventType")
                      .HasValue<GeneralEvent>("General")
                      .HasValue<ExerciseRecord>("Exercise");
            });
        }
    }
}