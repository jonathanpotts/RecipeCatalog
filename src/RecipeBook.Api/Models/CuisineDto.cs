using System.ComponentModel.DataAnnotations;

namespace RecipeBook.Api.Models;

public class CuisineDto
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }
}
