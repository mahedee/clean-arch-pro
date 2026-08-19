using EduTrack.Domain.Entities;

namespace EduTrack.Domain.Contracts.Repositories
{
    public interface IFeedbackRepository
    {
        Task<List<Feedback>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<Feedback>> GetUnreadAsync(CancellationToken cancellationToken = default);
        Task<Feedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Feedback feedback, CancellationToken cancellationToken = default);
        void Update(Feedback feedback);
    }
}
