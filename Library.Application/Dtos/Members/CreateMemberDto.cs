namespace Library.Application.Dtos.Members;

public record CreateMemberDto(
    string FullName,
    string Email
);