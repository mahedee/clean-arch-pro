using FluentValidation;

namespace EduTrack.Application.Features.Courses.Commands.DeleteCourse;

/// <summary>
/// Validator for DeleteCourseCommand
/// </summary>
public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Course ID is required")
            .NotEqual(Guid.Empty).WithMessage("Course ID must be a valid GUID");
    }
}