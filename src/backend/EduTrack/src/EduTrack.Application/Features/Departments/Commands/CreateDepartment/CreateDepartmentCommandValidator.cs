using EduTrack.Domain.Contracts.Repositories;
using FluentValidation;

namespace EduTrack.Application.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Department name is required.")
            .MaximumLength(100).WithMessage("Department name must not exceed 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Department code is required.")
            .MaximumLength(20).WithMessage("Department code must not exceed 20 characters.")
            .MustAsync(BeUniqueCode).WithMessage("A department with this code already exists.");

        RuleFor(x => x.ContactEmail)
            .EmailAddress().WithMessage("A valid contact email is required.")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));
    }

    private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
    {
        return !await _unitOfWork.Departments.ExistsByCodeAsync(code, cancellationToken);
    }
}
