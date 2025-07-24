using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrack.Domain.Common
{
    /// <summary>
    /// Base entity class providing common properties and domain event functionality
    /// </summary>
    /// <typeparam name="TId">The type of the entity identifier</typeparam>
    /// 
    /// IEquality is implemented to allow comparison of entities based on their Ids.
    /// This class includes properties for tracking creation, updates, and soft deletion
     
    public abstract class BaseEntity<TId> : IHasDomainEvents, IEquatable<BaseEntity<TId>>
        where TId : notnull
    {
        private readonly List<DomainEvent> _domainEvents = new();

        /// <summary>
        /// The unique identifier for this entity
        /// </summary>
        public virtual TId Id { get; protected set; } = default!;

        /// <summary>
        /// When this entity was created
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// When this entity was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Who created this entity
        /// </summary>
        public string? CreatedBy { get; private set; }

        /// <summary>
        /// Who last updated this entity
        /// </summary>
        public string? UpdatedBy { get; private set; }

        /// <summary>
        /// Soft delete flag - when set, entity is considered deleted
        /// </summary>
        public DateTime? DeletedAt { get; private set; }

        /// <summary>
        /// Who deleted this entity
        /// </summary>
        public string? DeletedBy { get; private set; }

        /// <summary>
        /// Version for optimistic concurrency control
        /// </summary>
        [Timestamp]
        public byte[]? Version { get; private set; }

        /// <summary>
        /// Domain events raised by this entity
        /// </summary>
        [NotMapped]
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }

        protected BaseEntity(TId id) : this()
        {
            Id = id;
        }

        /// <summary>
        /// Add a domain event to be published
        /// </summary>
        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Remove a domain event
        /// </summary>
        public void RemoveDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        /// <summary>
        /// Clear all domain events
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// Mark entity as updated
        /// </summary>
        public void MarkAsUpdated(string? updatedBy = null)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }

        /// <summary>
        /// Mark entity as deleted (soft delete)
        /// </summary>
        public void MarkAsDeleted(string? deletedBy = null)
        {
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
        }

        /// <summary>
        /// Restore a soft-deleted entity
        /// </summary>
        public void Restore()
        {
            DeletedAt = null;
            DeletedBy = null;
            MarkAsUpdated();
        }

        /// <summary>
        /// Check if entity is soft deleted
        /// </summary>
        public bool IsDeleted => DeletedAt.HasValue;

        #region Equality

        public bool Equals(BaseEntity<TId>? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override bool Equals(object? obj)
        {
            return obj is BaseEntity<TId> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TId>.Default.GetHashCode(Id);
        }

        public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}
