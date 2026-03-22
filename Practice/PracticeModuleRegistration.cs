using AngularNetBase.Practice.Entities.AffirmationValues;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.GrowthMessages;
using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.Sessions;
using AngularNetBase.Practice.Entities.Skills;
using AngularNetBase.Practice.Infrastructure;
using AngularNetBase.Practice.Infrastructure.Repositories;
using AngularNetBase.Practice.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Practice.Services;

namespace AngularNetBase.Practice;

public static class PracticeModuleRegistration
{
    public static IServiceCollection AddPracticeModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PracticeContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddScoped<IAffirmationValueRepository, AffirmationValueRepository>();
        services.AddScoped<IAffirmationValueService, AffirmationValueService>();
        services.AddScoped<IGrowthMessageRepository, GrowthMessageRepository>();
        services.AddScoped<IGrowthMessageService, GrowthMessageService>();
        services.AddScoped<IDistancedJournalService, DistancedJournalService>();
        services.AddScoped<IDistancedJournalExerciseRepository, DistancedJournalExerciseRepository>();
        services.AddScoped<IDistancedJournalChallengeRepository, DistancedJournalChallengeRepository>();
        services.AddScoped<IThirdPersonAnalyzer, ThirdPersonAnalyzer>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<IPerspectiveScenarioChallengeRepository, PerspectiveScenarioChallengeRepository>();
        services.AddScoped<IPerspectiveScenarioExerciseRepository, PerspectiveScenarioExerciseRepository>();
        services.AddScoped<IPerspectiveScenarioService, PerspectiveScenarioService>();

        return services;
    }

    public static async Task<WebApplication> UsePracticeModuleAsync(this WebApplication app)
    {
        var runMigrations = app.Environment.IsDevelopment()
            || app.Configuration.GetValue<bool>("RunMigrationsOnly");

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PracticeContext>();

        if (runMigrations)
        {
            await db.Database.MigrateAsync();
        }

        if (app.Environment.IsDevelopment())
        {
            await PracticeSeeder.SeedAsync(db);
        }

        return app;
    }
}
