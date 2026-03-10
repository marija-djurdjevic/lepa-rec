using AngularNetBase.Practice.Entities.Sessions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Infrastructure.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly PracticeContext _context;

        public SessionRepository(PracticeContext context)
        {
            _context = context;
        }

        public async Task<DailySession?> GetByUserAndDateAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await _context.DailySessions
                .Include("_events")
                .FirstOrDefaultAsync(
                    s => s.UserId == userId && s.Date == date.Date,
                    cancellationToken);
        }

        public async Task AddAsync(DailySession session, CancellationToken cancellationToken = default)
        {
            await _context.DailySessions.AddAsync(session, cancellationToken);
        }

        public Task UpdateAsync(DailySession session, CancellationToken cancellationToken = default)
        {
            _context.DailySessions.Update(session);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
