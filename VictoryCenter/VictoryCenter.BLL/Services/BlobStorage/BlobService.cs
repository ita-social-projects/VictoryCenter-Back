using System.Buffers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions;

namespace VictoryCenter.BLL.Services.BlobStorage;

public class BlobService : IBlobService
{
    private readonly BlobEnvironmentVariables _blobEnv;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BlobService(IOptions<BlobEnvironmentVariables> environment, IHttpContextAccessor httpContextAccessor)
    {
        _blobEnv = environment.Value;
        _httpContextAccessor = httpContextAccessor;
        Directory.CreateDirectory(Path.Combine(_blobEnv.RootPath, _blobEnv.ImagesSubPath));
    }

    public async Task<string> SaveFileInStorageAsync(string base64, string name, string mimeType)
    {
        try
        {
            byte[] imageBytes = ConvertBase64ToBytes(base64);
            string extension = GetExtensionFromMimeType(mimeType);

            Directory.CreateDirectory(_blobEnv.FullPath);
            await CreateFileAsync(imageBytes, extension, name);

            return $"{name}.{extension}";
        }
        catch (Exception ex) when (ex is not BlobStorageException)
        {
            throw new BlobFileSystemException(_blobEnv.FullPath, ex.Message, ex);
        }
    }

    public async Task<MemoryStream> FindFileInStorageAsMemoryStreamAsync(string name, string mimeType)
    {
            byte[] decodedBytes = await GetFileAsync(name, GetExtensionFromMimeType(mimeType));
            return new MemoryStream(decodedBytes);
    }

    public string GetFileUrl(string name, string mimeType)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Contains("..") || Path.GetInvalidFileNameChars().Any(name.Contains))
        {
            throw new BlobFileNameException(name, ImageConstants.CantGetFile(name));
        }

        var extension = GetExtensionFromMimeType(mimeType);
        var fileName = $"{name}.{extension}";
        var request = _httpContextAccessor.HttpContext?.Request;

        if (request == null)
        {
            throw new InvalidOperationException("HttpContext is not available.");
        }

        var baseUrl = $"{request.Scheme}://{request.Host}";
        return $"{baseUrl}/{_blobEnv.ImagesSubPath}/{fileName}";
    }

    public async Task<string> UpdateFileInStorageAsync(string previousBlobName, string previousMimeType, string base64Format, string newBlobName, string mimeType)
    {
        DeleteFileInStorage(previousBlobName, previousMimeType);
        await SaveFileInStorageAsync(base64Format, newBlobName, mimeType);
        return newBlobName;
    }

    public void DeleteFileInStorage(string name, string mimeType)
    {
        var fullName = name + "." + GetExtensionFromMimeType(mimeType);
        string filePath = Path.Combine(_blobEnv.FullPath, fullName);
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            throw new BlobFileSystemException(filePath, ImageConstants.FailToDeleteImage, ex);
        }
    }

    private byte[] ConvertBase64ToBytes(string base64)
    {
        if (base64.Contains(','))
        {
            base64 = base64.Split(',')[1];
        }

        int byteCount = base64.Length * 3 / 4;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(byteCount);
        try
        {
            if (!Convert.TryFromBase64String(base64, buffer, out int bytesWritten))
            {
                throw new InvalidBase64FormatException(ImageConstants.InvalidBase64String);
            }

            byte[] result = new byte[bytesWritten];
            Array.Copy(buffer, result, bytesWritten);
            return result;
        }
        catch (Exception ex) when (ex is not InvalidBase64FormatException)
        {
            throw new InvalidBase64FormatException(ImageConstants.FailedToConvertBase64);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private string GetExtensionFromMimeType(string mimeType)
    {
        return mimeType.ToLower() switch
        {
            "image/jpeg" => "jpg",
            "image/jpg" => "jpg",
            "image/png" => "png",
            "image/webp" => "webp",
            _ => "jpg"
        };
    }

    private async Task CreateFileAsync(byte[] imageBytes, string type, string name)
    {
        string filePath = Path.Combine(_blobEnv.FullPath, $"{name}.{type}");

        try
        {
            await File.WriteAllBytesAsync(filePath, imageBytes);
        }
        catch (Exception ex)
        {
            throw new ImageProcessingException($"{name}.{type}", ImageConstants.FailedToSaveImage, ex);
        }
    }

    private async Task<byte[]> GetFileAsync(string fileName, string type)
    {
        string filePath = Path.Combine(_blobEnv.FullPath, $"{fileName}.{type}");

        if (!File.Exists(filePath))
        {
            throw new BlobNotFoundException(fileName, ImageConstants.FileNotFound(filePath));
        }

        try
        {
            return await File.ReadAllBytesAsync(filePath);
        }
        catch (Exception ex) when (ex is not BlobStorageException)
        {
            throw new ImageProcessingException(fileName, ImageConstants.FailedToReadImage, ex);
        }
    }
}
