using System.Text;
using AngularNetBase.Identity.Entities;
using AngularNetBase.Identity.Infrastructure;
using AngularNetBase.Identity.Services;
using AngularNetBase.Shared.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace AngularNetBase.Identity;

public static class IdentityModuleRegistration
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        services.AddDbContext<IdentityContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<IdentityContext>();

        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        services.AddScoped<AuthService>();
        services.AddScoped<IUserProfileReader, UserProfileReader>();

        return services;
    }

    public static async Task<WebApplication> UseIdentityModuleAsync(this WebApplication app)
    {
        var runMigrations = app.Environment.IsDevelopment()
            || app.Configuration.GetValue<bool>("RunMigrationsOnly");

        if (runMigrations)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            await db.Database.MigrateAsync();
        }

        if (app.Environment.IsDevelopment())
        {
            await IdentitySeeder.SeedAsync(app.Services);
        }

        return app;
    }
}
