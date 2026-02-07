using MediatR;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Contracts.Repositories;

namespace EduTrack.Application.Features.Courses.Commands.UpdateCourse;

/// <summary>
/// Handler for UpdateCourseCommand
/// </summary>
public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        // Get existing course
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return false; // Course not found
        }

        // Parse course level from string
        if (!Enum.TryParse<CourseLevel>(request.Level, true, out var courseLevel))
        {
            throw new ArgumentException($"Invalid course level: {request.Level}");
        }

        // Update course properties using reflection to access private setters
        // Note: In a real application, you might want to add Update methods to the entity
        var titleProperty = typeof(Course).GetProperty("Title");
        var descriptionProperty = typeof(Course).GetProperty("Description");
        var codeProperty = typeof(Course).GetProperty("Code");
        var creditHoursProperty = typeof(Course).GetProperty("CreditHours");
        var maxEnrollmentProperty = typeof(Course).GetProperty("MaxEnrollment");
        var departmentProperty = typeof(Course).GetProperty("Department");
        var levelProperty = typeof(Course).GetProperty("Level");

        titleProperty?.SetValue(course, request.Title);
        descriptionProperty?.SetValue(course, request.Description);
        codeProperty?.SetValue(course, request.CourseCode);
        creditHoursProperty?.SetValue(course, request.Credits);
        maxEnrollmentProperty?.SetValue(course, request.MaxCapacity);
        departmentProperty?.SetValue(course, request.Department);
        levelProperty?.SetValue(course, courseLevel);

        // Set prerequisite requirement
        course.SetPrerequisiteRequirement(request.PrerequisiteCreditHours);

        // Update in repository
        _unitOfWork.Courses.Update(course);
        
        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
