using Stickto.Shared.Abstractions.Events;

namespace Stickto.Shared.Abstractions.Entities
{
    /// <summary>
    /// Represents the base class for all entities in the domain.
    /// </summary>
    public abstract class Entity
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        protected Entity()
        {
        }

        /// <summary>
        /// Gets the domain events that have been raised within the entity.
        /// </summary>
        /// <returns>A read-only list of domain events.</returns>
        public IReadOnlyList<IDomainEvent> GetDomainEvents()
        {
            return _domainEvents.ToList();
        }

        /// <summary>
        /// Clears the domain events that have been raised within the entity.
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// Raises a domain event within the entity.
        /// </summary>
        /// <param name="domainEvent">The domain event to raise.</param>
        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}
