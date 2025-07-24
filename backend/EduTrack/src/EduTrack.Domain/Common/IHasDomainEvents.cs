namespace EduTrack.Domain.Common
{
    /// <summary>
    /// Interface for entities that can raise domain events
    /// </summary>
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<DomainEvent> DomainEvents { get; }
        void AddDomainEvent(DomainEvent domainEvent);
        void RemoveDomainEvent(DomainEvent domainEvent);
        void ClearDomainEvents();
    }
}
