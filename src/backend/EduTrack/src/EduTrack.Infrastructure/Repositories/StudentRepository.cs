using Microsoft.EntityFrameworkCore;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Infrastructure.Data;

namespace EduTrack.Infrastructure.Repositories
{
    /// <summary>
    /// Student repository implementation for Entity Framework
    /// </summary>
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Students.ToListAsync(cancellationToken);
        }

        public async Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            // Use EF.Property to query the database column directly without triggering value object conversion
            return await _context.Students
                .Where(s => s.Email == email)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<Student> Students, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, string? searchTerm = null, 
            StudentStatus? status = null, string? sortBy = null, 
            bool sortDescending = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Students.AsQueryable();

            // Apply filters  
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Skip search for now to avoid value object translation issues
                // TODO: Implement proper search with value objects
                query = query.Where(s => true); // No-op for now
            }

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "fullname" => sortDescending 
                    ? query.OrderByDescending(s => EF.Property<string>(s, "FullName"))
                    : query.OrderBy(s => EF.Property<string>(s, "FullName")),
                "email" => sortDescending
                    ? query.OrderByDescending(s => EF.Property<string>(s, "Email"))
                    : query.OrderBy(s => EF.Property<string>(s, "Email")),
                "enrollmentdate" => sortDescending
                    ? query.OrderByDescending(s => s.EnrollmentDate)
                    : query.OrderBy(s => s.EnrollmentDate),
                "status" => sortDescending
                    ? query.OrderByDescending(s => s.Status)
                    : query.OrderBy(s => s.Status),
                _ => sortDescending
                    ? query.OrderByDescending(s => s.Id)
                    : query.OrderBy(s => s.Id)
            };

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination
            var students = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (students, totalCount);
        }

        public async Task<(IReadOnlyList<Student> Students, int TotalCount)> GetByStatusAsync(
            StudentStatus status, int pageNumber, int pageSize, 
            string? sortBy = null, bool sortDescending = false,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Students.Where(s => s.Status == status);

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "fullname" => sortDescending 
                    ? query.OrderByDescending(s => EF.Property<string>(s, "FullName"))
                    : query.OrderBy(s => EF.Property<string>(s, "FullName")),
                "email" => sortDescending
                    ? query.OrderByDescending(s => EF.Property<string>(s, "Email"))
                    : query.OrderBy(s => EF.Property<string>(s, "Email")),
                "enrollmentdate" => sortDescending
                    ? query.OrderByDescending(s => s.EnrollmentDate)
                    : query.OrderBy(s => s.EnrollmentDate),
                _ => sortDescending
                    ? query.OrderByDescending(s => s.Id)
                    : query.OrderBy(s => s.Id)
            };

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination
            var students = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (students, totalCount);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            // Use EF.Property to query the database column directly without triggering value object conversion
            return await _context.Students
                .Where(s => EF.Property<string>(s, "Email") == email)
                .AnyAsync(cancellationToken);
        }

        public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
        {
            await _context.Students.AddAsync(student, cancellationToken);
        }

        public void Update(Student student)
        {
            _context.Students.Update(student);
        }

        public void Delete(Student student)
        {
            _context.Students.Remove(student);
        }
    }
}
