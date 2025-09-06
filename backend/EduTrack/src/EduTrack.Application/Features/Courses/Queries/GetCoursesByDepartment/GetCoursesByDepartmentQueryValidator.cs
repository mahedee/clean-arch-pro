using FluentValidation;

namespace EduTrack.Application.Features.Courses.Queries.GetCoursesByDepartment;

/// <summary>
/// Validator for GetCoursesByDepartmentQuery
/// </summary>
public class GetCoursesByDepartmentQueryValidator : AbstractValidator<GetCoursesByDepartmentQuery>
{
    public GetCoursesByDepartmentQueryValidator()
    {
        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required")
            .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters");

        RuleFor(x => x.Level)
            .Must(level => string.IsNullOrEmpty(level) || 
                          new[] { "Undergraduate", "Graduate", "Postgraduate", "Doctoral", "Certificate", "Continuing" }.Contains(level))
            .WithMessage("Level must be one of: Undergraduate, Graduate, Postgraduate, Doctoral, Certificate, Continuing")
            .When(x => !string.IsNullOrEmpty(x.Level));

        RuleFor(x => x.Status)
            .Must(status => string.IsNullOrEmpty(status) || 
                           new[] { "Draft", "Scheduled", "Active", "Inactive", "Completed", "Cancelled", "Archived" }.Contains(status))
            .WithMessage("Status must be one of: Draft, Scheduled, Active, Inactive, Completed, Cancelled, Archived")
            .When(x => !string.IsNullOrEmpty(x.Status));
    }
}
