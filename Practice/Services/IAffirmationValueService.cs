using AngularNetBase.Practice.Dtos.AffirmationValues;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public interface IAffirmationValueService
    {
        Task<Guid> CreateAffirmationValueAsync(CreateAffirmationValueDto dto, CancellationToken cancellationToken = default);
        Task<Guid> AddStatementAsync(Guid affirmationValueId, AddStatementDto dto, CancellationToken cancellationToken = default);
        Task ToggleStatementStatusAsync(Guid affirmationValueId, Guid statementId, bool activate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PrimerStatementDto>> GetPrimerStatementsAsync(CancellationToken cancellationToken = default);
    }
}
