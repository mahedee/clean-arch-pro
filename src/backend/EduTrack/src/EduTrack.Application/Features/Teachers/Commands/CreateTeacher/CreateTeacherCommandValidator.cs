using EduTrack.Domain.Contracts.Repositories;
using FluentValidation;

namespace EduTrack.Application.Features.Teachers.Commands.CreateTeacher;

public class CreateTeacherCommandValidator : AbstractValidator<CreateTeacherCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTeacherCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MinimumLength(2).WithMessage("Full name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
            .MustAsync(BeUniqueEmail).WithMessage("A teacher with this email already exists.");

        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Employee ID is required.")
            .MaximumLength(50).WithMessage("Employee ID must not exceed 50 characters.")
            .MustAsync(BeUniqueEmployeeId).WithMessage("A teacher with this employee ID already exists.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Academic title is required.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.UtcNow.AddYears(-18)).WithMessage("Teacher must be at least 18 years old.");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _unitOfWork.Teachers.ExistsByEmailAsync(email, cancellationToken);
    }

    private async Task<bool> BeUniqueEmployeeId(string employeeId, CancellationToken cancellationToken)
    {
        return !await _unitOfWork.Teachers.ExistsByEmployeeIdAsync(employeeId, cancellationToken);
    }
}
