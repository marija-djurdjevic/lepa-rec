using AngularNetBase.Practice.Dtos.Rewards;
using AngularNetBase.Practice.Entities.Rewards;
using AngularNetBase.Practice.Entities.Sessions;
using AngularNetBase.Practice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AngularNetBase.Practice.Services
{
    public class RewardService : IRewardService
    {
        private readonly PracticeContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RewardService(PracticeContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<RewardProgressDto?> GetCurrentRewardAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var current = await GetCurrentProgressForDisplayAsync(userId, cancellationToken);
            return current is null
                ? null
                : Map(current, false, null, null);
        }

        public async Task<RewardProgressDto?> GrantDailyPieceAsync(
            Guid userId,
            DailySession session,
            CancellationToken cancellationToken = default)
        {
            if (session.UserId != userId)
                throw new InvalidOperationException("Session does not belong to the current user.");

            var existingGrant = await _context.RewardPieceGrants
                .Include(x => x.RewardProgress)
                .ThenInclude(x => x!.RewardImage)
                .FirstOrDefaultAsync(
                    x => x.UserId == userId && x.DailySessionId == session.Id,
                    cancellationToken);

            if (existingGrant?.RewardProgress is not null)
                return Map(existingGrant.RewardProgress, false, null, null);

            var progress = await GetIncompleteProgressAsync(userId, cancellationToken)
                ?? await CreateProgressAsync(userId, cancellationToken);

            if (progress is null)
                return null;

            var previousCount = progress.UnlockedPiecesCount;
            var now = _dateTimeProvider.UtcNow;
            var pieceIndex = progress.UnlockNextPiece(now);

            _context.RewardPieceGrants.Add(new RewardPieceGrant(
                Guid.NewGuid(),
                userId,
                session.Id,
                session.Date,
                progress.Id,
                pieceIndex,
                now));

            await _context.SaveChangesAsync(cancellationToken);

            return Map(progress, true, previousCount, pieceIndex);
        }

        public async Task<RewardProgressDto> SaveRewardAsync(
            Guid userId,
            Guid rewardProgressId,
            CancellationToken cancellationToken = default)
        {
            var progress = await _context.UserRewardProgresses
                .Include(x => x.RewardImage)
                .FirstOrDefaultAsync(
                    x => x.Id == rewardProgressId && x.UserId == userId,
                    cancellationToken);

            if (progress is null)
                throw new InvalidOperationException("Reward progress was not found.");

            progress.Save(_dateTimeProvider.UtcNow);
            await _context.SaveChangesAsync(cancellationToken);

            return Map(progress, false, null, null);
        }

        public async Task<RewardGalleryDto> GetGalleryAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var rewards = await _context.UserRewardProgresses
                .Include(x => x.RewardImage)
                .Where(x => x.UserId == userId && x.CompletedAt != null && x.SavedAt != null)
                .OrderByDescending(x => x.SavedAt)
                .ToListAsync(cancellationToken);

            return new RewardGalleryDto(rewards
                .Where(x => x.RewardImage is not null)
                .Select(x => new RewardGalleryItemDto(
                    x.Id,
                    x.RewardImage!.AssetKey,
                    x.RewardImage.AssetPath,
                    x.RewardImage.ImageUrl,
                    x.CompletedAt!.Value,
                    x.SavedAt!.Value))
                .ToList());
        }

        private async Task<UserRewardProgress?> GetCurrentProgressForDisplayAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var incomplete = await GetIncompleteProgressAsync(userId, cancellationToken);
            if (incomplete is not null)
                return incomplete;

            var today = _dateTimeProvider.BusinessDate;
            var completedToday = await _context.RewardPieceGrants
                .Include(x => x.RewardProgress)
                .ThenInclude(x => x!.RewardImage)
                .Where(x => x.UserId == userId && x.SessionDate == today)
                .OrderByDescending(x => x.GrantedAt)
                .Select(x => x.RewardProgress)
                .FirstOrDefaultAsync(cancellationToken);

            if (completedToday is not null)
                return completedToday;

            return null;
        }

        private async Task<UserRewardProgress?> GetIncompleteProgressAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _context.UserRewardProgresses
                .Include(x => x.RewardImage)
                .Where(x => x.UserId == userId && x.CompletedAt == null)
                .OrderBy(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private async Task<UserRewardProgress?> CreateProgressAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var image = await SelectNextImageAsync(userId, cancellationToken);
            if (image is null)
                return null;

            var now = _dateTimeProvider.UtcNow;
            var progress = new UserRewardProgress(Guid.NewGuid(), userId, image.Id, now);
            _context.UserRewardProgresses.Add(progress);
            await _context.SaveChangesAsync(cancellationToken);

            progress = await _context.UserRewardProgresses
                .Include(x => x.RewardImage)
                .FirstAsync(x => x.Id == progress.Id, cancellationToken);

            return progress;
        }

        private async Task<RewardImage?> SelectNextImageAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var activeImages = await _context.RewardImages
                .Where(x => x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(cancellationToken);

            if (activeImages.Count == 0)
                return null;

            var completedImageIds = await _context.UserRewardProgresses
                .Where(x => x.UserId == userId && x.CompletedAt != null)
                .Select(x => x.RewardImageId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var unseen = activeImages
                .Where(x => !completedImageIds.Contains(x.Id))
                .ToList();

            if (unseen.Count > 0)
                return unseen[Random.Shared.Next(unseen.Count)];

            var mostRecentImageId = await _context.UserRewardProgresses
                .Where(x => x.UserId == userId && x.CompletedAt != null)
                .OrderByDescending(x => x.CompletedAt)
                .Select(x => (Guid?)x.RewardImageId)
                .FirstOrDefaultAsync(cancellationToken);

            var repeatCandidates = activeImages
                .Where(x => x.Id != mostRecentImageId)
                .ToList();

            if (repeatCandidates.Count == 0)
                repeatCandidates = activeImages;

            return repeatCandidates[Random.Shared.Next(repeatCandidates.Count)];
        }

        private static RewardProgressDto Map(
            UserRewardProgress progress,
            bool shouldPlayUnlockAnimation,
            int? previousUnlockedPiecesCount,
            int? newlyUnlockedPieceIndex)
        {
            if (progress.RewardImage is null)
                throw new InvalidOperationException("Reward image must be loaded.");

            return new RewardProgressDto(
                progress.Id,
                progress.RewardImage.AssetKey,
                progress.RewardImage.AssetPath,
                progress.RewardImage.ImageUrl,
                progress.UnlockedPiecesCount,
                previousUnlockedPiecesCount,
                newlyUnlockedPieceIndex,
                progress.CompletedAt.HasValue,
                progress.CompletedAt,
                progress.SavedAt,
                shouldPlayUnlockAnimation);
        }
    }
}
