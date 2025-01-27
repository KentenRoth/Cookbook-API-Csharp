using CookBook.Data;
using CookBook.Dtos.Account;
using CookBook.Interfaces;
using CookBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Controllers;


[Route("api/account")]
[ApiController]

public class AccountController : ControllerBase
{   
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ApplicationDBContext _context;
    
    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager, ApplicationDBContext context)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
        _context = context;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var appUser = new AppUser
            {
                Name = registerDto.Name,
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

            if (createdUser.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(appUser, "Pending");
                if (roleResult.Succeeded)
                {
                    var accessToken = _tokenService.CreateToken(appUser);
                    var refreshToken = await _tokenService.GenerateRefreshToken(appUser);

                    _tokenService.SetAccessTokenCookie(Response, accessToken);

                    return Ok(new
                    {
                        Name = appUser.Name,
                        Username = appUser.UserName,
                        Email = appUser.Email,
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    });
                }
                else
                {
                    return StatusCode(500, roleResult.Errors);
                }
            }
            else
            {
                return StatusCode(500, createdUser.Errors);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var appUser = await _userManager.FindByNameAsync(loginDto.Username);
        if (appUser == null) return Unauthorized("Invalid username or password");

        var signInResult = await _signInManager.CheckPasswordSignInAsync(appUser, loginDto.Password, false);
        if (!signInResult.Succeeded) return Unauthorized("Invalid username or password");

        var accessToken = _tokenService.CreateToken(appUser);
        var refreshToken = await _tokenService.GenerateRefreshToken(appUser);

        _tokenService.SetAccessTokenCookie(Response, accessToken);

        return Ok(new
        {
            Name = appUser.Name,
            Username = appUser.UserName,
            Email = appUser.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(string refreshToken)
    {
        var storedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
        if (storedRefreshToken != null)
        {
            storedRefreshToken.IsRevoked = true;
            _context.Entry(storedRefreshToken).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        Response.Cookies.Delete("AccessToken");

        await _signInManager.SignOutAsync();
        return Ok("Logged out successfully");
    }
}