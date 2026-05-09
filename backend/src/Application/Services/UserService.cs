using EduConnect.Application.Common;
using EduConnect.Application.DTOs;
using EduConnect.Application.Repositories;
using EduConnect.Domain.Users;

namespace EduConnect.Application.Services;

public sealed class UserService(IIdGenerator idGenerator, IUserRepository userRepository) : IUserService
{
    public async Task<UserResponseDto> CreateAsync(
        CreateUserRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var user = User.Create(idGenerator.NewId(), request.Name, request.Email);
        var createdUser = await userRepository.CreateAsync(user, cancellationToken);

        return new UserResponseDto(
            createdUser.Id,
            createdUser.Name,
            createdUser.Email,
            createdUser.CreatedAt);
    }
}
