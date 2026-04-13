using AngularNetBase.Practice.Services;
using Minio;
using Minio.DataModel.Args;

namespace AngularNetBase.Practice.Infrastructure.Storage
{
    public class MinioJournalPhotoStorage : IJournalPhotoStorage
    {
        private readonly MinioClient _client;
        private readonly MinioOptions _options;

        public MinioJournalPhotoStorage(MinioOptions options)
        {
            _options = options;

            _client = new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .WithSSL(options.UseSsl)
                .Build();
        }

        public async Task SaveAsync(
            string objectKey,
            Stream content,
            long sizeBytes,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key must be provided.", nameof(objectKey));

            await EnsureBucketAsync(cancellationToken);

            var putArgs = new PutObjectArgs()
                .WithBucket(_options.Bucket)
                .WithObject(objectKey)
                .WithStreamData(content)
                .WithObjectSize(sizeBytes)
                .WithContentType(contentType);

            await _client.PutObjectAsync(putArgs, cancellationToken);
        }

        public async Task<Stream> OpenReadAsync(
            string objectKey,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key must be provided.", nameof(objectKey));

            await EnsureBucketAsync(cancellationToken);

            var output = new MemoryStream();
            var getArgs = new GetObjectArgs()
                .WithBucket(_options.Bucket)
                .WithObject(objectKey)
                .WithCallbackStream(stream => stream.CopyTo(output));

            await _client.GetObjectAsync(getArgs, cancellationToken);
            output.Position = 0;
            return output;
        }

        private async Task EnsureBucketAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_options.Bucket))
                throw new InvalidOperationException("Minio bucket name is missing.");

            var existsArgs = new BucketExistsArgs()
                .WithBucket(_options.Bucket);

            var exists = await _client.BucketExistsAsync(existsArgs, cancellationToken);
            if (exists)
                return;

            var makeArgs = new MakeBucketArgs()
                .WithBucket(_options.Bucket);

            await _client.MakeBucketAsync(makeArgs, cancellationToken);
        }
    }
}
