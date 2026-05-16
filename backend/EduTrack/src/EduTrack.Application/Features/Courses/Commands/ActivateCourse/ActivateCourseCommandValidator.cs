using FluentValidation;

namespace EduTrack.Application.Features.Courses.Commands.ActivateCourse;

/// <summary>
/// Validator for ActivateCourseCommand
/// </summary>
public class ActivateCourseCommandValidator : AbstractValidator<ActivateCourseCommand>
{
    public ActivateCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Course ID is required");
    }
}
