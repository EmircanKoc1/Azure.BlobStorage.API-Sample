using Azure.BlobStorage.API_Sample.Options;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;

namespace Azure.BlobStorage.API_Sample.Services
{
    public class FileService : IFileService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobDefaultContainerClient;


        public FileService(IOptions<AzureBlobOptions> options)
        {
            var optionsValue = options.Value;
            _blobServiceClient = new BlobServiceClient(optionsValue.ConnectionString);

            _blobDefaultContainerClient = new BlobContainerClient(
                connectionString: optionsValue.ConnectionString,
                blobContainerName: optionsValue.DefaultContainerName);

            _blobDefaultContainerClient.CreateIfNotExists();


        }

        public async Task<bool> DeleteFileAsync(string fileName, [Optional] string containerName)
        {

            var blobContainerClient = await GetDefaultOrOtherBlobContainerAsync(containerName);

            try
            {
                var deleteResponse = await blobContainerClient.DeleteBlobIfExistsAsync(fileName);
            }
            catch (Exception)
            {
                return false;
            }


            return true;


        }

        public async Task<byte[]?> GetFileAsync(string fileName, [Optional] string containerName)
        {
            var blobContainerClient = await GetDefaultOrOtherBlobContainerAsync(containerName);

            var blobClient = blobContainerClient.GetBlobClient(fileName);

            if (!await IsExistsBlobClientAsync(blobClient))
                return null;

            var response = await blobClient.DownloadContentAsync();

            return response.Value.Content.ToArray();

        }


        public async Task<bool> UploadFileAsync(byte[] file, string fileName, [Optional] string containerName)
        {
            var blobContainerClient = await GetDefaultOrOtherBlobContainerAsync(containerName);

            var blobClient = blobContainerClient.GetBlobClient(fileName);

            if (await IsExistsBlobClientAsync(blobClient))
                return false;

            var response = await blobContainerClient.UploadBlobAsync(
                blobName: fileName, new BinaryData(file));



            return !response.GetRawResponse().IsError;

        }

        async Task<BlobContainerClient> GetDefaultOrOtherBlobContainerAsync(string containerName)
        {
            if (containerName is not null)
            {
                await CreateBlobContainerIfNotExistsAsync(containerName);


                return _blobServiceClient.GetBlobContainerClient(containerName);
            }

            return _blobDefaultContainerClient;

        }

        protected async Task<bool> IsExistsBlobClientAsync(BlobClient client)
        {
            return (await client.ExistsAsync()) && client is not null;
        }

        async Task CreateBlobContainerIfNotExistsAsync(string containerName)
        => await _blobServiceClient.GetBlobContainerClient(containerName).CreateIfNotExistsAsync();


    }


}
