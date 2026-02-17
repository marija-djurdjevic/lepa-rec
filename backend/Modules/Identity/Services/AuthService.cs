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

        return new AuthResponse(accessToken, refreshTokenValue, expiresAt, user.Id.ToString(), role);
    }

    private async Task<(string Token, DateTime ExpiresAt, string Role)> GenerateAccessTokenAsync(ApplicationUser user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
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
}
