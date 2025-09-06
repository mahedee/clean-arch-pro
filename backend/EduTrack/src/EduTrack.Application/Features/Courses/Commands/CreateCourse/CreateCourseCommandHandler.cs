using MediatR;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Repositories;

namespace EduTrack.Application.Features.Courses.Commands.CreateCourse;

/// <summary>
/// Handler for CreateCourseCommand
/// </summary>
public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        // Parse course level from string
        if (!Enum.TryParse<CourseLevel>(request.Level, true, out var courseLevel))
        {
            throw new ArgumentException($"Invalid course level: {request.Level}");
        }

        // Create new course entity
        var course = Course.Create(
            request.Title,
            request.CourseCode,
            request.Description,
            request.Credits,
            courseLevel,
            request.Department,
            request.MaxCapacity
        );

        // Set prerequisite requirement if provided
        if (request.Prerequisites != null && request.Prerequisites.Any())
        {
            // For simplicity, we'll set the prerequisite credit hours based on the number of prerequisites
            // In a real scenario, you'd look up the actual credit hours for each prerequisite course
            var totalPrerequisiteCredits = request.Prerequisites.Count * 3; // Assuming 3 credits per prerequisite
            course.SetPrerequisiteRequirement(totalPrerequisiteCredits);
        }

        // Schedule the course if academic period is provided
        if (!string.IsNullOrEmpty(request.AcademicPeriod))
        {
            // Parse academic period (e.g., "Fall 2024" -> semester="Fall", year=2024)
            var parts = request.AcademicPeriod.Split(' ');
            if (parts.Length == 2 && int.TryParse(parts[1], out var year))
            {
                var semester = parts[0];
                var startDate = DateTime.Now.AddMonths(1); // Default start date
                var endDate = startDate.AddMonths(4); // Default end date (4 months later)
                
                course.Schedule(semester, year, startDate, endDate);
            }
        }

        // Add to repository
        await _unitOfWork.Courses.AddAsync(course);
        
        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return course.Id;
    }
}
