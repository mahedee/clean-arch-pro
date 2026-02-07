using FluentValidation;
using EduTrack.Domain.Contracts.Repositories;

namespace EduTrack.Application.Features.Students.Commands.CreateStudent;

/// <summary>
/// Validator for CreateStudentCommand
/// </summary>
public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    private readonly IStudentRepository _studentRepository;

    public CreateStudentCommandValidator(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .Length(2, 100).WithMessage("Full name must be between 2 and 100 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .Must(BeAValidBirthDate).WithMessage("Date of birth must be in the past and student must be at least 16 years old")
            .Must(BeNotTooOld).WithMessage("Date of birth cannot be more than 100 years ago");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address")
            .Length(5, 100).WithMessage("Email must be between 5 and 100 characters")
            .MustAsync(BeUniqueEmail).WithMessage("A student with this email address already exists");

        RuleFor(x => x.PhoneNumber)
            .Must(BeValidPhoneNumber).WithMessage("Phone number must be a valid format (e.g., +1-555-123-4567)")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        // Address validation (when provided)
        When(x => !string.IsNullOrEmpty(x.Street) || !string.IsNullOrEmpty(x.City), () =>
        {
            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required when address is provided")
                .Length(1, 100).WithMessage("Street must be between 1 and 100 characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required when address is provided")
                .Length(1, 50).WithMessage("City must be between 1 and 50 characters");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required when address is provided")
                .Length(2, 50).WithMessage("State must be between 2 and 50 characters");

            RuleFor(x => x.ZipCode)
                .NotEmpty().WithMessage("Zip code is required when address is provided")
                .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Zip code must be in format 12345 or 12345-6789");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required when address is provided")
                .Length(2, 50).WithMessage("Country must be between 2 and 50 characters");
        });
    }

    private static bool BeAValidBirthDate(DateTime dateOfBirth)
    {
        return dateOfBirth < DateTime.Today && dateOfBirth <= DateTime.Today.AddYears(-16);
    }

    private static bool BeNotTooOld(DateTime dateOfBirth)
    {
        return dateOfBirth >= DateTime.Today.AddYears(-100);
    }

    private static bool BeValidPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return true;

        // Validate phone number format (e.g., +1-555-123-4567)
        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\+\d{1,3}-\d{3}-\d{3}-\d{4}$");
    }

    /// <summary>
    /// Validates that the email address is unique across all students
    /// </summary>
    /// <param name="email">The email address to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the email is unique, false if it already exists</returns>
    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(email))
            return true; // Let the NotEmpty validation handle null/empty

        try
        {
            return !(await _studentRepository.ExistsByEmailAsync(email, cancellationToken));
        }
        catch
        {
            // If there's an error checking the database, let the command handler handle it
            return true;
        }
    }
}