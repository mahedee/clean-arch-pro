namespace EduTrack.Application.Features.Courses.DTOs;

/// <summary>
/// Course data transfer object for API responses
/// </summary>
public class CourseDto
{
    /// <summary>
    /// Course unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Course title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Course code (e.g., "CS101", "MATH201")
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Course description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Number of credit hours
    /// </summary>
    public int CreditHours { get; set; }

    /// <summary>
    /// Course level (Undergraduate, Graduate, etc.)
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// Department offering the course
    /// </summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Maximum enrollment capacity
    /// </summary>
    public int MaxEnrollment { get; set; }

    /// <summary>
    /// Current enrollment count
    /// </summary>
    public int CurrentEnrollment { get; set; }

    /// <summary>
    /// Prerequisite credit hours required
    /// </summary>
    public int PrerequisiteCreditHours { get; set; }

    /// <summary>
    /// Course status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Academic semester
    /// </summary>
    public string Semester { get; set; } = string.Empty;

    /// <summary>
    /// Academic year
    /// </summary>
    public int AcademicYear { get; set; }

    /// <summary>
    /// Course start date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Course end date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Record creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Record last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Simplified course DTO for list views
/// </summary>
public class CourseListDto
{
    /// <summary>
    /// Course unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Course title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Course code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Number of credit hours
    /// </summary>
    public int CreditHours { get; set; }

    /// <summary>
    /// Course level
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// Department
    /// </summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Current enrollment / Max enrollment
    /// </summary>
    public string Enrollment { get; set; } = string.Empty;

    /// <summary>
    /// Course status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Academic period (e.g., "Fall 2025")
    /// </summary>
    public string AcademicPeriod { get; set; } = string.Empty;
}

/// <summary>
/// DTO for creating courses
/// </summary>
public class CreateCourseDto
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
    /// Course code
    /// </summary>
    public string CourseCode { get; set; } = string.Empty;

    /// <summary>
    /// Number of credit hours
    /// </summary>
    public int Credits { get; set; }

    /// <summary>
    /// Maximum enrollment capacity
    /// </summary>
    public int MaxCapacity { get; set; }

    /// <summary>
    /// Department
    /// </summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Course level
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// Academic period
    /// </summary>
    public string? AcademicPeriod { get; set; }

    /// <summary>
    /// List of prerequisite course IDs
    /// </summary>
    public List<Guid>? Prerequisites { get; set; }
}

/// <summary>
/// DTO for updating courses
/// </summary>
public class UpdateCourseDto
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
    /// Course code
    /// </summary>
    public string CourseCode { get; set; } = string.Empty;

    /// <summary>
    /// Number of credit hours
    /// </summary>
    public int Credits { get; set; }

    /// <summary>
    /// Maximum enrollment capacity
    /// </summary>
    public int MaxCapacity { get; set; }

    /// <summary>
    /// Department
    /// </summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Course level
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// Prerequisite credit hours
    /// </summary>
    public int PrerequisiteCreditHours { get; set; }
}

/// <summary>
/// DTO for student enrollment in courses
/// </summary>
public class EnrollmentDto
{
    /// <summary>
    /// Course ID
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Student ID
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Student full name
    /// </summary>
    public string StudentName { get; set; } = string.Empty;

    /// <summary>
    /// Student email
    /// </summary>
    public string StudentEmail { get; set; } = string.Empty;

    /// <summary>
    /// Enrollment date
    /// </summary>
    public DateTime EnrollmentDate { get; set; }

    /// <summary>
    /// Enrollment status
    /// </summary>
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Paginated result for course lists
/// </summary>
public class PaginatedCourseListDto
{
    /// <summary>
    /// List of courses
    /// </summary>
    public List<CourseListDto> Courses { get; set; } = new();

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of courses
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage { get; set; }
}
