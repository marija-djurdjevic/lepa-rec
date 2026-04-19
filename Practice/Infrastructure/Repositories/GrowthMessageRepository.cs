using AngularNetBase.Practice.Entities.GrowthMessages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Infrastructure.Repositories
{
    public class GrowthMessageRepository : IGrowthMessageRepository
    {
        private readonly PracticeContext _context;

        public GrowthMessageRepository(PracticeContext context)
        {
            _context = context;
        }

        public async Task<GrowthMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.GrowthMessages
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<GrowthMessage?> GetRandomActiveMessageAsync(
            GrowthMessageType type,
            CancellationToken cancellationToken = default)
        {
            return await _context.GrowthMessages
                .Where(m => m.IsActive && m.Type == type)
                .OrderBy(r => Guid.NewGuid())
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<GrowthMessage?> GetRandomActiveMessageByAffirmationValueAsync(
            GrowthMessageType type,
            Guid affirmationValueId,
            CancellationToken cancellationToken = default)
        {
            return await _context.GrowthMessages
                .Where(m => m.IsActive && m.Type == type && m.AffirmationValueId == affirmationValueId)
                .OrderBy(r => Guid.NewGuid())
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<GrowthMessage?> GetRandomActiveMessageWithoutAffirmationValueAsync(
            GrowthMessageType type,
            CancellationToken cancellationToken = default)
        {
            return await _context.GrowthMessages
                .Where(m => m.IsActive && m.Type == type && m.AffirmationValueId == null)
                .OrderBy(r => Guid.NewGuid())
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(GrowthMessage growthMessage, CancellationToken cancellationToken = default)
        {
            await _context.GrowthMessages.AddAsync(growthMessage, cancellationToken);
        }

        public Task UpdateAsync(GrowthMessage growthMessage, CancellationToken cancellationToken = default)
        {
            _context.GrowthMessages.Update(growthMessage);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
