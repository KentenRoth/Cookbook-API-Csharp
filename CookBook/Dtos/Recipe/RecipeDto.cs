namespace CookBook.Dtos.Recipe;

public class RecipeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Directions { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
}