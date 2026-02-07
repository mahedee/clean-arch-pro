using EduTrack.Domain.Entities;

namespace EduTrack.Domain.Contracts.Repositories
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Student> Students, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, string? searchTerm = null, 
            StudentStatus? status = null, string? sortBy = null, 
            bool sortDescending = false, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Student> Students, int TotalCount)> GetByStatusAsync(
            StudentStatus status, int pageNumber, int pageSize, 
            string? sortBy = null, bool sortDescending = false,
            CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddAsync(Student student, CancellationToken cancellationToken = default);
        void Update(Student student);
        void Delete(Student student);
    }
}
