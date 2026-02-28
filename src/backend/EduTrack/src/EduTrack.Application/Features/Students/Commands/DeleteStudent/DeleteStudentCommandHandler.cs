using EduTrack.Domain.Contracts.Repositories;
using MediatR;

namespace EduTrack.Application.Features.Students.Commands.DeleteStudent;

/// <summary>
/// Handler for DeleteStudentCommand
/// </summary>
public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStudentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"DEBUG: Attempting to delete student with ID: {request.StudentId}");
            
            // Get the student to verify it exists
            var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId, cancellationToken);
            
            if (student == null)
            {
                Console.WriteLine($"DEBUG: Student not found with ID: {request.StudentId}");
                throw new InvalidOperationException($"Student with ID '{request.StudentId}' not found");
            }

            Console.WriteLine($"DEBUG: Found student: {student.FullName.Value}, proceeding with delete");
            
            // Delete the student
            _unitOfWork.Students.Delete(student);
            
            Console.WriteLine($"DEBUG: Called Delete method, now saving changes");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            Console.WriteLine($"DEBUG: Successfully deleted student {student.FullName.Value}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DEBUG: Exception occurred: {ex.Message}");
            Console.WriteLine($"DEBUG: Stack trace: {ex.StackTrace}");
            Console.WriteLine($"DEBUG: Inner exception: {ex.InnerException?.Message}");
            throw new InvalidOperationException($"Error deleting student with ID: {request.StudentId}", ex);
        }
    }
}
