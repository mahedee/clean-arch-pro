namespace EduTrack.Domain.Common
{
    /// <summary>
    /// Base class for aggregate root entities
    /// Aggregate roots are the only entities that can be retrieved directly from repositories
    /// </summary>
    /// <typeparam name="TId">The type of the entity identifier</typeparam>
    public abstract class AggregateRoot<TId> : BaseEntity<TId>
        where TId : notnull
    {
        protected AggregateRoot() : base()
        {
        }

        protected AggregateRoot(TId id) : base(id)
        {
        }
    }
}
