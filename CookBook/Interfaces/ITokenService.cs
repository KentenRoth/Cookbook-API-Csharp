using CookBook.Models;

namespace CookBook.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);
}