using System.ComponentModel.DataAnnotations;

namespace RecipeBook.Api.Models;

public class CuisineCreateOrUpdateDto
{
    [Required]
    public string? Name { get; set; }
}
