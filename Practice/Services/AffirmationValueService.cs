using AngularNetBase.Practice.Dtos.AffirmationValues;
using AngularNetBase.Practice.Entities.AffirmationValues;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public class AffirmationValueService : IAffirmationValueService
    {
        private readonly IAffirmationValueRepository _repository;

        public AffirmationValueService(IAffirmationValueRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> CreateAffirmationValueAsync(CreateAffirmationValueDto dto, CancellationToken cancellationToken = default)
        {
            var affirmationValue = new AffirmationValue(Guid.NewGuid(), dto.Name);

            await _repository.AddAsync(affirmationValue, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return affirmationValue.Id;
        }

        public async Task<Guid> AddStatementAsync(Guid affirmationValueId, AddStatementDto dto, CancellationToken cancellationToken = default)
        {
            var affirmationValue = await _repository.GetByIdAsync(affirmationValueId, cancellationToken)
                ?? throw new InvalidOperationException($"Affirmation value sa ID-jem {affirmationValueId} nije pronađen.");

            var statement = affirmationValue.AddStatement(Guid.NewGuid(), dto.Text, dto.IsActive, dto.TextEn);

            await _repository.UpdateAsync(affirmationValue, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return statement.Id;
        }

        public async Task ToggleStatementStatusAsync(Guid affirmationValueId, Guid statementId, bool activate, CancellationToken cancellationToken = default)
        {
            var affirmationValue = await _repository.GetByIdAsync(affirmationValueId, cancellationToken)
                ?? throw new InvalidOperationException($"Affirmation value sa ID-jem {affirmationValueId} nije pronađen.");

            if (activate)
                affirmationValue.ActivateStatement(statementId);
            else
                affirmationValue.DeactivateStatement(statementId);

            await _repository.UpdateAsync(affirmationValue, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<PrimerStatementDto>> GetPrimerStatementsAsync(string? language = null, CancellationToken cancellationToken = default)
        {
            var isEnglish = IsEnglish(language);
            var statements = await _repository.GetRandomActiveStatementsAsync(4, cancellationToken);

            if (statements.Count < 4) throw new InvalidOperationException("Nema dovoljno aktivnih izjava.");

            return statements
                .Select(s => new PrimerStatementDto(s.Id, SelectLocalized(s.Text, s.TextEn, isEnglish)))
                .ToList();
        }

        private static bool IsEnglish(string? language)
            => !string.IsNullOrWhiteSpace(language)
                && language.StartsWith("en", StringComparison.OrdinalIgnoreCase);

        private static string SelectLocalized(string sr, string? en, bool isEnglish)
            => isEnglish && !string.IsNullOrWhiteSpace(en) ? en : sr;
    }
}
