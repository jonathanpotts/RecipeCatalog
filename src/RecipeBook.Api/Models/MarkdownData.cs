using System.ComponentModel.DataAnnotations;

namespace RecipeBook.Api.Models;

public class MarkdownData
{
    [Required]
    public string? Markdown { get; set; }

    [Required]
    public string? Html { get; set; }
}
