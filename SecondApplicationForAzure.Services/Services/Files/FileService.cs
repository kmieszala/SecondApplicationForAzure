using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using SecondApplicationForAzure.Common.Configuration;

namespace SecondApplicationForAzure.Services.Services.Files;

public interface IFileService
{
    Task<IEnumerable<string>> GetListBlobs();

    Task<string> AddDefaultTxtFile(string message);
}

public class FileService : IFileService
{
    private readonly AzureStorageBlobSection _configStorageBlob;

    public FileService(
        IOptions<AzureStorageBlobSection> configStorageBlob)
    {
        _configStorageBlob = configStorageBlob.Value;
    }

    public async Task<string> AddDefaultTxtFile(string message)
    {
        var fileName = Guid.NewGuid().ToString() + ".txt";
        await File.WriteAllTextAsync(fileName, message);

        var blobServiceClient = new BlobServiceClient(_configStorageBlob.ConnectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_configStorageBlob.BlobContainerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileName, true);

        return fileName;
    }

    public async Task<IEnumerable<string>> GetListBlobs()
    {
        var blobServiceClient = new BlobServiceClient(_configStorageBlob.ConnectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_configStorageBlob.BlobContainerName);

        var result = new List<string>();

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            Console.WriteLine("\t" + blobItem.Name);
            result.Add(blobItem.Name);
        }

        return result;
    }
}