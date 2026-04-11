using AngularNetBase.Practice.Dtos.GrowthMessages;
using AngularNetBase.Practice.Entities.GrowthMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public class GrowthMessageService : IGrowthMessageService
    {
        private readonly IGrowthMessageRepository _repository;

        public GrowthMessageService(IGrowthMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> CreateMessageAsync(CreateGrowthMessageDto dto, CancellationToken cancellationToken = default)
        {
            var message = new GrowthMessage(Guid.NewGuid(), dto.Text, dto.Type, true);

            await _repository.AddAsync(message, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return message.Id;
        }

        public async Task ToggleMessageStatusAsync(Guid id, bool activate, CancellationToken cancellationToken = default)
        {
            var message = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new InvalidOperationException($"Growth message sa ID-jem {id} nije pronađen.");

            if (activate)
                message.Activate();
            else
                message.Deactivate();

            await _repository.UpdateAsync(message, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }

        public async Task<GrowthMessageDto> GetRandomMessageAsync(
            GrowthMessageType type,
            CancellationToken cancellationToken = default)
        {
            var message = await _repository.GetRandomActiveMessageAsync(type, cancellationToken)
                ?? throw new InvalidOperationException("Nema aktivnih poruka ohrabrenja u bazi.");

            return new GrowthMessageDto(message.Id, message.Text);
        }
    }
}
