namespace Entities.Dtos;

public record LoginResponseDto(
    string Token,
    string RefreshToken,
    DateTime RefreshTokenExpires);