using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;
using EduTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduTrack.Infrastructure.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly ApplicationDbContext _context;

    public TeacherRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Teacher?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Teachers.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Teacher?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Teachers
            .Where(t => t.Email == email)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Teacher> Teachers, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, string? searchTerm = null,
        string? department = null, EmploymentStatus? status = null,
        string? sortBy = null, bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Teachers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(department))
            query = query.Where(t => t.Department == department);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        query = sortBy?.ToLower() switch
        {
            "fullname" => sortDescending
                ? query.OrderByDescending(t => EF.Property<string>(t, "FullName"))
                : query.OrderBy(t => EF.Property<string>(t, "FullName")),
            "department" => sortDescending
                ? query.OrderByDescending(t => t.Department)
                : query.OrderBy(t => t.Department),
            "hiredate" => sortDescending
                ? query.OrderByDescending(t => t.HireDate)
                : query.OrderBy(t => t.HireDate),
            "status" => sortDescending
                ? query.OrderByDescending(t => t.Status)
                : query.OrderBy(t => t.Status),
            _ => sortDescending
                ? query.OrderByDescending(t => t.Id)
                : query.OrderBy(t => t.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var teachers = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (teachers, totalCount);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Teachers.AnyAsync(t => t.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Teachers.AnyAsync(t => t.EmployeeId == employeeId, cancellationToken);
    }

    public async Task AddAsync(Teacher teacher, CancellationToken cancellationToken = default)
    {
        await _context.Teachers.AddAsync(teacher, cancellationToken);
    }

    public void Update(Teacher teacher)
    {
        _context.Teachers.Update(teacher);
    }

    public void Delete(Teacher teacher)
    {
        _context.Teachers.Remove(teacher);
    }
}
