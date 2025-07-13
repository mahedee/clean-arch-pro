using EduTrack.Domain.Entities;

namespace EduTrack.Domain.Repositories
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Student student, CancellationToken cancellationToken = default);
        void Update(Student student);
        void Delete(Student student);
    }
}
