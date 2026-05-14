using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Enums;
using MediatR;

namespace EduTrack.Application.Features.Teachers.Commands.UpdateTeacher;

public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTeacherCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Teacher with ID '{request.Id}' not found.");

        if (!string.IsNullOrWhiteSpace(request.Email) || !string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var email = request.Email ?? teacher.Email.Value;
            teacher.UpdateContactInformation(email, request.PhoneNumber);
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            if (Enum.TryParse<AcademicTitle>(request.Title, out var academicTitle))
                teacher.UpdateTitle(academicTitle);
        }

        teacher.SetOfficeInfo(request.OfficeLocation, request.OfficeHours);

        _unitOfWork.Teachers.Update(teacher);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
