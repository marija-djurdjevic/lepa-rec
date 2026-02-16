using AngularNetBase.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AngularNetBase.Identity.Persistence;

public static class IdentitySeeder
{
    private const string Email = "admin@test.com";
    private const string Password = "Admin123!";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await userManager.FindByEmailAsync(Email) is not null)
            return;

        var user = new ApplicationUser
        {
            UserName = Email,
            Email = Email,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(user, Password);
    }
}
