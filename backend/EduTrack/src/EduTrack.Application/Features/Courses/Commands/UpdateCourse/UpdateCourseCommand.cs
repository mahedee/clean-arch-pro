using MediatR;

namespace EduTrack.Application.Features.Courses.Commands.UpdateCourse;

/// <summary>
/// Command to update an existing course
/// </summary>
public class UpdateCourseCommand : IRequest<bool>
{
    /// <summary>
    /// Course ID to update
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Course title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Course description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Course code (e.g., "CS101", "MATH201")
    /// </summary>
    public string CourseCode { get; set; } = string.Empty;

    /// <summary>
    /// Number of credit hours
    /// </summary>
    public int Credits { get; set; }

    /// <summary>
    /// Maximum number of students that can enroll
    /// </summary>
    public int MaxCapacity { get; set; }

    /// <summary>
    /// Department offering the course
    /// </summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Course level (Undergraduate, Graduate, Postgraduate, Doctoral, Certificate, Continuing)
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// Prerequisite credit hours requirement
    /// </summary>
    public int PrerequisiteCreditHours { get; set; }
}
