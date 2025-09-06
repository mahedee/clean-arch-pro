using AutoMapper;
using EduTrack.Domain.Repositories;
using EduTrack.Domain.ValueObjects;
using MediatR;

namespace EduTrack.Application.Features.Students.Commands.UpdateStudent;

/// <summary>
/// Handler for UpdateStudentCommand
/// </summary>
public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateStudentCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing student
            var student = await _unitOfWork.Students.GetByIdAsync(request.Id, cancellationToken);
            if (student == null)
            {
                throw new InvalidOperationException($"Student with ID '{request.Id}' not found");
            }

            // Update full name if provided
            if (!string.IsNullOrEmpty(request.FullName))
            {
                var fullName = FullName.Create(request.FullName);
                student.UpdateFullName(fullName);
            }

            // Update email if provided
            if (!string.IsNullOrEmpty(request.Email))
            {
                // Check if new email already exists for another student
                var existingStudent = await _unitOfWork.Students.GetByEmailAsync(request.Email, cancellationToken);
                if (existingStudent != null && existingStudent.Id != request.Id)
                {
                    throw new InvalidOperationException($"Email '{request.Email}' already exists for another student");
                }

                var email = Email.Create(request.Email);
                student.UpdateContactInformation(email);
            }

            // Update phone number if provided
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                var phoneNumber = PhoneNumber.Create(request.PhoneNumber);
                student.UpdatePhoneNumber(phoneNumber);
            }

            // Update address if provided
            if (HasAddressData(request))
            {
                var address = Address.Create(
                    request.Street!,
                    request.City!,
                    request.State!,
                    request.ZipCode!,
                    request.Country!);
                
                student.UpdateAddress(address);
            }

            // Update GPA if provided
            if (request.GPA.HasValue)
            {
                var gpa = GPA.Create(request.GPA.Value);
                student.UpdateGPA(gpa);
            }

            // Save changes
            _unitOfWork.Students.Update(student);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error updating student with ID: {request.Id}", ex);
        }
    }

    private static bool HasAddressData(UpdateStudentCommand request)
    {
        return !string.IsNullOrEmpty(request.Street) ||
               !string.IsNullOrEmpty(request.City) ||
               !string.IsNullOrEmpty(request.State) ||
               !string.IsNullOrEmpty(request.ZipCode) ||
               !string.IsNullOrEmpty(request.Country);
    }
}
