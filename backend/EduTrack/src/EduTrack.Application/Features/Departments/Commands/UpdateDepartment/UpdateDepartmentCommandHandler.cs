using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.ValueObjects;
using MediatR;

namespace EduTrack.Application.Features.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Department with ID '{request.Id}' not found.");

        if (!string.IsNullOrWhiteSpace(request.Name))
            department.UpdateInformation(request.Name, request.Description);

        if (!string.IsNullOrWhiteSpace(request.Location))
            department.UpdateLocation(request.Location);

        if (request.ContactEmail != null || request.ContactPhone != null)
        {
            var contactEmail = string.IsNullOrWhiteSpace(request.ContactEmail) ? null : Email.Create(request.ContactEmail);
            var contactPhone = string.IsNullOrWhiteSpace(request.ContactPhone) ? null : PhoneNumber.Create(request.ContactPhone);
            department.UpdateContactInformation(contactEmail, contactPhone);
        }

        _unitOfWork.Departments.Update(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
