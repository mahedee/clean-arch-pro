using FluentValidation;

namespace EduTrack.Application.Features.Students.Commands.UpdateStudent;

/// <summary>
/// Validator for UpdateStudentCommand
/// </summary>
public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Student ID is required");

        RuleFor(x => x.FullName)
            .Length(2, 100).WithMessage("Full name must be between 2 and 100 characters")
            .When(x => !string.IsNullOrEmpty(x.FullName));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email must be a valid email address")
            .Length(5, 100).WithMessage("Email must be between 5 and 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber)
            .Must(BeValidPhoneNumber).WithMessage("Phone number must be a valid format (e.g., +1-555-123-4567)")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.GPA)
            .InclusiveBetween(0.0m, 4.0m).WithMessage("GPA must be between 0.0 and 4.0")
            .Must(BeValidGPAPrecision).WithMessage("GPA can have at most 2 decimal places")
            .When(x => x.GPA.HasValue);

        // Address validation (all fields required if any address field is provided)
        When(x => HasAnyAddressField(x), () =>
        {
            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required when updating address")
                .Length(1, 100).WithMessage("Street must be between 1 and 100 characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required when updating address")
                .Length(1, 50).WithMessage("City must be between 1 and 50 characters");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required when updating address")
                .Length(2, 50).WithMessage("State must be between 2 and 50 characters");

            RuleFor(x => x.ZipCode)
                .NotEmpty().WithMessage("Zip code is required when updating address")
                .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Zip code must be in format 12345 or 12345-6789");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required when updating address")
                .Length(2, 50).WithMessage("Country must be between 2 and 50 characters");
        });
    }

    private static bool BeValidPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return true;

        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\+\d{1,3}-\d{3}-\d{3}-\d{4}$");
    }

    private static bool BeValidGPAPrecision(decimal? gpa)
    {
        if (!gpa.HasValue)
            return true;

        return Math.Round(gpa.Value, 2) == gpa.Value;
    }

    private static bool HasAnyAddressField(UpdateStudentCommand command)
    {
        return !string.IsNullOrEmpty(command.Street) ||
               !string.IsNullOrEmpty(command.City) ||
               !string.IsNullOrEmpty(command.State) ||
               !string.IsNullOrEmpty(command.ZipCode) ||
               !string.IsNullOrEmpty(command.Country);
    }
}
