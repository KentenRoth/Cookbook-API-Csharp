using CookBook.Models;

namespace CookBook.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);
    void SetAccessTokenCookie(HttpResponse response, string accessToken);
    Task<object> GenerateRefreshToken(AppUser appUser);
}