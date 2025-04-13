namespace EduTrack.Infrastructure.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IStudentRepository Students { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
