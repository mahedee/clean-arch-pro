using EduTrack.Domain.Common;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Represents a feedback submission from any user (anonymous or named)
    /// </summary>
    public class Feedback : BaseEntity<Guid>
    {
        /// <summary>
        /// Optional name of the person submitting feedback. Null means anonymous.
        /// </summary>
        public string? Name { get; private set; }

        /// <summary>
        /// The feedback message content
        /// </summary>
        public string Message { get; private set; } = null!;

        /// <summary>
        /// Indicates whether this feedback has been read by an administrator
        /// </summary>
        public bool IsRead { get; private set; }

        /// <summary>
        /// When the feedback was marked as read
        /// </summary>
        public DateTime? ReadAt { get; private set; }

        // Private constructor for EF Core
        private Feedback() : base()
        {
        }

        /// <summary>
        /// Create a new feedback submission
        /// </summary>
        /// <param name="message">The feedback message</param>
        /// <param name="name">Optional name; null means anonymous</param>
        public static Feedback Create(string message, string? name = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Feedback message cannot be empty.", nameof(message));

            return new Feedback
            {
                Id = Guid.NewGuid(),
                Message = message.Trim(),
                Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim(),
                IsRead = false
            };
        }

        /// <summary>
        /// Mark this feedback as read
        /// </summary>
        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
                ReadAt = DateTime.UtcNow;
                MarkAsUpdated();
            }
        }
    }
}
