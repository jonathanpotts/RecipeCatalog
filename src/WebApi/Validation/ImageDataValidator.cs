using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

namespace JonathanPotts.RecipeCatalog.WebApi.Validation;

public class ImageDataValidator : AbstractValidator<ImageData>
{
    public ImageDataValidator()
    {
        RuleFor(x => x.Url).NotEmpty();
    }
}
