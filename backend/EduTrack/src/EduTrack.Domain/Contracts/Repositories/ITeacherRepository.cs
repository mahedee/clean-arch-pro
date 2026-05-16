using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;

namespace EduTrack.Domain.Contracts.Repositories
{
    public interface ITeacherRepository
    {
        Task<Teacher?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Teacher?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Teacher> Teachers, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, string? searchTerm = null,
            string? department = null, EmploymentStatus? status = null,
            string? sortBy = null, bool sortDescending = false,
            CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default);
        Task AddAsync(Teacher teacher, CancellationToken cancellationToken = default);
        void Update(Teacher teacher);
        void Delete(Teacher teacher);
    }
}
