namespace Entities.Dtos;

public record LoginRequestDto(
    string EmailOrUserName,
    string Password,
    bool RememberMe = false);