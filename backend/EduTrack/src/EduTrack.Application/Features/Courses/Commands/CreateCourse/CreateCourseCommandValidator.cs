using FluentValidation;

namespace EduTrack.Application.Features.Courses.Commands.CreateCourse;

/// <summary>
/// Validator for CreateCourseCommand
/// </summary>
public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Course title is required")
            .MaximumLength(200).WithMessage("Course title cannot exceed 200 characters")
            .MinimumLength(3).WithMessage("Course title must be at least 3 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Course description is required")
            .MaximumLength(2000).WithMessage("Course description cannot exceed 2000 characters")
            .MinimumLength(10).WithMessage("Course description must be at least 10 characters");

        RuleFor(x => x.CourseCode)
            .NotEmpty().WithMessage("Course code is required")
            .MaximumLength(20).WithMessage("Course code cannot exceed 20 characters")
            .Matches(@"^[A-Z]{2,4}\d{3,4}$").WithMessage("Course code must be in format like 'CS101' or 'MATH2010'");

        RuleFor(x => x.Credits)
            .GreaterThan(0).WithMessage("Credits must be greater than 0")
            .LessThanOrEqualTo(12).WithMessage("Credits cannot exceed 12");

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Max capacity must be greater than 0")
            .LessThanOrEqualTo(500).WithMessage("Max capacity cannot exceed 500 students");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required")
            .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters");

        RuleFor(x => x.Level)
            .NotEmpty().WithMessage("Course level is required")
            .Must(level => new[] { "Undergraduate", "Graduate", "Doctorate", "Certificate" }.Contains(level))
            .WithMessage("Course level must be one of: Undergraduate, Graduate, Doctorate, Certificate");

        RuleFor(x => x.AcademicPeriod)
            .NotEmpty().WithMessage("Academic period is required")
            .MaximumLength(50).WithMessage("Academic period cannot exceed 50 characters");

        RuleFor(x => x.Prerequisites)
            .Must(prerequisites => prerequisites == null || prerequisites.Count <= 10)
            .WithMessage("A course cannot have more than 10 prerequisites");
    }
}
