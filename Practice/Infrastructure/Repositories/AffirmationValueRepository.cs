using AngularNetBase.Practice.Entities.AffirmationValues;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Infrastructure.Repositories
{
    public class AffirmationValueRepository : IAffirmationValueRepository
    {
        private readonly PracticeContext _context;

        public AffirmationValueRepository(PracticeContext context)
        {
            _context = context;
        }

        public async Task<AffirmationValue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.AffirmationValues
                .Include(a => a.Statements)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<AffirmationValue>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.AffirmationValues
                .Include(a => a.Statements)
                .ToListAsync(cancellationToken);
        }

        public async Task<ValueStatement?> GetStatementByIdAsync(Guid statementId, CancellationToken cancellationToken = default)
        {
            return await _context.AffirmationValues
                .SelectMany(a => a.Statements)
                .FirstOrDefaultAsync(s => s.Id == statementId, cancellationToken);
        }

        public async Task AddAsync(AffirmationValue affirmationValue, CancellationToken cancellationToken = default)
        {
            await _context.AffirmationValues.AddAsync(affirmationValue, cancellationToken);
        }

        public Task UpdateAsync(AffirmationValue affirmationValue, CancellationToken cancellationToken = default)
        {
            _context.AffirmationValues.Update(affirmationValue);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ValueStatement>> GetRandomActiveStatementsAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _context.AffirmationValues
                .SelectMany(a => a.Statements)
                .Where(s => s.IsActive)
                .OrderBy(r => Guid.NewGuid())
                .Take(count)
                .ToListAsync(cancellationToken);
        }
    }
}
