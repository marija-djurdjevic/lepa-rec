using AngularNetBase.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.Skills
{
    public interface ISkillRepository : IRepository<Skill, Guid>
    {
        Task<IReadOnlyCollection<Skill>> GetByIdsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
