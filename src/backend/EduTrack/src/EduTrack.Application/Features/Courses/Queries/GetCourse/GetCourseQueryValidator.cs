using FluentValidation;

namespace EduTrack.Application.Features.Courses.Queries.GetCourse;

/// <summary>
/// Validator for GetCourseQuery
/// </summary>
public class GetCourseQueryValidator : AbstractValidator<GetCourseQuery>
{
    public GetCourseQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Course ID is required");
    }
}
