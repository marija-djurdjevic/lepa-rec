using AngularNetBase.Practice.Dtos.GrowthMessages;
using AngularNetBase.Practice.Entities.AffirmationValues;
using AngularNetBase.Practice.Entities.GrowthMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public class GrowthMessageService : IGrowthMessageService
    {
        private readonly IGrowthMessageRepository _repository;
        private readonly IAffirmationValueRepository _affirmationValueRepository;

        public GrowthMessageService(
            IGrowthMessageRepository repository,
            IAffirmationValueRepository affirmationValueRepository)
        {
            _repository = repository;
            _affirmationValueRepository = affirmationValueRepository;
        }

        public async Task<Guid> CreateMessageAsync(CreateGrowthMessageDto dto, CancellationToken cancellationToken = default)
        {
            var message = new GrowthMessage(Guid.NewGuid(), dto.Text, dto.Type, true, null, dto.TextEn, dto.SkillId);

            await _repository.AddAsync(message, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return message.Id;
        }

        public async Task ToggleMessageStatusAsync(Guid id, bool activate, CancellationToken cancellationToken = default)
        {
            var message = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new InvalidOperationException($"Growth message sa ID-jem {id} nije pronaden.");

            if (activate)
                message.Activate();
            else
                message.Deactivate();

            await _repository.UpdateAsync(message, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }

        public async Task<GrowthMessageDto> GetRandomMessageAsync(
            GrowthMessageType type,
            Guid? selectedStatementId = null,
            IReadOnlyCollection<Guid>? developedSkillIds = null,
            string? language = null,
            CancellationToken cancellationToken = default)
        {
            var message = await SelectMessageAsync(type, selectedStatementId, developedSkillIds, cancellationToken)
                ?? throw new InvalidOperationException("Nema aktivnih poruka ohrabrenja u bazi.");

            return new GrowthMessageDto(message.Id, SelectLocalized(message.Text, message.TextEn, language));
        }

        private async Task<GrowthMessage?> SelectMessageAsync(
            GrowthMessageType type,
            Guid? selectedStatementId,
            IReadOnlyCollection<Guid>? developedSkillIds,
            CancellationToken cancellationToken)
        {
            if (type == GrowthMessageType.End)
            {
                var selectedSkillMessage = await SelectMessageForDevelopedSkillsAsync(
                    developedSkillIds,
                    cancellationToken);

                if (selectedSkillMessage is not null)
                    return selectedSkillMessage;

                return await _repository.GetRandomActiveMessageAsync(type, cancellationToken);
            }

            if (type != GrowthMessageType.Begin || !selectedStatementId.HasValue || selectedStatementId.Value == Guid.Empty)
            {
                return await _repository.GetRandomActiveMessageAsync(type, cancellationToken);
            }

            var statement = await _affirmationValueRepository.GetStatementByIdAsync(selectedStatementId.Value, cancellationToken);
            if (statement is null)
            {
                return await _repository.GetRandomActiveMessageAsync(type, cancellationToken);
            }

            var preferGeneric = Random.Shared.Next(4) == 0; // 25%

            if (preferGeneric)
            {
                var generic = await _repository.GetRandomActiveMessageWithoutAffirmationValueAsync(type, cancellationToken);
                if (generic is not null)
                    return generic;

                return await _repository.GetRandomActiveMessageByAffirmationValueAsync(type, statement.AffirmationValueId, cancellationToken);
            }

            var matched = await _repository.GetRandomActiveMessageByAffirmationValueAsync(type, statement.AffirmationValueId, cancellationToken);
            if (matched is not null)
                return matched;

            return await _repository.GetRandomActiveMessageWithoutAffirmationValueAsync(type, cancellationToken);
        }

        private async Task<GrowthMessage?> SelectMessageForDevelopedSkillsAsync(
            IReadOnlyCollection<Guid>? developedSkillIds,
            CancellationToken cancellationToken)
        {
            if (developedSkillIds is null || developedSkillIds.Count == 0)
                return null;

            var distinctSkillIds = developedSkillIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (distinctSkillIds.Count == 0)
                return null;

            var randomSkillId = distinctSkillIds[Random.Shared.Next(distinctSkillIds.Count)];

            return await _repository.GetRandomActiveMessageBySkillAsync(
                GrowthMessageType.End,
                randomSkillId,
                cancellationToken);
        }

        private static string SelectLocalized(string sr, string? en, string? language)
        {
            var isEnglish = !string.IsNullOrWhiteSpace(language)
                && language.StartsWith("en", StringComparison.OrdinalIgnoreCase);

            if (isEnglish && !string.IsNullOrWhiteSpace(en))
                return en;

            return sr;
        }
    }
}
