using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using EduTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduTrack.Infrastructure.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FeedbackRepository> _logger;

        public FeedbackRepository(ApplicationDbContext context, ILogger<FeedbackRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Feedback>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Feedbacks
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Feedback>> GetUnreadAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Feedbacks
                .Where(f => !f.IsRead)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Feedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Fetching Feedback by ID {FeedbackId}", id);
            return await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task AddAsync(Feedback feedback, CancellationToken cancellationToken = default)
        {
            await _context.Feedbacks.AddAsync(feedback, cancellationToken);
        }

        public void Update(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
        }
    }
}
