using Microsoft.AspNetCore.Identity;

namespace RecipeCatalog.Domain.Entities;

public class User : IdentityUser
{
    public List<Recipe>? Recipes { get; set; }
}
