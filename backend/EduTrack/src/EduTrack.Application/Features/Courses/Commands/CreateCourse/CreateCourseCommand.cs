using MediatR;

namespace EduTrack.Application.Features.Courses.Commands.CreateCourse;

/// <summary>
/// Command to create a new course in the system
/// </summary>
public class CreateCourseCommand : IRequest<Guid>
{
    /// <summary>
    /// Course title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Course description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Course code (e.g., CS101, MATH201)
    /// </summary>
    public string CourseCode { get; set; } = string.Empty;

    /// <summary>
    /// Number of credits for this course
    /// </summary>
    public int Credits { get; set; }

    /// <summary>
    /// Maximum number of students that can enroll
    /// </summary>
    public int MaxCapacity { get; set; }

    /// <summary>
    /// Department offering this course
    /// </summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Course level (e.g., Undergraduate, Graduate)
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// Course prerequisites (course IDs)
    /// </summary>
    public List<Guid> Prerequisites { get; set; } = new();

    /// <summary>
    /// Academic year/semester for the course
    /// </summary>
    public string AcademicPeriod { get; set; } = string.Empty;
}
