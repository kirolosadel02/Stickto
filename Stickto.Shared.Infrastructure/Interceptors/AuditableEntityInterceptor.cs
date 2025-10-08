using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Stickto.Shared.Abstractions.Entities;
using System.Security.Claims;

namespace Stickto.Shared.Infrastructure.Interceptors
{
    /// <summary>
    /// Represents an interceptor that sets the audit properties of an entity before it is saved to the database.
    /// This class is responsible for setting the CreatedOn, CreatedBy, UpdatedOn, and UpdatedBy properties of an entity that implements the <see cref="AuditableEntity"/> interface.
    /// </summary>
    public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditableEntityInterceptor"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public AuditableEntityInterceptor(IHttpContextAccessor httpContextAccessor)
            => _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// Intercepts the SavingChanges event of the DbContext and sets the audit properties of the entity.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="result">The result.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The interception result.</returns>
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
                    DbContextEventData eventData,
                    InterceptionResult<int> result,
                    CancellationToken cancellationToken = default)
        {
            DbContext dbContext = eventData.Context;
            if (dbContext is null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            Claim authenticatedUser = _httpContextAccessor
                .HttpContext?
                .User?
                .Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name);
            IEnumerable<EntityEntry<AuditableEntity>> entries = eventData.Context.ChangeTracker.Entries<AuditableEntity>()
                .Where(e => e.State is EntityState.Added or EntityState.Modified);

            foreach (EntityEntry<AuditableEntity> entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property(x => x.CreatedOn).CurrentValue =
                        entityEntry.Property(x => x.CreatedOn).CurrentValue == DateTime.MinValue ?
                        DateTime.UtcNow :
                        entityEntry.Property(x => x.CreatedOn).CurrentValue;
                }
                else
                {
                    entityEntry.Property(x => x.UpdatedOn).CurrentValue =
                        entityEntry.Property(x => x.UpdatedOn).CurrentValue == null ?
                        DateTime.UtcNow
                        : entityEntry.Property(x => x.UpdatedOn).CurrentValue;
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }

}
