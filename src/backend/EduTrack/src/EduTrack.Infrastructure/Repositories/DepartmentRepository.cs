using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;
using EduTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduTrack.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDbContext _context;

    public DepartmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Departments.FirstOrDefaultAsync(d => d.Code == code, cancellationToken);
    }

    public async Task<(IReadOnlyList<Department> Departments, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, string? searchTerm = null,
        DepartmentStatus? status = null, string? sortBy = null,
        bool sortDescending = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Departments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(d => d.Name.Contains(searchTerm) || d.Code.Contains(searchTerm));

        if (status.HasValue)
            query = query.Where(d => d.Status == status.Value);

        query = sortBy?.ToLower() switch
        {
            "name" => sortDescending
                ? query.OrderByDescending(d => d.Name)
                : query.OrderBy(d => d.Name),
            "code" => sortDescending
                ? query.OrderByDescending(d => d.Code)
                : query.OrderBy(d => d.Code),
            "status" => sortDescending
                ? query.OrderByDescending(d => d.Status)
                : query.OrderBy(d => d.Status),
            _ => sortDescending
                ? query.OrderByDescending(d => d.Id)
                : query.OrderBy(d => d.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var departments = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (departments, totalCount);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Departments.AnyAsync(d => d.Code == code, cancellationToken);
    }

    public async Task AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        await _context.Departments.AddAsync(department, cancellationToken);
    }

    public void Update(Department department)
    {
        _context.Departments.Update(department);
    }

    public void Delete(Department department)
    {
        _context.Departments.Remove(department);
    }
}
