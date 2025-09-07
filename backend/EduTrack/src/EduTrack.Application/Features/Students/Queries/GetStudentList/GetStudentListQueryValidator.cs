using FluentValidation;

namespace EduTrack.Application.Features.Students.Queries.GetStudentList;

/// <summary>
/// Validator for GetStudentListQuery
/// </summary>
public class GetStudentListQueryValidator : AbstractValidator<GetStudentListQuery>
{
    private static readonly string[] ValidSortFields = { "FullName", "Email", "DateOfBirth", "GPA", "Status", "EnrollmentDate" };
    private static readonly string[] ValidSortDirections = { "asc", "desc" };

    public GetStudentListQueryValidator()
    {
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

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .WithMessage("Search term cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        RuleFor(x => x.MinGPA)
            .GreaterThanOrEqualTo(0.0m)
            .WithMessage("Minimum GPA must be 0.0 or higher")
            .LessThanOrEqualTo(4.0m)
            .WithMessage("Minimum GPA cannot exceed 4.0")
            .When(x => x.MinGPA.HasValue);

        RuleFor(x => x.MaxGPA)
            .GreaterThanOrEqualTo(0.0m)
            .WithMessage("Maximum GPA must be 0.0 or higher")
            .LessThanOrEqualTo(4.0m)
            .WithMessage("Maximum GPA cannot exceed 4.0")
            .When(x => x.MaxGPA.HasValue);

        RuleFor(x => x)
            .Must(HaveValidGPARange)
            .WithMessage("Maximum GPA must be greater than or equal to minimum GPA")
            .When(x => x.MinGPA.HasValue && x.MaxGPA.HasValue);

        RuleFor(x => x.MinAge)
            .GreaterThanOrEqualTo(16)
            .WithMessage("Minimum age must be 16 or higher")
            .LessThanOrEqualTo(100)
            .WithMessage("Minimum age cannot exceed 100")
            .When(x => x.MinAge.HasValue);

        RuleFor(x => x.MaxAge)
            .GreaterThanOrEqualTo(16)
            .WithMessage("Maximum age must be 16 or higher")
            .LessThanOrEqualTo(100)
            .WithMessage("Maximum age cannot exceed 100")
            .When(x => x.MaxAge.HasValue);

        RuleFor(x => x)
            .Must(HaveValidAgeRange)
            .WithMessage("Maximum age must be greater than or equal to minimum age")
            .When(x => x.MinAge.HasValue && x.MaxAge.HasValue);
    }

    private static bool BeValidSortField(string sortField)
    {
        return ValidSortFields.Contains(sortField, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidSortDirection(string sortDirection)
    {
        return ValidSortDirections.Contains(sortDirection, StringComparer.OrdinalIgnoreCase);
    }

    private static bool HaveValidGPARange(GetStudentListQuery query)
    {
        return query.MaxGPA >= query.MinGPA;
    }

    private static bool HaveValidAgeRange(GetStudentListQuery query)
    {
        return query.MaxAge >= query.MinAge;
    }
}
