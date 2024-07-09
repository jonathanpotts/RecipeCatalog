using FluentValidation;
using RecipeCatalog.Domain.Shared.ValueObjects;

namespace RecipeCatalog.Application.Validation;

public class ImageDataValidator : AbstractValidator<ImageData>
{
    public ImageDataValidator()
    {
        RuleFor(x => x.Url).NotEmpty();
    }
}
