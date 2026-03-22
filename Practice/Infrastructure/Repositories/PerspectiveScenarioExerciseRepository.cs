using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Infrastructure.Repositories
{
    public class PerspectiveScenarioExerciseRepository : IPerspectiveScenarioExerciseRepository
    {
        private readonly PracticeContext _context;

        public PerspectiveScenarioExerciseRepository(PracticeContext context)
        {
            _context = context;
        }

        public async Task<PerspectiveScenarioExercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.PerspectiveScenarioExercises
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<PerspectiveScenarioExercise>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.PerspectiveScenarioExercises
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<PerspectiveScenarioExercise>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.PerspectiveScenarioExercises
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<PerspectiveScenarioExercise?> GetByUserIdAndChallengeIdAsync(Guid userId, Guid challengeId, CancellationToken cancellationToken = default)
        {
            return await _context.PerspectiveScenarioExercises
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ChallengeId == challengeId, cancellationToken);
        }

        public async Task AddAsync(PerspectiveScenarioExercise entity, CancellationToken cancellationToken = default)
        {
            await _context.PerspectiveScenarioExercises.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(PerspectiveScenarioExercise entity, CancellationToken cancellationToken = default)
        {
            _context.PerspectiveScenarioExercises.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(PerspectiveScenarioExercise entity, CancellationToken cancellationToken = default)
        {
            _context.PerspectiveScenarioExercises.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
