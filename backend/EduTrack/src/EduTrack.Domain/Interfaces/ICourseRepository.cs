using EduTrack.Domain.Entities;

namespace EduTrack.Domain.Repositories
{
    /// <summary>
    /// Course repository interface for data access operations
    /// </summary>
    public interface ICourseRepository
    {
        /// <summary>
        /// Add a new course
        /// </summary>
        /// <param name="course">Course entity to add</param>
        Task AddAsync(Course course);

        /// <summary>
        /// Get course by ID
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <returns>Course entity or null if not found</returns>
        Task<Course?> GetByIdAsync(Guid id);

        /// <summary>
        /// Get course by code
        /// </summary>
        /// <param name="code">Course code</param>
        /// <returns>Course entity or null if not found</returns>
        Task<Course?> GetByCodeAsync(string code);

        /// <summary>
        /// Get all courses
        /// </summary>
        /// <returns>List of all courses</returns>
        Task<IEnumerable<Course>> GetAllAsync();

        /// <summary>
        /// Get paginated courses with filtering and sorting
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="searchTerm">Search term for title or code</param>
        /// <param name="department">Department filter</param>
        /// <param name="level">Level filter</param>
        /// <param name="status">Status filter</param>
        /// <param name="sortBy">Sort field</param>
        /// <param name="sortDirection">Sort direction</param>
        /// <returns>Paginated course results</returns>
        Task<(IEnumerable<Course> Courses, int TotalCount)> GetPaginatedAsync(
            int pageNumber, 
            int pageSize, 
            string? searchTerm = null,
            string? department = null,
            string? level = null,
            string? status = null,
            string sortBy = "Title",
            string sortDirection = "asc");

        /// <summary>
        /// Get courses by department
        /// </summary>
        /// <param name="department">Department name</param>
        /// <returns>List of courses in the department</returns>
        Task<IEnumerable<Course>> GetByDepartmentAsync(string department);

        /// <summary>
        /// Get courses by level
        /// </summary>
        /// <param name="level">Course level</param>
        /// <returns>List of courses at the specified level</returns>
        Task<IEnumerable<Course>> GetByLevelAsync(CourseLevel level);

        /// <summary>
        /// Get courses by status
        /// </summary>
        /// <param name="status">Course status</param>
        /// <returns>List of courses with the specified status</returns>
        Task<IEnumerable<Course>> GetByStatusAsync(CourseStatus status);

        /// <summary>
        /// Update an existing course
        /// </summary>
        /// <param name="course">Course entity to update</param>
        void Update(Course course);

        /// <summary>
        /// Delete a course
        /// </summary>
        /// <param name="course">Course entity to delete</param>
        void Delete(Course course);

        /// <summary>
        /// Check if course code exists
        /// </summary>
        /// <param name="code">Course code to check</param>
        /// <returns>True if code exists, false otherwise</returns>
        Task<bool> ExistsAsync(string code);
    }
}
