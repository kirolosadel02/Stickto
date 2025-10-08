using Stickto.Shared.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stickto.Shared.Infrastructure.Repositories
{
    public class GenericViewRepository<TEntity> : AbstractionRepository<TEntity>
    where TEntity : Entity
    {
        public GenericViewRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
