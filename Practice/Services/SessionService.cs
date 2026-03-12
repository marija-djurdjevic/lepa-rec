using AngularNetBase.Practice.Dtos.Sessions;
using AngularNetBase.Practice.Entities.Sessions;
using AngularNetBase.Practice.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Modules.Practice.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SessionService(
            ISessionRepository sessionRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _sessionRepository = sessionRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<DailySessionStateDto> GetOrCreateTodaySessionAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var session = await GetOrCreateTodaySessionEntityAsync(userId, cancellationToken);
            return MapToStateDto(session);
        }

        public async Task<DailySessionStateDto> CompletePrimerAsync(
     Guid userId,
     CompletePrimerDto dto,
     CancellationToken cancellationToken = default)
        {
            var session = await GetOrCreateTodaySessionEntityAsync(userId, cancellationToken);
            var now = _dateTimeProvider.UtcNow;

            if (dto.IsSkipped)
            {
                if (dto.SelectedStatementId.HasValue || dto.GrowthMessageId.HasValue)
                    throw new InvalidOperationException("Ako je primer preskočen, SelectedStatementId i GrowthMessageId moraju biti null.");

                session.SkipPrimer(now);
            }
            else
            {
                if (dto.PresentedStatementIds == null || !dto.PresentedStatementIds.Any())
                    throw new InvalidOperationException("Mora postojati barem jedna ponuđena izjava.");

                if (!dto.SelectedStatementId.HasValue)
                    throw new InvalidOperationException("SelectedStatementId je obavezan kada primer nije preskočen.");

                if (!dto.GrowthMessageId.HasValue)
                    throw new InvalidOperationException("GrowthMessageId je obavezan kada primer nije preskočen.");

                session.CompletePrimer(
                    dto.PresentedStatementIds,
                    dto.SelectedStatementId.Value,
                    dto.GrowthMessageId.Value,
                    now);
            }

            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return MapToStateDto(session);
        }
        public async Task<DailySessionStateDto> RecordExerciseAsync(
            Guid userId,
            RecordExerciseDto dto,
            CancellationToken cancellationToken = default)
        {
            var session = await GetOrCreateTodaySessionEntityAsync(userId, cancellationToken);
            var now = _dateTimeProvider.UtcNow;

            session.RecordExercise(dto.ExerciseId, dto.Type, now);

            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return MapToStateDto(session);
        }

        public async Task<DailySessionStateDto> CompleteTodaySessionAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var session = await GetOrCreateTodaySessionEntityAsync(userId, cancellationToken);
            var now = _dateTimeProvider.UtcNow;

            session.Complete(now);

            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return MapToStateDto(session);
        }

        private async Task<DailySession> GetOrCreateTodaySessionEntityAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var today = _dateTimeProvider.UtcNow.Date;

            var session = await _sessionRepository.GetByUserAndDateAsync(
                userId,
                today,
                cancellationToken);

            if (session != null)
                return session;

            session = new DailySession(Guid.NewGuid(), userId, today);

            await _sessionRepository.AddAsync(session, cancellationToken);
            await _sessionRepository.SaveChangesAsync(cancellationToken);

            return session;
        }

        private static DailySessionStateDto MapToStateDto(DailySession session)
        {
            return new DailySessionStateDto(
                session.Id,
                session.Status.ToString(),
                session.RequiresPrimer,
                session.PrimerCompleted,
                session.PrimerSkipped,
                session.CompletedExercisesCount);
        }
    }
}