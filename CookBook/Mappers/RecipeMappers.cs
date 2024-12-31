using CookBook.Dtos.Recipe;
using CookBook.Models;

namespace CookBook.Mappers;

public static class RecipeMappers
{
    public static RecipeDto ToRecipeDto(this Recipe recipeModel)
    {
        return new RecipeDto
        {
            Id = recipeModel.Id,
            Title = recipeModel.Title,
            Description = recipeModel.Description,
            Directions = recipeModel.Directions,
            Ingredients = recipeModel.Ingredients

        };
    }

    public static Recipe ToRecipeFromCreateDto(this CreateRecipeRequestDto recipeDto)
    {
        return new Recipe
        {
            Title = recipeDto.Title,
            Description = recipeDto.Description,
            Directions = recipeDto.Directions,
            Ingredients = recipeDto.Ingredients
        };
    }
}
