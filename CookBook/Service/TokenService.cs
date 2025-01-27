using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CookBook.Data;
using CookBook.Interfaces;
using CookBook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CookBook.Service;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;
    private readonly ApplicationDBContext _context;
    private readonly UserManager<AppUser> _userManager;
    
    public TokenService(IConfiguration config, UserManager<AppUser> userManager, ApplicationDBContext context)
    {
        _config = config;
        _userManager = userManager;
        _context = context;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
    }

    public string CreateToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
        };
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds,
            Issuer = _config["JWT:Issuer"],
            Audience = _config["JWT:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string CreateRefreshToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:RefreshKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<object> GenerateRefreshToken(AppUser user)
    {
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddMonths(1)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken.Token;
    }
    
    public async Task<IActionResult> RefreshAccessToken(string userId, string oldRefreshToken, HttpResponse response)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new NotFoundObjectResult("User not found");
        }

        var storedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == oldRefreshToken);

        if (storedRefreshToken == null || storedRefreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return new UnauthorizedObjectResult("Invalid or expired refresh token");
        }

        storedRefreshToken.IsRevoked = true;
        _context.Entry(storedRefreshToken).State = EntityState.Modified;

        var newAccessToken = CreateToken(user);
        var newRefreshToken = await GenerateRefreshToken(user);

        await _context.SaveChangesAsync();
        SetAccessTokenCookie(response, newAccessToken);

        return new OkObjectResult("Access token refreshed successfully");
    }


    public void SetAccessTokenCookie(HttpResponse response, string accessToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15)
        };

        response.Cookies.Append("AccessToken", accessToken, cookieOptions);
    }
}