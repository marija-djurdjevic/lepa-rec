using AngularNetBase.Shared.Core.Interfaces;

namespace AngularNetBase.Practice.Entities.PerspectiveScenarios
{
    public interface IAnswerConversationRepository : IRepository<AnswerConversation, Guid>
    {
        Task<AnswerConversation?> GetByExerciseQuestionAsync(
            Guid userId,
            Guid exerciseId,
            Guid questionId,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
