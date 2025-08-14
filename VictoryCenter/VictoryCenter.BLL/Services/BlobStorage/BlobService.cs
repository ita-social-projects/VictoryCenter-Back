using System.Buffers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Services.BlobStorage;

public class BlobService : IBlobService
{
    private readonly string _keyCrypt;
    private readonly string _blobPath;

    public BlobService(IOptions<BlobEnvironmentVariables> environment)
    {
        _keyCrypt = environment.Value.BlobStoreKey;
        _blobPath = environment.Value.BlobStorePath;
    }

    public string BlobPath => _blobPath;

    public async Task<string> SaveFileInStorageAsync(string base64, string name, string mimeType)
    {
        try
        {
            byte[] imageBytes = ConvertBase64ToBytes(base64);
            string extension = GetExtensionFromMimeType(mimeType);

            Directory.CreateDirectory(_blobPath);
            await EncryptFileAsync(imageBytes, extension, name);

            return $"{name}.{extension}";
        }
        catch (Exception ex) when (ex is not BlobStorageException)
        {
            throw new BlobFileSystemException(BlobPath, ex.Message, ex);
        }
    }

    public async Task<MemoryStream> FindFileInStorageAsMemoryStreamAsync(string name, string mimeType)
    {
        byte[] decodedBytes = await DecryptFileAsync(name, GetExtensionFromMimeType(mimeType));
        return new MemoryStream(decodedBytes);
    }

    public async Task<string> FindFileInStorageAsBase64Async(string name, string mimeType)
    {
        try
        {
            using var stream = await FindFileInStorageAsMemoryStreamAsync(name, mimeType);
            return Convert.ToBase64String(stream.ToArray());
        }
        catch (Exception ex) when (ex is not BlobStorageException)
        {
            throw new InvalidBase64FormatException(ex.Message, ex);
        }
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
        string filePath = Path.Combine(_blobPath, fullName);
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            throw new BlobFileSystemException(filePath, ErrorMessagesConstants.FailedToDeleteEntity(typeof(Image)), ex);
        }
    }

    private static byte[] ConvertBase64ToBytes(string base64)
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

    private static string GetExtensionFromMimeType(string mimeType)
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

    private async Task EncryptFileAsync(byte[] imageBytes, string type, string name)
    {
        string filePath = Path.Combine(_blobPath, $"{name}.{type}");
        byte[] keyBytes = Encoding.UTF8.GetBytes(_keyCrypt.PadRight(32)[..32]);

        try
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Key = keyBytes;
            aes.GenerateIV();

            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            fileStream.Write(aes.IV, 0, aes.IV.Length);

            using var cryptoStream = new CryptoStream(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

            int bufferSize = 4096;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

            try
            {
                using var memoryStream = new MemoryStream(imageBytes);
                int bytesRead;
                while ((bytesRead = memoryStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    cryptoStream.Write(buffer, 0, bytesRead);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            await cryptoStream.FlushAsync();
        }
        catch (Exception ex) when (ex is not BlobStorageException)
        {
            throw new BlobCryptographyException($"{name}.{type}", ImageConstants.EncryptionFailed, ex);
        }
    }

    private async Task<byte[]> DecryptFileAsync(string fileName, string type)
    {
        string filePath = Path.Combine(_blobPath, $"{fileName}.{type}");

        if (!File.Exists(filePath))
        {
            throw new BlobNotFoundException(fileName, ImageConstants.FileNotFound(filePath));
        }

        try
        {
            await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] keyBytes = Encoding.UTF8.GetBytes(_keyCrypt.PadRight(32)[..32]);

            byte[] iv = new byte[16];
            if (fileStream.Read(iv, 0, iv.Length) != iv.Length)
            {
                throw new BlobCryptographyException(fileName, ImageConstants.InvalidIVLength);
            }

            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Key = keyBytes;
            aes.IV = iv;

            await using var cryptoStream = new CryptoStream(fileStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            await using var memoryStream = new MemoryStream();

            int bufferSize = 4096;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

            try
            {
                int bytesRead;
                while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    await memoryStream.WriteAsync(buffer, 0, bytesRead);
                }

                return memoryStream.ToArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        catch (Exception ex) when (ex is not BlobStorageException)
        {
            throw new BlobCryptographyException(fileName, ImageConstants.DecryptionFailed, ex);
        }
    }
}
