using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EduConnect.Application.DTOs;
using EduConnect.Application.Services;
using Microsoft.Extensions.Options;

namespace EduConnect.Infrastructure.Authentication;

public sealed class AuthService(IOptions<AuthSettings> options) : IAuthService
{
    private static readonly ConcurrentDictionary<string, RefreshTokenState> RefreshTokens = new();

    private readonly AuthSettings _settings = options.Value;

    public AuthResponseDto? Authenticate(LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var userId = Guid.NewGuid().ToString("N");
        return CreateAuthenticationResponse(userId, request.Email.Trim().ToLowerInvariant());
    }

    public AuthResponseDto? Refresh(RefreshTokenRequestDto request)
    {
        if (!RefreshTokens.TryRemove(request.RefreshToken, out var state))
        {
            return null;
        }

        if (state.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            return null;
        }

        return CreateAuthenticationResponse(state.Subject, state.Email);
    }

    public TokenValidationResponseDto Validate(ValidateTokenRequestDto request)
    {
        if (!TryReadPayload(request.Token, out var payload))
        {
            return new TokenValidationResponseDto(false, null, null, null);
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var issuer = GetString(payload, "iss");
        var audience = GetString(payload, "aud");
        var subject = GetString(payload, "sub");
        var email = GetString(payload, "email");
        var expiresAtUnix = GetLong(payload, "exp");
        var notBeforeUnix = GetLong(payload, "nbf");

        var isValid = issuer == _settings.Issuer
            && audience == _settings.Audience
            && !string.IsNullOrWhiteSpace(subject)
            && expiresAtUnix is not null
            && expiresAtUnix > now
            && (notBeforeUnix is null || notBeforeUnix <= now);

        DateTimeOffset? expiresAt = expiresAtUnix is null
            ? null
            : DateTimeOffset.FromUnixTimeSeconds(expiresAtUnix.Value);

        return new TokenValidationResponseDto(isValid, isValid ? subject : null, isValid ? email : null, expiresAt);
    }

    private AuthResponseDto CreateAuthenticationResponse(string subject, string email)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);
        var accessToken = GenerateAccessToken(subject, email, expiresAt);
        var refreshToken = GenerateRefreshToken();

        RefreshTokens[refreshToken] = new RefreshTokenState(
            subject,
            email,
            DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenExpirationDays));

        return new AuthResponseDto(accessToken, refreshToken, expiresAt);
    }

    private string GenerateAccessToken(string subject, string email, DateTimeOffset expiresAt)
    {
        var now = DateTimeOffset.UtcNow;
        var header = new Dictionary<string, object>
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        };
        var payload = new Dictionary<string, object>
        {
            ["iss"] = _settings.Issuer,
            ["aud"] = _settings.Audience,
            ["sub"] = subject,
            ["email"] = email,
            ["jti"] = Guid.NewGuid().ToString("N"),
            ["iat"] = now.ToUnixTimeSeconds(),
            ["nbf"] = now.ToUnixTimeSeconds(),
            ["exp"] = expiresAt.ToUnixTimeSeconds()
        };

        var encodedHeader = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(header));
        var encodedPayload = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
        var unsignedToken = $"{encodedHeader}.{encodedPayload}";
        var signature = Sign(unsignedToken);

        return $"{unsignedToken}.{signature}";
    }

    private bool TryReadPayload(string token, out JsonElement payload)
    {
        payload = default;
        var parts = token.Split('.');

        if (parts.Length != 3)
        {
            return false;
        }

        var expectedSignature = Sign($"{parts[0]}.{parts[1]}");
        if (!CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expectedSignature),
            Encoding.UTF8.GetBytes(parts[2])))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(Base64UrlDecode(parts[1]));
            payload = document.RootElement.Clone();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private string Sign(string value)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.SecretKey));
        return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(value)));
    }

    private static string GenerateRefreshToken()
    {
        return Base64UrlEncode(RandomNumberGenerator.GetBytes(64));
    }

    private static string Base64UrlEncode(byte[] value)
    {
        return Convert.ToBase64String(value)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        var padding = base64.Length % 4;

        if (padding > 0)
        {
            base64 = base64.PadRight(base64.Length + 4 - padding, '=');
        }

        return Convert.FromBase64String(base64);
    }

    private static string? GetString(JsonElement payload, string propertyName)
    {
        return payload.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private static long? GetLong(JsonElement payload, string propertyName)
    {
        return payload.TryGetProperty(propertyName, out var value) && value.TryGetInt64(out var result)
            ? result
            : null;
    }

    private sealed record RefreshTokenState(string Subject, string Email, DateTimeOffset ExpiresAt);
}
