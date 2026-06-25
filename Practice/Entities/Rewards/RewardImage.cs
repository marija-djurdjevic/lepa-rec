using System;

namespace AngularNetBase.Practice.Entities.Rewards
{
    public class RewardImage
    {
        public Guid Id { get; private set; }
        public string AssetKey { get; private set; } = string.Empty;
        public string AssetPath { get; private set; } = string.Empty;
        public string? ImageUrl { get; private set; }
        public int SortOrder { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private RewardImage() { }

        public RewardImage(
            Guid id,
            string assetKey,
            string assetPath,
            int sortOrder,
            bool isActive,
            DateTime createdAt,
            string? imageUrl = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must be provided.", nameof(id));

            if (string.IsNullOrWhiteSpace(assetKey))
                throw new ArgumentException("Asset key must be provided.", nameof(assetKey));

            if (string.IsNullOrWhiteSpace(assetPath) && string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Either asset path or image URL must be provided.", nameof(assetPath));

            Id = id;
            AssetKey = assetKey.Trim();
            AssetPath = assetPath.Trim();
            ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
            SortOrder = sortOrder;
            IsActive = isActive;
            CreatedAt = createdAt;
        }
    }
}
