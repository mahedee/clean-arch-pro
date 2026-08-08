using EduTrack.Domain.Entities;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EduTrack.Application.Features.Students.Commands.CreateStudent;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateStudentCommandHandler> _logger;

    public CreateStudentCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateStudentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var fullName = FullName.Create(request.FullName);
        var email = Email.Create(request.Email);

        var student = Student.Create(fullName, request.DateOfBirth, email);

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            var phoneNumber = PhoneNumber.Create(request.PhoneNumber);
            student.UpdatePhoneNumber(phoneNumber);
        }

        if (!string.IsNullOrEmpty(request.Street) && !string.IsNullOrEmpty(request.City))
        {
            var address = Address.Create(
                request.Street,
                request.City,
                request.State ?? string.Empty,
                request.ZipCode ?? string.Empty,
                request.Country ?? string.Empty);

            student.UpdateAddress(address);
        }

        await _unitOfWork.Students.AddAsync(student, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created student {StudentId}", student.Id);

        return student.Id;
    }
}