using FluentValidation;

namespace EduTrack.Application.Features.Courses.Commands.CompleteCourse;

/// <summary>
/// Validator for CompleteCourseCommand
/// </summary>
public class CompleteCourseCommandValidator : AbstractValidator<CompleteCourseCommand>
{
    public CompleteCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Course ID is required");

        RuleFor(x => x.CompletionNotes)
            .MaximumLength(1000).WithMessage("Completion notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.CompletionNotes));
    }
}
