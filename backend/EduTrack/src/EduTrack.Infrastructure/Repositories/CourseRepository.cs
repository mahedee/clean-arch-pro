using Microsoft.EntityFrameworkCore;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Repositories;
using EduTrack.Infrastructure.Data;

namespace EduTrack.Infrastructure.Repositories
{
    /// <summary>
    /// Course repository implementation for Entity Framework
    /// </summary>
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
        }

        public async Task<Course?> GetByIdAsync(Guid id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<Course?> GetByCodeAsync(string code)
        {
            return await _context.Courses
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<(IEnumerable<Course> Courses, int TotalCount)> GetPaginatedAsync(
            int pageNumber, 
            int pageSize, 
            string? searchTerm = null,
            string? department = null,
            string? level = null,
            string? status = null,
            string sortBy = "Title",
            string sortDirection = "asc")
        {
            var query = _context.Courses.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.Title.Contains(searchTerm) || c.Code.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(c => c.Department == department);
            }

            if (!string.IsNullOrWhiteSpace(level) && Enum.TryParse<CourseLevel>(level, true, out var courseLevel))
            {
                query = query.Where(c => c.Level == courseLevel);
            }

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<CourseStatus>(status, true, out var courseStatus))
            {
                query = query.Where(c => c.Status == courseStatus);
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "code" => sortDirection.ToLower() == "desc" 
                    ? query.OrderByDescending(c => c.Code)
                    : query.OrderBy(c => c.Code),
                "department" => sortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.Department)
                    : query.OrderBy(c => c.Department),
                "credithours" => sortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.CreditHours)
                    : query.OrderBy(c => c.CreditHours),
                "level" => sortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.Level)
                    : query.OrderBy(c => c.Level),
                "status" => sortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.Status)
                    : query.OrderBy(c => c.Status),
                _ => sortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.Title)
                    : query.OrderBy(c => c.Title)
            };

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var courses = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (courses, totalCount);
        }

        public async Task<IEnumerable<Course>> GetByDepartmentAsync(string department)
        {
            return await _context.Courses
                .Where(c => c.Department == department)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetByLevelAsync(CourseLevel level)
        {
            return await _context.Courses
                .Where(c => c.Level == level)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetByStatusAsync(CourseStatus status)
        {
            return await _context.Courses
                .Where(c => c.Status == status)
                .ToListAsync();
        }

        public void Update(Course course)
        {
            _context.Courses.Update(course);
        }

        public void Delete(Course course)
        {
            _context.Courses.Remove(course);
        }

        public async Task<bool> ExistsAsync(string code)
        {
            return await _context.Courses
                .AnyAsync(c => c.Code == code);
        }
    }
}
