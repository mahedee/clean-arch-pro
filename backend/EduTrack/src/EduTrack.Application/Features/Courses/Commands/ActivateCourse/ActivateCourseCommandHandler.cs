using MediatR;
using EduTrack.Domain.Repositories;

namespace EduTrack.Application.Features.Courses.Commands.ActivateCourse;

/// <summary>
/// Handler for ActivateCourseCommand
/// </summary>
public class ActivateCourseCommandHandler : IRequestHandler<ActivateCourseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivateCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ActivateCourseCommand request, CancellationToken cancellationToken)
    {
        // Get existing course
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return false; // Course not found
        }

        // Activate the course
        course.Activate();

        // Update in repository
        _unitOfWork.Courses.Update(course);
        
        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
