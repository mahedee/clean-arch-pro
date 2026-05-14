using EduTrack.Domain.Contracts.Repositories;
using MediatR;

namespace EduTrack.Application.Features.Teachers.Commands.DeleteTeacher;

public class DeleteTeacherCommandHandler : IRequestHandler<DeleteTeacherCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTeacherCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTeacherCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId, cancellationToken)
            ?? throw new KeyNotFoundException($"Teacher with ID '{request.TeacherId}' not found.");

        _unitOfWork.Teachers.Delete(teacher);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
