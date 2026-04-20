using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AngularNetBase.Practice.Services
{
    public class DailyChallengeAssignmentScheduler : BackgroundService
    {
        private readonly ILogger<DailyChallengeAssignmentScheduler> _logger;
        private readonly TimeZoneInfo _timeZone;
        private readonly IServiceScopeFactory _scopeFactory;

        public DailyChallengeAssignmentScheduler(
            ILogger<DailyChallengeAssignmentScheduler> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _timeZone = ResolveTimeZone();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await EnsureAssignmentAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ensure daily challenge assignment on startup.");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var localNow = TimeZoneInfo.ConvertTimeFromUtc(now, _timeZone);
                var nextLocalRun = new DateTime(localNow.Year, localNow.Month, localNow.Day, 3, 0, 0, DateTimeKind.Unspecified);
                if (localNow >= nextLocalRun)
                {
                    nextLocalRun = nextLocalRun.AddDays(1);
                }

                var nextRunUtc = TimeZoneInfo.ConvertTimeToUtc(nextLocalRun, _timeZone);
                var delay = nextRunUtc - now;
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, stoppingToken);
                }

                if (stoppingToken.IsCancellationRequested)
                    break;

                try
                {
                    await EnsureAssignmentAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to ensure daily challenge assignment.");
                }
            }
        }

        private static TimeZoneInfo ResolveTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Europe/Sarajevo");
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                }
                catch (TimeZoneNotFoundException)
                {
                    return TimeZoneInfo.Local;
                }
            }
        }

        private async Task EnsureAssignmentAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
            var assignmentService = scope.ServiceProvider.GetRequiredService<IDailyChallengeAssignmentService>();
            await assignmentService.EnsureAssignmentForDateAsync(dateTimeProvider.BusinessDate, stoppingToken);
        }
    }
}
