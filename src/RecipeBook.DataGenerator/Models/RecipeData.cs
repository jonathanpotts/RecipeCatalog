namespace RecipeBook.DataGenerator.Models;

public class RecipeData
{
    public string? CoverImagePrompt { get; set; }

    public string? Description { get; set; }

    public List<string>? Ingredients { get; set; }

    public string? InstructionsMarkdown { get; set; }
}
