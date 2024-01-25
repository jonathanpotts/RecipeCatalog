using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeBook.WebApi.Models;

public class MarkdownData
{
    [Required]
    public string? Markdown { get; set; }

    [Required]
    public string? Html { get; set; }
}
