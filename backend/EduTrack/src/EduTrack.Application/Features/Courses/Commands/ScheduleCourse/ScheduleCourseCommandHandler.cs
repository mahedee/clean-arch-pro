using MediatR;
using EduTrack.Domain.Repositories;

namespace EduTrack.Application.Features.Courses.Commands.ScheduleCourse;

/// <summary>
/// Handler for ScheduleCourseCommand
/// </summary>
public class ScheduleCourseCommandHandler : IRequestHandler<ScheduleCourseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ScheduleCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ScheduleCourseCommand request, CancellationToken cancellationToken)
    {
        // Get existing course
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return false; // Course not found
        }

        // Schedule the course
        course.Schedule(
            request.Semester,
            request.AcademicYear,
            request.StartDate,
            request.EndDate);

        // Update in repository
        _unitOfWork.Courses.Update(course);
        
        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
