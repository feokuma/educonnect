namespace EduConnect.Infrastructure.Authentication;

public sealed class AuthSettings
{
    public string Issuer { get; init; } = "educonnect";

    public string Audience { get; init; } = "educonnect-api";

    public string SecretKey { get; init; } = "educonnect-development-secret-key-change-in-production";

    public int AccessTokenExpirationMinutes { get; init; } = 15;

    public int RefreshTokenExpirationDays { get; init; } = 7;
}
