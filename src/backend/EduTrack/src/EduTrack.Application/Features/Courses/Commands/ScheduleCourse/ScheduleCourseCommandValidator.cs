using FluentValidation;

namespace EduTrack.Application.Features.Courses.Commands.ScheduleCourse;

/// <summary>
/// Validator for ScheduleCourseCommand
/// </summary>
public class ScheduleCourseCommandValidator : AbstractValidator<ScheduleCourseCommand>
{
    public ScheduleCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Course ID is required");

        RuleFor(x => x.Semester)
            .NotEmpty().WithMessage("Semester is required")
            .Must(semester => new[] { "Fall", "Spring", "Summer", "Winter" }.Contains(semester))
            .WithMessage("Semester must be one of: Fall, Spring, Summer, Winter");

        RuleFor(x => x.AcademicYear)
            .GreaterThanOrEqualTo(2000).WithMessage("Academic year must be 2000 or later")
            .LessThanOrEqualTo(2100).WithMessage("Academic year must be 2100 or earlier");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");

        RuleFor(x => x.EndDate)
            .Must((command, endDate) => endDate <= command.StartDate.AddYears(1))
            .WithMessage("Course duration cannot exceed one year");
    }
}
