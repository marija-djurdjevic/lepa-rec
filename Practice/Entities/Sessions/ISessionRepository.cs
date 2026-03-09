using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.Sessions
{
    public interface ISessionRepository
    {
        Task<DailySession?> GetTodaySessionAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task AddAsync(DailySession session, CancellationToken cancellationToken = default);
        Task UpdateAsync(DailySession session, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
