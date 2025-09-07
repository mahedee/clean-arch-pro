using AutoMapper;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Repositories;
using EduTrack.Domain.ValueObjects;
using MediatR;

namespace EduTrack.Application.Features.Students.Commands.CreateStudent;

/// <summary>
/// Handler for CreateStudentCommand
/// </summary>
public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateStudentCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Debug logging
            Console.WriteLine($"DEBUG: Handler received FullName: '{request.FullName}'");
            Console.WriteLine($"DEBUG: Handler received Email: '{request.Email}'");
            
            // Check if email already exists
            var existingStudent = await _unitOfWork.Students.GetByEmailAsync(request.Email, cancellationToken);
            if (existingStudent != null)
            {
                throw new InvalidOperationException($"Student with email '{request.Email}' already exists");
            }

            // Create value objects
            Console.WriteLine($"DEBUG: About to create FullName with value: '{request.FullName}'");
            var fullName = FullName.Create(request.FullName);
            Console.WriteLine($"DEBUG: Created FullName: '{fullName.Value}'");
            
            var email = Email.Create(request.Email);

            // Create student entity
            var student = Student.Create(fullName, request.DateOfBirth, email);

            // Set optional properties
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

            // Add to repository
            await _unitOfWork.Students.AddAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return student.Id;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error creating student with email: {request.Email}", ex);
        }
    }
}