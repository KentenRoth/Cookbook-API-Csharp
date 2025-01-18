using CookBook.Data;
using CookBook.Helpers;
using CookBook.Interfaces;
using CookBook.Models;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Repository;

public class RecipeRepository : IRecipeRepository
{
    private readonly ApplicationDBContext _context;
    public RecipeRepository(ApplicationDBContext context)
    {
        _context = context;
    }
    
    public async Task<List<Recipe>> GetAllAsync(QueryObject query)
    {
        var recipes =  _context.Recipes.AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Title))
        {
            recipes = recipes.Where(s => s.Title.Contains(query.Title));
        }
        
        if (!string.IsNullOrWhiteSpace(query.Ingredients))
        {
            recipes = recipes.Where(s => s.Ingredients.Contains(query.Ingredients));
        }
        
        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            if (query.SortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                recipes = query.IsDescending ? recipes.OrderByDescending(s => s.Title): recipes.OrderBy(s => s.Title);
            }
        }

        var skipNumber = (query.PageNumber - 1) * query.PageSize;
        
        return await recipes.Skip(skipNumber).Take(query.PageSize).ToListAsync();
    }

    public async Task<Recipe?> GetByIdAsync(int id)
    {
        return await _context.Recipes
            .Include(r => r.AppUser) 
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Recipe> CreateAsync(Recipe recipeModel)
    {
        await _context.Recipes.AddAsync(recipeModel);
        await _context.SaveChangesAsync();
        return recipeModel;
    }

    public async Task<Recipe?> DeleteAsync(int id)
    {
        var recipeModel = await _context.Recipes.FirstOrDefaultAsync(x => x.Id == id);
        if (recipeModel == null)
        {
            return null;
        }

        _context.Recipes.Remove(recipeModel);
        await _context.SaveChangesAsync();
        return recipeModel;
    }
}