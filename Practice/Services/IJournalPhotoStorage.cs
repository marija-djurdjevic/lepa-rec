namespace AngularNetBase.Practice.Services
{
    public interface IJournalPhotoStorage
    {
        Task SaveAsync(
            string objectKey,
            Stream content,
            long sizeBytes,
            string contentType,
            CancellationToken cancellationToken = default);

        Task<Stream> OpenReadAsync(
            string objectKey,
            CancellationToken cancellationToken = default);
    }
}
