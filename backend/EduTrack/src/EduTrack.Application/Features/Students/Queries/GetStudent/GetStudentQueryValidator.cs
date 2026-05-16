using FluentValidation;

namespace EduTrack.Application.Features.Students.Queries.GetStudent;

/// <summary>
/// Validator for GetStudentQuery
/// </summary>
public class GetStudentQueryValidator : AbstractValidator<GetStudentQuery>
{
    public GetStudentQueryValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty()
            .WithMessage("Student ID is required")
            .NotEqual(Guid.Empty)
            .WithMessage("Student ID cannot be empty");
    }
}
