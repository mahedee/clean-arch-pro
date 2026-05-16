using FluentValidation;

namespace EduTrack.Application.Features.Students.Queries.GetStudentsByStatus;

/// <summary>
/// Validator for GetStudentsByStatusQuery
/// </summary>
public class GetStudentsByStatusQueryValidator : AbstractValidator<GetStudentsByStatusQuery>
{
    private static readonly string[] ValidSortFields = { "FullName", "Email", "DateOfBirth", "GPA", "EnrollmentDate" };
    private static readonly string[] ValidSortDirections = { "asc", "desc" };

    public GetStudentsByStatusQueryValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid student status");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.SortBy)
            .Must(BeValidSortField)
            .WithMessage($"Sort field must be one of: {string.Join(", ", ValidSortFields)}");

        RuleFor(x => x.SortDirection)
            .Must(BeValidSortDirection)
            .WithMessage($"Sort direction must be one of: {string.Join(", ", ValidSortDirections)}");
    }

    private static bool BeValidSortField(string sortField)
    {
        return ValidSortFields.Contains(sortField, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidSortDirection(string sortDirection)
    {
        return ValidSortDirections.Contains(sortDirection, StringComparer.OrdinalIgnoreCase);
    }
}
