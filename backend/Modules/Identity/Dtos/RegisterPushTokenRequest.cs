namespace AngularNetBase.Identity.Dtos;

public record RegisterPushTokenRequest(
    string Token,
    string Platform);
