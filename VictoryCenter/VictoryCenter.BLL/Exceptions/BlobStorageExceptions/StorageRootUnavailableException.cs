using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

public sealed class StorageRootUnavailableException : IOException
{
    public const string StableErrorCode = "StorageRootUnavailable";

    public StorageRootUnavailableException(StorageFileKind fileKind, Exception innerException)
        : base($"The {fileKind} storage root is unavailable.", innerException)
    {
        FileKind = fileKind;
    }

    public StorageFileKind FileKind { get; }
}
