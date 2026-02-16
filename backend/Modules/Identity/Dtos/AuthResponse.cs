namespace AngularNetBase.Identity.Dtos;

public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);
