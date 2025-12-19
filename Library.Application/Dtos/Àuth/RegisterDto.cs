namespace Library.Application.Dtos.Auth;

public record RegisterDto(
    string Username,
    string Password,
    string? Role
);