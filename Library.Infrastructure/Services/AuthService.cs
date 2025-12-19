using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Library.Application.Dtos.Auth;
using Library.Application.Interfaces;
using Library.Infrastructure.Data;
using Library.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Library.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly LibraryDbContext _db;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(LibraryDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<bool> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        var username = dto.Username.Trim();

        var exists = await _db.Users.AnyAsync(u => u.Username == username, ct);
        if (exists) return false;

        var role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role.Trim();

        var user = new User
        {
            Username = username,
            Role = role
        };

        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var username = dto.Username.Trim();

        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username, ct);
        if (user is null) return null;

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed) return null;

        var jwt = CreateJwt(user);
        return jwt;
    }

    private AuthResponseDto CreateJwt(User user)
    {
        var key = _config["Jwt:Key"];
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];
        var expiresMinutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 60;

        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Jwt:Key missing from configuration.");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto(
            AccessToken: accessToken,
            TokenType: "Bearer",
            ExpiresInSeconds: expiresMinutes * 60,
            Username: user.Username,
            Role: user.Role
        );
    }
}