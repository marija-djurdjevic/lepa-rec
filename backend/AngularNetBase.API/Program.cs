using AngularNetBase.Identity;

namespace AngularNetBase.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddIdentityModule(builder.Configuration);
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        await app.UseIdentityModuleAsync();

        if (app.Configuration.GetValue<bool>("RunMigrationsOnly"))
        {
            return;
        }

        app.UseDefaultFiles();
        app.MapStaticAssets();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseHttpsRedirection();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
