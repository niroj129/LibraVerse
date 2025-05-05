using LibraVerse.Enums;

namespace LibraVerse.Models;

public class User
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    
    public string State { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
    
    public Role Role { get; set; }
}