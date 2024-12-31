using CookBook.Helpers;
using CookBook.Models;

namespace CookBook.Interfaces;

public interface IRecipeRepository
{
    Task<List<Recipe>> GetAllAsync(QueryObject query);
    Task<Recipe> CreateAsync(Recipe recipeModel);
    Task<Recipe?> GetByIdAsync(int id);
    Task<Recipe?> DeleteAsync(int id);
}