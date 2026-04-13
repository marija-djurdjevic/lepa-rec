using AngularNetBase.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.Sessions
{
    public interface ISessionRepository 
    {
        Task<DailySession?> GetByUserAndDateAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task AddAsync(DailySession session, CancellationToken cancellationToken = default);
        Task UpdateAsync(DailySession session, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<DailySession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<DailySession?> ReloadAsync(Guid id, CancellationToken cancellationToken = default);

    }
}
