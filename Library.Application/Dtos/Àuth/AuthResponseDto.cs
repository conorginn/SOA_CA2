namespace Library.Application.Dtos.Auth;

public record AuthResponseDto(
    string AccessToken,
    string TokenType,
    int ExpiresInSeconds,
    string Username,
    string Role
);