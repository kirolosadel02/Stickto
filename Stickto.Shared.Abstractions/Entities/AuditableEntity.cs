namespace Stickto.Shared.Abstractions.Entities
{
    public abstract class AuditableEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditableEntity"/> class.
        /// </summary>
        protected AuditableEntity()
        {
        }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last updated.
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
    }
}
