using EduConnect.Application.DTOs;
using EduConnect.Infrastructure.Persistence;
using EduConnect.Infrastructure.Persistence.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace EduConnect.Integration.Setup;

public static class IntegrationUserSeeder
{
    public static async Task SeedUserAsync(
        this IntegrationWebAppFactory factory,
        CreateUserRequestDto request)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EduConnectDbContext>();

        dbContext.Users.Add(new UserEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTimeOffset.UtcNow
        });

        await dbContext.SaveChangesAsync();
    }
}
