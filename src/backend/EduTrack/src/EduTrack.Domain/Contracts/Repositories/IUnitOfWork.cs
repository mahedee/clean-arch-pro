namespace EduTrack.Domain.Contracts.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IStudentRepository Students { get; }
        ICourseRepository Courses { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}