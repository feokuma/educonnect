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

    private sealed record RefreshTokenState(string Subject, string Email, DateTimeOffset ExpiresAt);
}
