using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Application.DTOs.ResponseDTOs;
using Application.DTOs.CreateDTOs;
using Application.DTOs.Jwt;
using Application.Interfaces;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public AuthService(IAppDbContext context, ITokenService tokenService, IUserService userService)
    {
        _context = context;
        _tokenService = tokenService;
        _userService = userService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterDto registerDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _userService.CreateUserAsync(new CreateUserDto
            {
                Email = registerDto.Email,
                Password = registerDto.Password,
                UserName = registerDto.UserName
            });

            var defaultProject = new Project
            {
                Name = $"Проект {user.Email}",
                Description = "Проект по умолчанию",
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id
            };

            _context.Projects.Add(defaultProject);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // Генерация токена
            string token = _tokenService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                User = new ResponseUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt
                }
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<AuthResponse?> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
            return null;

        bool passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
        if (!passwordValid)
            return null;

        string token = _tokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            User = new ResponseUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            }
        };
    }
}