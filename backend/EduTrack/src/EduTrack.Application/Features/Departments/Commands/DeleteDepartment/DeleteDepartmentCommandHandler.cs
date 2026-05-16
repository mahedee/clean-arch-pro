using EduTrack.Domain.Contracts.Repositories;
using MediatR;

namespace EduTrack.Application.Features.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDepartmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Department with ID '{request.DepartmentId}' not found.");

        _unitOfWork.Departments.Delete(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
