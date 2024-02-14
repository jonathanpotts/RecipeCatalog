using FluentValidation;
using JonathanPotts.RecipeCatalog.Domain.Shared.Models;

namespace JonathanPotts.RecipeCatalog.Application.Validation;

public class ImageDataValidator : AbstractValidator<ImageData>
{
    public ImageDataValidator()
    {
        RuleFor(x => x.Url).NotEmpty();
    }
}
