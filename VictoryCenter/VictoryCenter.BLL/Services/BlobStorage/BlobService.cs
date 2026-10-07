using System.Buffers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Services.BlobStorage;

public class BlobService : IBlobService
{
    private readonly BlobEnvironmentVariables _blobEnv;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<BlobService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlobService"/> class.
    /// Ensures that the storage directory exists during initialization.
    /// </summary>
    /// <param name="environment">Configuration settings for the blob storage.</param>
    /// <param name="httpContextAccessor">Provides access to <see cref="HttpContext"/> for building absolute URLs.</param>
    /// <param name="logger">The logger used to report best-effort cleanup failures.</param>
    /// <exception cref="BlobFileSystemException">
    /// Thrown if the local storage directory could not be created.
    /// </exception>
    public BlobService(
        IOptions<BlobEnvironmentVariables> environment,
        IHttpContextAccessor httpContextAccessor,
        ILogger<BlobService> logger)
    {
        _blobEnv = environment.Value;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        try
        {
            Directory.CreateDirectory(Path.Combine(_blobEnv.RootPath, _blobEnv.ImagesSubPath));
        }
        catch (Exception ex)
        {
            throw new BlobFileSystemException(Path.Combine(_blobEnv.RootPath, _blobEnv.ImagesSubPath), ImageConstants.FailToCreateDirectory, ex);
        }
    }

    /// <summary>
    /// Saves a file into the blob storage from a Base64-encoded string.
    /// </summary>
    /// <param name="base64">The Base64 string representing the file.</param>
    /// <param name="name">The file name without extension.</param>
    /// <param name="mimeType">The MIME type of the file (e.g., "image/png").</param>
    /// <returns>The saved file name with extension.</returns>
    /// <exception cref="BlobFileSystemException">
    /// Thrown if an unknown error related to the file system occurs.
    /// </exception>
    /// <exception cref="BlobFileNameException">
    /// Thrown if the file name is invalid.
    /// </exception>
    /// <exception cref="InvalidBase64FormatException">
    /// Thrown if the Base64 string has an invalid format.
    /// </exception>
    /// <exception cref="ImageProcessingException">
    /// Thrown if there are problems saving the file to the storage.
    /// </exception>
    public async Task<string> SaveFileInStorageAsync(string base64, string name, string mimeType)
    {
        try
        {
            ValidateFileName(name);

            var imageBytes = ConvertBase64ToBytes(base64);
            var extension = GetExtensionFromMimeType(mimeType);

            await CreateFileAsync(imageBytes, extension, name);

            return $"{name}.{extension}";
        }
        catch (Exception ex) when (ex is not BlobStorageException)
        {
            throw new BlobFileSystemException(_blobEnv.FullPath, ex.Message, ex);
        }
    }

    /// <summary>
    /// Retrieves a file from storage as a <see cref="MemoryStream"/>.
    /// </summary>
    /// <param name="name">The file name without extension.</param>
    /// <param name="mimeType">The MIME type of the file.</param>
    /// <returns>A memory stream containing the file content.</returns>
    /// <exception cref="BlobNotFoundException">
    /// Thrown if the file could not be found in the storage.
    /// </exception>
    /// /// <exception cref="ImageProcessingException">
    /// thrown if there are problems retrieving the image from storage.
    /// </exception>
    /// <exception cref="BlobFileNameException">
    /// Thrown if the file name is invalid.
    /// </exception>
    public async Task<MemoryStream> FindFileInStorageAsMemoryStreamAsync(string name, string mimeType)
    {
        ValidateFileName(name);

        var decodedBytes = await GetFileAsync(name, GetExtensionFromMimeType(mimeType));
        return new MemoryStream(decodedBytes);
    }

