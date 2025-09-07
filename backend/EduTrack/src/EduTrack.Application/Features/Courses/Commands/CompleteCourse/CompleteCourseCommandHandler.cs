using MediatR;
using EduTrack.Domain.Repositories;

namespace EduTrack.Application.Features.Courses.Commands.CompleteCourse;

/// <summary>
/// Handler for CompleteCourseCommand
/// </summary>
public class CompleteCourseCommandHandler : IRequestHandler<CompleteCourseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public CompleteCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CompleteCourseCommand request, CancellationToken cancellationToken)
    {
        // Get existing course
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return false; // Course not found
        }

        // Complete the course
        course.Complete();

        // Update in repository
        _unitOfWork.Courses.Update(course);
        
        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
