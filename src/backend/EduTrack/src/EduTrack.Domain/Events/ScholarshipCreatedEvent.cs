using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a new scholarship is created
    /// </summary>
    public class ScholarshipCreatedEvent : DomainEvent
    {
        public Guid ScholarshipId { get; }
        public string Name { get; }
        public decimal Amount { get; }
        public int MaxRecipients { get; }

        public ScholarshipCreatedEvent(Guid scholarshipId, string name, decimal amount, int maxRecipients)
        {
            ScholarshipId = scholarshipId;
            Name = name;
            Amount = amount;
            MaxRecipients = maxRecipients;
        }
    }
}
