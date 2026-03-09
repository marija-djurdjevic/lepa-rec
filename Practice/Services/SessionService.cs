using AngularNetBase.Practice.Dtos.Sessions;
using AngularNetBase.Practice.Entities.Sessions;
using AngularNetBase.Practice.Services;

namespace Modules.Practice.Services;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _sessionRepository;

    public SessionService(ISessionRepository sessionRepo)
    {
        _sessionRepository = sessionRepo;
    }

    public async Task<DailySessionStateDto> GetOrCreateTodaySessionAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        var session = await _sessionRepository.GetTodaySessionAsync(userId, today, cancellationToken);

        if (session == null)
        {
            session = new DailySession(Guid.NewGuid(), userId);
            await _sessionRepository.AddAsync(session, cancellationToken);
            await _sessionRepository.SaveChangesAsync(cancellationToken);
        }

        bool requiresPrimer = session.PrimerResult == null;

        return new DailySessionStateDto(session.Id, requiresPrimer, session.Status.ToString());
    }

    public async Task CompletePrimerAsync(Guid userId, CompletePrimerDto dto, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        var session = await _sessionRepository.GetTodaySessionAsync(userId, today, cancellationToken);

        if (session == null)
            throw new InvalidOperationException("Dnevna sesija nije pronađena.");

        session.RecordPrimerCompleted(dto.IsSkipped, dto.AffirmationValueId, dto.GrowthMessageId);

        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);
    }
}