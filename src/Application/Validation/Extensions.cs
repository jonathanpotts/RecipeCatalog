using FluentValidation;

namespace JonathanPotts.RecipeCatalog.Application.Validation;

public static class Extensions
{
    public static Dictionary<string, string[]> ToDictionary(this ValidationException exception)
    {
        return exception.Errors.GroupBy(f => f.PropertyName).ToDictionary(
            g => g.Key,
            g => g.Select(f => f.ErrorMessage).ToArray());
    }
}
