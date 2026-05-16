using FluentValidation;

namespace EduTrack.Application.Features.Courses.Queries.GetCourseList;

/// <summary>
/// Validator for GetCourseListQuery
/// </summary>
public class GetCourseListQueryValidator : AbstractValidator<GetCourseListQuery>
{
    public GetCourseListQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.SortBy)
            .Must(sortBy => new[] { "Title", "Code", "Department", "CreditHours", "Level", "Status" }.Contains(sortBy))
            .WithMessage("Sort by must be one of: Title, Code, Department, CreditHours, Level, Status");

        RuleFor(x => x.SortDirection)
            .Must(direction => new[] { "asc", "desc" }.Contains(direction.ToLower()))
            .WithMessage("Sort direction must be 'asc' or 'desc'");
    }
}
