using FluentValidation;
using TodoApi.Models.DTOs;

namespace TodoApi.Validators;

/// <summary>Validates <see cref="UpdateBookDto"/> before updating an existing book.</summary>
public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
{
    public UpdateBookDtoValidator()
    {
        RuleFor(x => x.CountryCode)
            .NotEmpty().WithMessage("Country code is required.")
            .Length(2, 3).WithMessage("Country code must be 2 or 3 characters (e.g., US, HK, GBR).")
            .Must(BeValidIsoCode).WithMessage("Country code must be uppercase letters only.");

        RuleFor(x => x.Category)
            .GreaterThan(0).WithMessage("Category must be a positive number greater than 0.");

        RuleFor(x => x.PublishDate)
            .NotEmpty().WithMessage("Publish date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Publish date cannot be in the future.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Book name cannot be empty.")
            .MaximumLength(200).WithMessage("Book name cannot exceed 200 characters.")
            .Must(name => !name.All(char.IsDigit)).WithMessage("Book name cannot consist entirely of numbers.");
    }

    /// <summary>Validates that the country code consists only of uppercase letters (A-Z).</summary>
    private bool BeValidIsoCode(string countryCode)
    {
        if (string.IsNullOrEmpty(countryCode)) return false;
        return countryCode.All(char.IsUpper) && countryCode.All(char.IsLetter);
    }
}
