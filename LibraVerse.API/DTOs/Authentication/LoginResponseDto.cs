using LibraVerse.DTOs.User;

namespace LibraVerse.DTOs.Authentication;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;

    public UserDto User { get; set; } = new UserDto();
}