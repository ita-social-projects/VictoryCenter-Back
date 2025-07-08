namespace VictoryCenter.BLL.Interfaces.BlobStorage;
public interface IBlobService
{
    string SaveFileInStorage(string base64, string name, string mimeType);
    MemoryStream FindFileInStorageAsMemoryStream(string? name, string mimetype);
    string FindFileInStorageAsBase64(string name, string mimeType);
    string UpdateFileInStorage(string? previousBlobName, string previousMimeType, string base64Format, string newBlobName, string extension);
    void DeleteFileInStorage(string? name, string mimeType);
}
