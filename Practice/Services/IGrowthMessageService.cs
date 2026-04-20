using AngularNetBase.Practice.Dtos.GrowthMessages;
using AngularNetBase.Practice.Entities.GrowthMessages;

namespace AngularNetBase.Practice.Services
{
    public interface IGrowthMessageService
    {
        Task<Guid> CreateMessageAsync(CreateGrowthMessageDto dto, CancellationToken cancellationToken = default);
        Task ToggleMessageStatusAsync(Guid id, bool activate, CancellationToken cancellationToken = default);
        Task<GrowthMessageDto> GetRandomMessageAsync(
            GrowthMessageType type,
            Guid? selectedStatementId = null,
            string? language = null,
            CancellationToken cancellationToken = default);
    }
}
