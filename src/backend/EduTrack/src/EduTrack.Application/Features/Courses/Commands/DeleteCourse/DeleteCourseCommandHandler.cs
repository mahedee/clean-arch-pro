using MediatR;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Contracts.Repositories;

namespace EduTrack.Application.Features.Courses.Commands.DeleteCourse;

/// <summary>
/// Handler for DeleteCourseCommand
/// </summary>
public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        // Check if course exists
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return false; // Course not found
        }

        // Additional business rules can be added here
        // For example: Check if course has active enrollments
        // if (course.HasActiveEnrollments())
        // {
        //     throw new InvalidOperationException("Cannot delete course with active enrollments");
        // }

        // Delete the course
        _unitOfWork.Courses.Delete(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}