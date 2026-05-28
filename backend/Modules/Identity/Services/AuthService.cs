using AngularNetBase.Identity.Dtos;
using AngularNetBase.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using AngularNetBase.Identity.Infrastructure;

namespace AngularNetBase.Identity.Services;

public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentityContext _db;
    private readonly JwtSettings _jwtSettings;
    private readonly string _googleClientId;

    public AuthService(UserManager<ApplicationUser> userManager, IdentityContext db, IOptions<JwtSettings> jwtSettings, IConfiguration configuration)
    {
        _userManager = userManager;
        _db = db;
        _jwtSettings = jwtSettings.Value;
        _googleClientId = configuration["Google:ClientId"]
                          ?? throw new Exception("Google ClientId is missing in configuration!");
    
    }

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
            return null;

        return await GenerateTokensAsync(user);
    }

    public async Task DeleteAccountAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return;

        var refreshTokens = await _db.RefreshTokens
            .Where(x => x.UserId == userId)
            .ToListAsync();
        if (refreshTokens.Count > 0)
            _db.RefreshTokens.RemoveRange(refreshTokens);

        var pushTokens = await _db.PushDeviceTokens
            .Where(x => x.UserId == userId)
            .ToListAsync();
        if (pushTokens.Count > 0)
            _db.PushDeviceTokens.RemoveRange(pushTokens);

        await _db.SaveChangesAsync();

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<AuthResponse> RegisterAsync(string email, string password)
    {
        var user = await RegisterUserAsync(email, password);
        return await GenerateTokensAsync(user);
    }

    public async Task<ApplicationUser> RegisterUserAsync(string email, string password)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
            throw new InvalidOperationException("Email is already registered.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
            throw new InvalidOperationException(string.Join(", ", createResult.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "User");
        return user;
    }

    public Task<ApplicationUser?> FindUserByEmailAsync(string email)
    {
        return _userManager.FindByEmailAsync(email);
    }

    public Task<AuthResponse> IssueTokensAsync(ApplicationUser user)
    {
        return GenerateTokensAsync(user);
    }

    public async Task LogoutAsync(string refreshToken, Guid userId)
    {
        var stored = await _db.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == refreshToken && r.UserId == userId);

        if (stored is null)
            return;

        _db.RefreshTokens.Remove(stored);
        await _db.SaveChangesAsync();
    }

    public async Task<AuthResponse?> RefreshAsync(string refreshToken)
    {
        var stored = await _db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (stored is null || stored.ExpiresAt < DateTime.UtcNow)
            return null;

        _db.RefreshTokens.Remove(stored);
        await _db.SaveChangesAsync();

        return await GenerateTokensAsync(stored.User);
    }

    private async Task<AuthResponse> GenerateTokensAsync(ApplicationUser user)
    {
        var (accessToken, expiresAt, role) = await GenerateAccessTokenAsync(user);
        var refreshTokenValue = GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenValue,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow
        };

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return new AuthResponse(
            accessToken,
            refreshTokenValue,
            expiresAt,
            user.Id.ToString(),
            role,
            user.OnboardingCompleted);
    }

    private async Task<(string Token, DateTime ExpiresAt, string Role)> GenerateAccessTokenAsync(ApplicationUser user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrEmpty(role))
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt, role);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task<AuthResponse> LoginWithGoogleAsync(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string>() { _googleClientId }
        };

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch
        {
            throw new UnauthorizedAccessException("Invalid Google Token or Client ID mismatch.");
        }
        return await GetOrCreateExternalUserAsync(payload.Email, payload.GivenName, payload.FamilyName);
    }

    private async Task<AuthResponse> GetOrCreateExternalUserAsync(string email, string? firstName, string? lastName)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName ?? "", 
                LastName = lastName ?? ""
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                throw new Exception($"Registration failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            await _userManager.AddToRoleAsync(user, "User");
        }

        return await GenerateTokensAsync(user);
    }

    public async Task UpdateOnboardingLanguageAsync(Guid userId, string preferredLanguage)
    {
        var user = await GetRequiredUserAsync(userId);
        EnsureOnboardingStillOpen(user);

        user.PreferredLanguage = preferredLanguage.Trim();
        await _userManager.UpdateAsync(user);
    }

    public async Task UpdateOnboardingHookAsync(Guid userId, string hookType, Guid? hookChallengeId)
    {
        var user = await GetRequiredUserAsync(userId);
        EnsureOnboardingStillOpen(user);

        var normalized = hookType.Trim().ToLowerInvariant();
        if (normalized is not ("distancedjournal" or "perspectivescenario"))
            throw new InvalidOperationException("HookType must be DistancedJournal or PerspectiveScenario.");

        user.HookType = normalized;
        user.HookChallengeId = hookChallengeId;
        await _userManager.UpdateAsync(user);
    }

    public async Task UpdateOnboardingProfileAsync(
        Guid userId,
        string firstName,
        string lastName,
        bool notificationEnabled,
        string? notificationTimeLocal,
        string? timeZoneId)
    {
        var user = await GetRequiredUserAsync(userId);
        EnsureOnboardingStillOpen(user);
        ApplyProfileSettings(
            user,
            firstName,
            lastName,
            user.PreferredLanguage,
            notificationEnabled,
            notificationTimeLocal,
            timeZoneId);

        await _userManager.UpdateAsync(user);
    }

    public async Task<ProfileMeResponse> GetProfileAsync(Guid userId)
    {
        var user = await GetRequiredUserAsync(userId);
        return MapProfile(user);
    }

    public async Task<ProfileMeResponse> UpdateProfileAsync(Guid userId, UpdateProfileMeRequest request)
    {
        var user = await GetRequiredUserAsync(userId);

        ApplyProfileSettings(
            user,
            request.FirstName,
            request.LastName,
            request.PreferredLanguage,
            request.NotificationEnabled,
            request.NotificationTimeLocal,
            request.TimeZoneId);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        return MapProfile(user);
    }

    public async Task CompleteOnboardingAsync(Guid userId)
    {
        var user = await GetRequiredUserAsync(userId);
        EnsureOnboardingStillOpen(user);

        if (string.IsNullOrWhiteSpace(user.PreferredLanguage))
            throw new InvalidOperationException("PreferredLanguage is required.");

        if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
            throw new InvalidOperationException("Profile is incomplete.");

        user.OnboardingCompleted = true;
        user.OnboardingCompletedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
    }

    public async Task<UserBootstrapResponse> GetBootstrapAsync(Guid userId)
    {
        var user = await GetRequiredUserAsync(userId);

        return new UserBootstrapResponse(
            user.Id.ToString(),
            user.OnboardingCompleted,
            user.PreferredLanguage,
            user.FirstName,
            user.LastName,
            user.NotificationEnabled,
            user.NotificationTimeLocal,
            user.TimeZoneId,
            user.HookType,
            user.HookChallengeId);
    }

    private async Task<ApplicationUser> GetRequiredUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            throw new InvalidOperationException("User not found.");

        return user;
    }

    private static void EnsureOnboardingStillOpen(ApplicationUser user)
    {
        if (user.OnboardingCompleted)
            throw new InvalidOperationException("Onboarding already completed.");
    }

    private static void ApplyProfileSettings(
        ApplicationUser user,
        string firstName,
        string lastName,
        string preferredLanguage,
        bool notificationEnabled,
        string? notificationTimeLocal,
        string? timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            throw new InvalidOperationException("FirstName and LastName are required.");

        if (string.IsNullOrWhiteSpace(preferredLanguage))
            throw new InvalidOperationException("PreferredLanguage is required.");

        if (notificationEnabled)
        {
            if (string.IsNullOrWhiteSpace(notificationTimeLocal) || !TimeOnly.TryParse(notificationTimeLocal, out _))
                throw new InvalidOperationException("NotificationTimeLocal must be a valid time when notifications are enabled.");

            if (string.IsNullOrWhiteSpace(timeZoneId))
                throw new InvalidOperationException("TimeZoneId is required when notifications are enabled.");
        }

        user.FirstName = firstName.Trim();
        user.LastName = lastName.Trim();
        user.PreferredLanguage = preferredLanguage.Trim();
        user.NotificationEnabled = notificationEnabled;
        user.NotificationTimeLocal = notificationEnabled ? notificationTimeLocal?.Trim() : null;
        user.TimeZoneId = notificationEnabled ? timeZoneId?.Trim() : null;
    }

    private static ProfileMeResponse MapProfile(ApplicationUser user)
    {
        return new ProfileMeResponse(
            user.Id.ToString(),
            user.Email ?? string.Empty,
            user.FirstName,
            user.LastName,
            user.PreferredLanguage,
            user.NotificationEnabled,
            user.NotificationTimeLocal,
            user.TimeZoneId,
            user.OnboardingCompleted);
    }
}
