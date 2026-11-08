using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Services.StorageLifecycle;

public class ImageStorageService(BlobService storage, StorageUploadReservations reservations) : IBlobService
{
    public async Task<string> SaveFileInStorageAsync(string base64, string name, string mimeType)
    {
        await reservations.ReserveAsync(StorageFileKind.Image, StorageFileNames.Image(name, mimeType));
        return await storage.SaveFileInStorageAsync(base64, name, mimeType);
    }

    public Task<MemoryStream> FindFileInStorageAsMemoryStreamAsync(string name, string mimeType)
        => storage.FindFileInStorageAsMemoryStreamAsync(name, mimeType);

    public string GetFileUrl(string name, string mimeType) => storage.GetFileUrl(name, mimeType);

    public void DeleteFileInStorage(string name, string mimeType)
        => reservations.ScheduleDeletion(StorageFileKind.Image, StorageFileNames.Image(name, mimeType));
}
