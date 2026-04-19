using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.GrowthMessages
{
    public interface IGrowthMessageRepository
    {
        Task<GrowthMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<GrowthMessage?> GetRandomActiveMessageAsync(
            GrowthMessageType type,
            CancellationToken cancellationToken = default);
        Task<GrowthMessage?> GetRandomActiveMessageByAffirmationValueAsync(
            GrowthMessageType type,
            Guid affirmationValueId,
            CancellationToken cancellationToken = default);
        Task<GrowthMessage?> GetRandomActiveMessageWithoutAffirmationValueAsync(
            GrowthMessageType type,
            CancellationToken cancellationToken = default);
        Task AddAsync(GrowthMessage growthMessage, CancellationToken cancellationToken = default);
        Task UpdateAsync(GrowthMessage growthMessage, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
