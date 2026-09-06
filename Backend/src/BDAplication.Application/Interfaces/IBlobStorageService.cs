namespace BDAplication.Application.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream stream, string blobPath, string contentType);
    Task DeleteAsync(string blobPath);
    Task<string> GetSasUrlAsync(string blobPath, TimeSpan expiry);

    /// <summary>URL SAS de escritura para que el navegador suba un archivo directo a Blob Storage,
    /// sin pasar por el circuito Blazor Server ni por el body de la API (necesario para video).</summary>
    Task<string> GetSasUploadUrlAsync(string blobPath, TimeSpan expiry);

    /// <summary>Tamaño real del blob y sus primeros bytes, para validar la firma binaria (magic bytes)
    /// de un archivo subido directamente a Blob sin descargarlo completo al servidor.</summary>
    Task<(long SizeBytes, byte[] Header)> GetBlobHeaderAsync(string blobPath, int headerBytes = 64);
}
