using Microsoft.AspNetCore.Identity;

namespace CookBook.Models;

public class AppUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
}