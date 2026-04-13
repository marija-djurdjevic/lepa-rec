namespace AngularNetBase.Practice.Services
{
    public record PhotoUpload(Stream Content, string ContentType, string FileName, long SizeBytes);
}
