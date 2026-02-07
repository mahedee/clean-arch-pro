using FluentValidation;

namespace EduTrack.Application.Features.Students.Queries.GetStudentsOnProbation;

/// <summary>
/// Validator for GetStudentsOnProbationQuery
/// </summary>
public class GetStudentsOnProbationQueryValidator : AbstractValidator<GetStudentsOnProbationQuery>
{
    private static readonly string[] ValidSortFields = { "FullName", "Email", "DateOfBirth", "GPA", "EnrollmentDate" };
    private static readonly string[] ValidSortDirections = { "asc", "desc" };

    public GetStudentsOnProbationQueryValidator()
    {
        RuleFor(x => x.GPAThreshold)
            .GreaterThan(0.0m)
            .WithMessage("GPA threshold must be greater than 0.0")
            .LessThanOrEqualTo(4.0m)
            .WithMessage("GPA threshold cannot exceed 4.0");

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
