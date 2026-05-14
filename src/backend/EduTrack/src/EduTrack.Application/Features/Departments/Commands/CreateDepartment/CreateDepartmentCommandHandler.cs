using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using EduTrack.Domain.ValueObjects;
using MediatR;

namespace EduTrack.Application.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var codeExists = await _unitOfWork.Departments.ExistsByCodeAsync(request.Code, cancellationToken);
        if (codeExists)
            throw new InvalidOperationException($"Department with code '{request.Code}' already exists.");

        var department = Department.Create(request.Name, request.Code, request.Description);

        if (!string.IsNullOrWhiteSpace(request.Location))
            department.UpdateLocation(request.Location);

        if (!string.IsNullOrWhiteSpace(request.ContactEmail) || !string.IsNullOrWhiteSpace(request.ContactPhone))
        {
            var contactEmail = string.IsNullOrWhiteSpace(request.ContactEmail) ? null : Email.Create(request.ContactEmail);
            var contactPhone = string.IsNullOrWhiteSpace(request.ContactPhone) ? null : PhoneNumber.Create(request.ContactPhone);
            department.UpdateContactInformation(contactEmail, contactPhone);
        }

        await _unitOfWork.Departments.AddAsync(department, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return department.Id;
    }
}
