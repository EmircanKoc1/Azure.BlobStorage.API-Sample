using System.Runtime.InteropServices;

namespace Azure.BlobStorage.API_Sample.Services
{
    public interface IFileService
    {

        Task<bool> DeleteFileAsync(string fileName, [Optional] string containerName);
        Task<bool> UploadFileAsync(byte[] file, string fileName, [Optional] string containerName);
        Task<byte[]?> GetFileAsync(string fileName, [Optional] string containerName);


    }
}
