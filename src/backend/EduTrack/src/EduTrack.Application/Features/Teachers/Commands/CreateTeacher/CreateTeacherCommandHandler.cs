using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;
using MediatR;

namespace EduTrack.Application.Features.Teachers.Commands.CreateTeacher;

public class CreateTeacherCommandHandler : IRequestHandler<CreateTeacherCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTeacherCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
    {
        var existingTeacher = await _unitOfWork.Teachers.GetByEmailAsync(request.Email, cancellationToken);
        if (existingTeacher != null)
            throw new InvalidOperationException($"Teacher with email '{request.Email}' already exists.");

        var emailExists = await _unitOfWork.Teachers.ExistsByEmployeeIdAsync(request.EmployeeId, cancellationToken);
        if (emailExists)
            throw new InvalidOperationException($"Teacher with employee ID '{request.EmployeeId}' already exists.");

        if (!Enum.TryParse<AcademicTitle>(request.Title, out var academicTitle))
            academicTitle = AcademicTitle.Lecturer;

        var teacher = Teacher.Create(
            request.FullName,
            request.Email,
            request.EmployeeId,
            request.Department,
            academicTitle,
            request.DateOfBirth);

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            teacher.UpdateContactInformation(request.Email, request.PhoneNumber);

        await _unitOfWork.Teachers.AddAsync(teacher, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return teacher.Id;
    }
}
