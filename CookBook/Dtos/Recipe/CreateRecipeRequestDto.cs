using System.ComponentModel.DataAnnotations;

namespace CookBook.Dtos.Recipe;

public class CreateRecipeRequestDto
{
    [Required]
    [MaxLength(50, ErrorMessage = "Title must be 50 characters or less")]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Directions { get; set; } = string.Empty;
    
    [Required]
    public string Ingredients { get; set; } = string.Empty;

    public string Description { get; set; }
}