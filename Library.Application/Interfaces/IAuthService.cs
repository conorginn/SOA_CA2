using Library.Application.Dtos.Auth;

namespace Library.Application.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken ct = default);
}