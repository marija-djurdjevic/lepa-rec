using AngularNetBase.Practice.Entities.Skills;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Infrastructure.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly PracticeContext _context;

        public SkillRepository(PracticeContext context)
        {
            _context = context;
        }

        public async Task<Skill?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Skills
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Skill>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Skills
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<Skill>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var distinctIds = ids.Distinct().ToList();

            return await _context.Skills
                .Where(x => distinctIds.Contains(x.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Skill entity, CancellationToken cancellationToken = default)
        {
            await _context.Skills.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Skill entity, CancellationToken cancellationToken = default)
        {
            _context.Skills.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Skill entity, CancellationToken cancellationToken = default)
        {
            _context.Skills.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
