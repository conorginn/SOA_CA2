namespace Library.Application.Dtos.Members;

public record MemberDto(
    int Id,
    string FullName,
    string Email
);