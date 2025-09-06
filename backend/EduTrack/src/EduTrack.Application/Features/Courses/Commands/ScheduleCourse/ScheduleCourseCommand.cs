using MediatR;

namespace EduTrack.Application.Features.Courses.Commands.ScheduleCourse;

/// <summary>
/// Command to schedule a course for a specific academic period
/// </summary>
public class ScheduleCourseCommand : IRequest<bool>
{
    /// <summary>
    /// Course ID to schedule
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Academic semester (e.g., "Fall", "Spring", "Summer")
    /// </summary>
    public string Semester { get; set; } = string.Empty;

    /// <summary>
    /// Academic year (e.g., 2024)
    /// </summary>
    public int AcademicYear { get; set; }

    /// <summary>
    /// Course start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Course end date
    /// </summary>
    public DateTime EndDate { get; set; }
}
