using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeCatalog.WebApi.Models;

public class CuisineCreateOrUpdateDto
{
    [Required]
    public string? Name { get; set; }
}
