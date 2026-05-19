using FluentValidation;
using TodoApi.Models.DTOs;

namespace TodoApi.Validators;

public class BookSearchRequestValidator : AbstractValidator<BookSearchRequest>
{
    public BookSearchRequestValidator()
    {
        When(x => x.CountryCode is not null, () =>
        {
            RuleFor(x => x.CountryCode!)
                .Length(2, 3).WithMessage("Country code must be 2 or 3 characters.")
                .Must(BeValidIsoCode).WithMessage("Country code must be uppercase letters only.");
        });

        When(x => x.Category is not null, () =>
        {
            RuleFor(x => x.Category!)
                .GreaterThan(0).WithMessage("Category must be a positive number.");
        });

        When(x => x.PublishDateNotLaterThan is not null, () =>
        {
            RuleFor(x => x.PublishDateNotLaterThan!)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("PublishDateNotLaterThan cannot be in the future.");
        });

        When(x => x.PublishDateNotEarlierThan is not null, () =>
        {
            RuleFor(x => x.PublishDateNotEarlierThan!)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("PublishDateNotEarlierThan cannot be in the future.");
        });

        When(x => x.PublishDateNotEarlierThan is not null && x.PublishDateNotLaterThan is not null, () =>
        {
            RuleFor(x => x.PublishDateNotEarlierThan)
                .LessThanOrEqualTo(x => x.PublishDateNotLaterThan)
                .WithMessage("PublishDateNotEarlierThan must not be later than PublishDateNotLaterThan.");
        });

        When(x => x.Name is not null, () =>
        {
            RuleFor(x => x.Name!)
                .MaximumLength(200).WithMessage("Name filter cannot exceed 200 characters.");
        });
    }

    private bool BeValidIsoCode(string countryCode)
    {
        if (string.IsNullOrEmpty(countryCode)) return false;
        return countryCode.All(char.IsUpper) && countryCode.All(char.IsLetter);
    }
}
