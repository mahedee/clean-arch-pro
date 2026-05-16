using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;

namespace EduTrack.Domain.Contracts.Repositories
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Department> Departments, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, string? searchTerm = null,
            DepartmentStatus? status = null, string? sortBy = null,
            bool sortDescending = false, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task AddAsync(Department department, CancellationToken cancellationToken = default);
        void Update(Department department);
        void Delete(Department department);
    }
}
