namespace BDAplication.Application.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream stream, string blobPath, string contentType);
    Task DeleteAsync(string blobPath);
    Task<string> GetSasUrlAsync(string blobPath, TimeSpan expiry);
}
