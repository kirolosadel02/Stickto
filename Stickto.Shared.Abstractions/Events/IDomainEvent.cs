using MediatR;

namespace Stickto.Shared.Abstractions.Events
{
    /// <summary>
    /// Represents a domain event. This is a marker interface that extends
    /// the <see cref="INotification"/> interface from MediatR.
    /// </summary>
    public interface IDomainEvent : INotification
    {
        /// <summary>
        /// Gets the date and time when the event occurred.
        /// </summary>
        DateTime OccurredOn { get; }
    }
}
