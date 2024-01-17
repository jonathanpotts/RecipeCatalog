using System.ComponentModel.DataAnnotations;

namespace RecipeBook.Api.Models;

public class Recipe
{
    public long Id { get; set; }

    [Required] public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    [Required] public string? InstructionsMarkdown { get; set; }

    [Required] public string? InstructionsHtml { get; set; }
}