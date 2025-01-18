using CookBook.Data;
using CookBook.Dtos.Recipe;
using CookBook.Extensions;
using CookBook.Helpers;
using CookBook.Interfaces;
using CookBook.Mappers;
using CookBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CookBook.Controllers;

[Authorize(Roles = "Admin,User")]
[Route("api/recipe")]
[ApiController]

public class RecipeController : ControllerBase
{
    private readonly ApplicationDBContext _context;
    private readonly IRecipeRepository _recipeRepo;
    private readonly UserManager<AppUser> _userManager;
    
    public RecipeController(ApplicationDBContext context, IRecipeRepository recipeRepo, UserManager<AppUser> userManager)
    {
        _context = context;
        _recipeRepo = recipeRepo;
        _userManager = userManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
    {
        var recipes = await _recipeRepo.GetAllAsync(query);
        return Ok(recipes);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var recipe = await _recipeRepo.GetByIdAsync(id);
        if (recipe == null)
        {
            return NotFound();
        }
        return Ok(recipe);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRecipeRequestDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var name = User.GetName();
        var appUser = await _userManager.FindByNameAsync(name);
        
        var recipeModel = createDto.ToRecipeFromCreateDto();
        recipeModel.CreatedBy = appUser.Name;
        await _recipeRepo.CreateAsync(recipeModel);
        return CreatedAtAction(nameof(GetById), new { id = recipeModel.Id }, recipeModel.ToRecipeDto());
      
    }
    
    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var recipeModel = await _recipeRepo.DeleteAsync(id);
        if (recipeModel == null)
        {
            return NotFound();
        }
        return Ok(recipeModel);
    }
}