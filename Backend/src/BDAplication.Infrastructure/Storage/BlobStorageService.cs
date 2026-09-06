using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using BDAplication.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BDAplication.Infrastructure.Storage;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _client;
    private readonly string _containerName;

    public BlobStorageService(IConfiguration config)
    {
        var cs = config["BlobStorage:Conn"]
            ?? throw new InvalidOperationException("BlobStorage:Conn not configured");
        _containerName = config["BlobStorage:Container"] ?? "attachments";
        _client = new BlobServiceClient(cs);
    }

    public async Task<string> UploadAsync(Stream stream, string blobPath, string contentType)
    {
        var container = _client.GetBlobContainerClient(_containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None);

        var blob = container.GetBlobClient(blobPath);
        await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });
        return blob.Uri.ToString();
    }

    public async Task DeleteAsync(string blobPath)
    {
        var container = _client.GetBlobContainerClient(_containerName);
        var blob = container.GetBlobClient(blobPath);
        await blob.DeleteIfExistsAsync();
    }

    public Task<string> GetSasUrlAsync(string blobPath, TimeSpan expiry)
    {
        var container = _client.GetBlobContainerClient(_containerName);
        var blob = container.GetBlobClient(blobPath);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobPath,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiry)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var uri = blob.GenerateSasUri(sasBuilder);
        return Task.FromResult(uri.ToString());
    }

    public async Task<string> GetSasUploadUrlAsync(string blobPath, TimeSpan expiry)
    {
        var container = _client.GetBlobContainerClient(_containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None);

        var blob = container.GetBlobClient(blobPath);
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobPath,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiry)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);

        var uri = blob.GenerateSasUri(sasBuilder);
        return uri.ToString();
    }

    public async Task<(long SizeBytes, byte[] Header)> GetBlobHeaderAsync(string blobPath, int headerBytes = 64)
    {
        var container = _client.GetBlobContainerClient(_containerName);
        var blob = container.GetBlobClient(blobPath);

        var props = await blob.GetPropertiesAsync();
        var len = (int)Math.Min(headerBytes, props.Value.ContentLength);
        var buffer = Array.Empty<byte>();

        if (len > 0)
        {
            var download = await blob.DownloadStreamingAsync(new BlobDownloadOptions
            {
                Range = new HttpRange(0, len)
            });
            buffer = new byte[len];
            await using var stream = download.Value.Content;
            var offset = 0;
            while (offset < len)
            {
                var read = await stream.ReadAsync(buffer.AsMemory(offset, len - offset));
                if (read == 0) break;
                offset += read;
            }
        }

        return (props.Value.ContentLength, buffer);
    }
}
