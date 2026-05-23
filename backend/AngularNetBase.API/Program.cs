using AngularNetBase.Identity;
using AngularNetBase.Practice;

namespace AngularNetBase.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddIdentityModule(builder.Configuration);
        builder.Services.AddPracticeModule(builder.Configuration);
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        await app.UseIdentityModuleAsync();
        await app.UsePracticeModuleAsync();

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

        app.MapGet("/privacy", () =>
        {
            const string privacyHtml = """
                <!doctype html>
                <html lang="en">
                <head>
                  <meta charset="utf-8">
                  <meta name="viewport" content="width=device-width, initial-scale=1">
                  <meta name="robots" content="index,follow">
                  <title>Privacy Policy - Sagledaj</title>
                  <style>
                    :root { color-scheme: light; }
                    body {
                      margin: 0;
                      font-family: Arial, sans-serif;
                      line-height: 1.6;
                      color: #1a1a1a;
                      background: #f7f7f7;
                    }
                    main {
                      max-width: 860px;
                      margin: 0 auto;
                      padding: 24px 16px 48px;
                      background: #ffffff;
                    }
                    h1, h2 { line-height: 1.25; }
                    h1 { margin-top: 0; }
                    ul { padding-left: 24px; }
                    p { margin: 0 0 12px; }
                  </style>
                </head>
                <body>
                  <main>
                    <h1>Privacy Policy for Sagledaj</h1>
                    <p><strong>Effective date:</strong> 2026-05-23</p>
                    <p><strong>Last updated:</strong> 2026-05-23</p>

                    <p>Sagledaj ("we", "our", "us") respects your privacy. This Privacy Policy explains what data we collect, how we use it, and your rights when using the Sagledaj mobile application.</p>

                    <section>
                      <h2>1. Who we are</h2>
                      <p>App name: Sagledaj</p>
                      <p>Developer: Marija Đurđević</p>
                      <p>Contact email: masadjurdjevic02@gmail.com</p>
                    </section>

                    <section>
                      <h2>2. Data we collect</h2>
                      <p>We may collect:</p>
                      <ul>
                        <li>Account and identity data (email, basic Google Sign-In profile info, backend account identifiers)</li>
                        <li>Authentication/session tokens</li>
                        <li>Device/app technical data (app version, device model, OS, logs)</li>
                        <li>Push notification data (FCM token)</li>
                        <li>User-provided in-app content (forms/profile fields, and optional uploads if feature is used)</li>
                      </ul>
                    </section>

                    <section>
                      <h2>3. How we use data</h2>
                      <ul>
                        <li>Account creation and management</li>
                        <li>Authentication (including Google Sign-In)</li>
                        <li>Providing app functionality and personalization</li>
                        <li>Sending opted-in notifications</li>
                        <li>Security, abuse prevention, troubleshooting</li>
                        <li>Legal compliance</li>
                      </ul>
                    </section>

                    <section>
                      <h2>4. Legal bases (where applicable)</h2>
                      <ul>
                        <li>Contract performance</li>
                        <li>Legitimate interests</li>
                        <li>Consent (for optional features/notifications)</li>
                        <li>Legal obligations</li>
                      </ul>
                    </section>

                    <section>
                      <h2>5. Data sharing</h2>
                      <p>We do not sell personal data.</p>
                      <p>We may share data with service providers as needed, including:</p>
                      <ul>
                        <li>Google Firebase</li>
                        <li>Google Sign-In services</li>
                      </ul>
                    </section>

                    <section>
                      <h2>6. Data retention</h2>
                      <p>We retain data only as long as necessary for service delivery, security, legal obligations, and dispute resolution, then delete or anonymize it.</p>
                    </section>

                    <section>
                      <h2>7. Security</h2>
                      <p>We use reasonable technical and organizational safeguards (e.g., HTTPS/TLS, access controls).</p>
                    </section>

                    <section>
                      <h2>8. Your rights</h2>
                      <p>Depending on jurisdiction, users may request access, correction, deletion, restriction/objection, or consent withdrawal.</p>
                      <p>Contact: masadjurdjevic02@gmail.com</p>
                    </section>

                    <section>
                      <h2>9. Children</h2>
                      <p>Sagledaj is not directed to children under 13, and we do not knowingly collect personal data from children under 13.</p>
                    </section>

                    <section>
                      <h2>10. International transfers</h2>
                      <p>Data may be processed in countries where providers operate, with appropriate safeguards as required by law.</p>
                    </section>

                    <section>
                      <h2>11. Changes</h2>
                      <p>We may update this policy; updates appear on this page with revised “Last updated” date.</p>
                    </section>

                    <section>
                      <h2>12. Contact</h2>
                      <p>Email: masadjurdjevic02@gmail.com</p>
                      <p>Developer/Company: Marija Đurđević</p>
                    </section>
                  </main>
                </body>
                </html>
                """;

            return Results.Content(privacyHtml, "text/html; charset=utf-8");
        }).AllowAnonymous();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
