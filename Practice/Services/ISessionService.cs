using AngularNetBase.Practice.Dtos.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public interface ISessionService
    {
        Task<DailySessionStateDto> GetOrCreateTodaySessionAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<DailySessionStateDto> CompletePrimerAsync(Guid userId, CompletePrimerDto dto, CancellationToken cancellationToken = default);

        Task<DailySessionStateDto> RecordExerciseAsync(Guid userId, RecordExerciseDto dto, CancellationToken cancellationToken = default);

        Task<DailySessionStateDto> CompleteTodaySessionAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
