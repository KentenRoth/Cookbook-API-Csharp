using System.ComponentModel.DataAnnotations;

namespace CookBook.Dtos.Account;

public class RegisterDto
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? Username { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
}