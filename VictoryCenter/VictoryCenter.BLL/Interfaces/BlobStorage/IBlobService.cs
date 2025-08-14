namespace VictoryCenter.BLL.Interfaces.BlobStorage;

public interface IBlobService
{
    Task<string> SaveFileInStorageAsync(string base64, string name, string mimeType);
    Task<MemoryStream> FindFileInStorageAsMemoryStreamAsync(string name, string mimeType);
    Task<string> FindFileInStorageAsBase64Async(string name, string mimeType);
    Task<string> UpdateFileInStorageAsync(string previousBlobName, string previousMimeType, string base64Format, string newBlobName, string mimeType);
    void DeleteFileInStorage(string name, string mimeType);
}
