using AngularNetBase.Practice.Dtos.GrowthMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public interface IGrowthMessageService
    {
        Task<Guid> CreateMessageAsync(CreateGrowthMessageDto dto, CancellationToken cancellationToken = default);
        Task ToggleMessageStatusAsync(Guid id, bool activate, CancellationToken cancellationToken = default);
        Task<GrowthMessageDto> GetRandomMessageAsync(CancellationToken cancellationToken = default);
    }
}
