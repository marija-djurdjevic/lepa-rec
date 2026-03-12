using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.AffirmationValues
{
    public interface IAffirmationValueRepository
    {
        Task<AffirmationValue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AffirmationValue>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(AffirmationValue affirmationValue, CancellationToken cancellationToken = default);
        Task UpdateAsync(AffirmationValue affirmationValue, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ValueStatement>> GetRandomActiveStatementsAsync(int count, CancellationToken cancellationToken = default);
    }
}
