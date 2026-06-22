using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace AngularNetBase.Practice.Infrastructure.Repositories
{
    public class AnswerConversationRepository : IAnswerConversationRepository
    {
        private readonly PracticeContext _context;

        public AnswerConversationRepository(PracticeContext context)
        {
            _context = context;
        }

        public async Task<AnswerConversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.AnswerConversations
                .Include(x => x.Turns)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<AnswerConversation>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.AnswerConversations
                .Include(x => x.Turns)
                .ToListAsync(cancellationToken);
        }

        public async Task<AnswerConversation?> GetByExerciseQuestionAsync(
            Guid userId,
            Guid exerciseId,
            Guid questionId,
            CancellationToken cancellationToken = default)
        {
            return await _context.AnswerConversations
                .Include(x => x.Turns)
                .FirstOrDefaultAsync(
                    x => x.UserId == userId
                        && x.ExerciseId == exerciseId
                        && x.QuestionId == questionId,
                    cancellationToken);
        }

        public async Task AddAsync(AnswerConversation entity, CancellationToken cancellationToken = default)
        {
            await _context.AnswerConversations.AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(AnswerConversation entity, CancellationToken cancellationToken = default)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _context.AnswerConversations.Attach(entity);
            }

            await Task.CompletedTask;
        }

        public async Task DeleteAsync(AnswerConversation entity, CancellationToken cancellationToken = default)
        {
            _context.AnswerConversations.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await NormalizeConversationStatesAsync(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task NormalizeConversationStatesAsync(CancellationToken cancellationToken)
        {
            foreach (var entry in _context.ChangeTracker.Entries<AnswerConversation>())
            {
                if (entry.State != EntityState.Modified)
                    continue;

                var exists = await AnswerConversationExistsAsync(entry.Entity.Id, cancellationToken);
                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }

            foreach (var entry in _context.ChangeTracker.Entries<ConversationTurn>())
            {
                if (entry.State != EntityState.Modified)
                    continue;

                var exists = await ConversationTurnExistsAsync(entry.Entity.Id, cancellationToken);
                entry.State = exists ? EntityState.Unchanged : EntityState.Added;
                SetOwnedEvaluationState(entry, entry.State);
            }
        }

        private async Task<bool> AnswerConversationExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.AnswerConversations
                .AsNoTracking()
                .AnyAsync(x => x.Id == id, cancellationToken);
        }

        private async Task<bool> ConversationTurnExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State == System.Data.ConnectionState.Closed;

            if (shouldClose)
                await connection.OpenAsync(cancellationToken);

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT EXISTS (
                        SELECT 1
                        FROM practice."AnswerConversationTurns"
                        WHERE "Id" = @id
                    )
                    """;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "id";
                parameter.Value = id;
                command.Parameters.Add(parameter);

                var result = await command.ExecuteScalarAsync(cancellationToken);
                return result is bool exists && exists;
            }
            finally
            {
                if (shouldClose)
                    await connection.CloseAsync();
            }
        }

        private static void SetOwnedEvaluationState(EntityEntry<ConversationTurn> turnEntry, EntityState state)
        {
            if (turnEntry.Entity.EvaluationSummary is null)
                return;

            var evaluationEntry = turnEntry.References
                .Select(x => x.TargetEntry)
                .FirstOrDefault(x => x?.Entity is EvaluationSummary);

            if (evaluationEntry is not null)
                evaluationEntry.State = state;
        }
    }
}
