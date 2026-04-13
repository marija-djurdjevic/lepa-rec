using System;

namespace AngularNetBase.Practice.Entities.DistancedJournals
{
    public class DistancedJournalPhoto
    {
        public Guid Id { get; private set; }
        public string ObjectKey { get; private set; } = string.Empty;
        public string FileName { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;
        public long SizeBytes { get; private set; }
        public DateTime UploadedAt { get; private set; }

        private DistancedJournalPhoto() { }

        public DistancedJournalPhoto(
            Guid id,
            string objectKey,
            string fileName,
            string contentType,
            long sizeBytes,
            DateTime uploadedAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Photo id must be provided.", nameof(id));

            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key must be provided.", nameof(objectKey));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name must be provided.", nameof(fileName));

            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content type must be provided.", nameof(contentType));

            if (sizeBytes <= 0)
                throw new ArgumentException("Photo size must be positive.", nameof(sizeBytes));

            Id = id;
            ObjectKey = objectKey.Trim();
            FileName = fileName.Trim();
            ContentType = contentType.Trim();
            SizeBytes = sizeBytes;
            UploadedAt = uploadedAt;
        }
    }
}