    /// <summary>
    /// Builds an absolute URL for accessing a stored file.
    /// </summary>
    /// <param name="name">The file name without extension.</param>
    /// <param name="mimeType">The MIME type of the file.</param>
    /// <returns>The absolute URL of the file.</returns>
    /// <exception cref="BlobFileNameException">
    /// Thrown if the file name is invalid.
    /// </exception>
    /// <exception cref="BlobHttpContextException">
    /// Thrown if <see cref="HttpContext"/> is not available.
    /// </exception>
    /// <exception cref="BlobFileSystemException">
    /// Thrown if another error occurs while building the URL.
    /// </exception>
    public string GetFileUrl(string name, string mimeType)
    {
        ValidateFileName(name);

        try
        {
            var extension = GetExtensionFromMimeType(mimeType);
            var fileName = $"{name}.{extension}";
            HttpRequest? request = _httpContextAccessor.HttpContext?.Request ?? throw new BlobHttpContextException(ImageConstants.HttpContextIsNotAvailable);
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{_blobEnv.ImagesSubPath}/{fileName}";
        }
        catch (Exception ex) when (ex is not BlobStorageException)
        {
            throw new BlobFileSystemException(name, ex.Message, ex);
        }
    }

    /// <summary>
    /// Deletes a file from storage.
    /// If the file does not exist, nothing happens.
    /// </summary>
    /// <param name="name">The file name without extension.</param>
    /// <param name="mimeType">The MIME type of the file.</param>
    /// <exception cref="BlobFileNameException">
    /// Thrown if the file name is invalid.
    /// </exception>
    /// <exception cref="ImageProcessingException">
    /// Thrown if an error occurs while deleting the file.
    /// </exception>
    public void DeleteFileInStorage(string name, string mimeType)
    {
        ValidateFileName(name);

        var fullName = name + "." + GetExtensionFromMimeType(mimeType);
        var filePath = Path.Combine(_blobEnv.FullPath, fullName);
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            throw new ImageProcessingException(name, ErrorMessagesConstants.FailedToDeleteEntity(typeof(Image)), ex);
        }
    }

    private static byte[] ConvertBase64ToBytes(string base64)
    {
        var byteCount = base64.Length * 3 / 4;
        var buffer = ArrayPool<byte>.Shared.Rent(byteCount);
        try
        {
            if (!Convert.TryFromBase64String(base64, buffer, out var bytesWritten))
            {
                throw new InvalidBase64FormatException(ImageConstants.InvalidBase64String);
            }

            var result = new byte[bytesWritten];
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

    private static string GetExtensionFromMimeType(string mimeType)
    {
        return mimeType.ToLower() switch
        {
            ImageMimeTypes.Jpeg => "jpg",
            ImageMimeTypes.Jpg => "jpg",
            ImageMimeTypes.Png => "png",
            ImageMimeTypes.Webp => "webp",
            _ => "jpg"
        };
    }

    private async Task CreateFileAsync(byte[] imageBytes, string type, string name)
    {
        ValidateFileName(name);

        var filePath = Path.Combine(_blobEnv.FullPath, $"{name}.{type}");
        var temporaryFilePath = Path.Combine(
            _blobEnv.FullPath,
            $".{name}.{Guid.NewGuid():N}.tmp");

        try
        {
            await using (var stream = new FileStream(
                             temporaryFilePath,
                             FileMode.CreateNew,
                             FileAccess.Write,
                             FileShare.None,
                             bufferSize: 4096,
                             FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await stream.WriteAsync(imageBytes);
                await stream.FlushAsync();
            }

            File.Move(temporaryFilePath, filePath, overwrite: true);
        }
        catch (Exception ex)
        {
            throw new ImageProcessingException($"{name}.{type}", ImageConstants.FailedToSaveImage, ex);
        }
        finally
        {
            DeleteTemporaryFileIfExists(temporaryFilePath);
        }
    }

    private void DeleteTemporaryFileIfExists(string temporaryFilePath)
    {
        try
        {
            if (File.Exists(temporaryFilePath))
            {
                File.Delete(temporaryFilePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to delete temporary image file {TemporaryFilePath}",
                temporaryFilePath);
        }
    }

    private async Task<byte[]> GetFileAsync(string fileName, string type)
    {
        ValidateFileName(fileName);

        var filePath = Path.Combine(_blobEnv.FullPath, $"{fileName}.{type}");

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

    private static void ValidateFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)
            || name.Contains("..")
            || Path.GetInvalidFileNameChars().Any(name.Contains))
        {
            throw new BlobFileNameException(name, ImageConstants.WrongFileName(name));
        }
    }
}
