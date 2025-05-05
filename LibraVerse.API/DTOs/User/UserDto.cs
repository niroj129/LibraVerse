namespace LibraVerse.DTOs.User;

public class UserDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    
    public string State { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }

    public string Role { get; set; } = string.Empty;
}