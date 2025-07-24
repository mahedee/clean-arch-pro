using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrack.Domain.Common
{
    /// <summary>
    /// Base class for all domain events
    /// </summary>
    public abstract class DomainEvent
    {
        public DateTime OccurredOn { get; protected set; }
        public Guid EventId { get; protected set; }

        protected DomainEvent()
        {
            EventId = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }
    }
}
