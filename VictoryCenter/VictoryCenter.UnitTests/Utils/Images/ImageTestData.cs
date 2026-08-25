using System.Buffers.Binary;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.UnitTests.Utils.Images;

public static class ImageTestData
{
    private const int BitsPerByte = 8;
    private const uint Crc32LeastSignificantBitMask = 1;
    private const uint Crc32Polynomial = 0xEDB88320U;
    private const string GifMimeType = "image/gif";
    private const int PngHeaderLength = 13;

    private static readonly byte[] PngSignature = [137, 80, 78, 71, 13, 10, 26, 10];

    /// <summary>
    /// Creates a small valid image and returns its encoded bytes as Base64 test data.
    /// </summary>
    /// <param name="mimeType">Image format to encode.</param>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    /// <param name="animated">Whether to add a second frame.</param>
    /// <returns>A Base64-encoded image.</returns>
    public static string CreateBase64(
        string mimeType,
        int width = 2,
        int height = 2,
        bool animated = false)
    {
        using var image = new Image<Rgba32>(width, height, Color.Red);
        if (animated)
        {
            image.Frames.AddFrame(image.Frames.RootFrame);
        }

        using var stream = new MemoryStream();
        image.Save(stream, GetEncoder(mimeType));
        return Convert.ToBase64String(stream.ToArray());
    }

    /// <summary>
    /// Creates a minimal PNG structure containing dimensions and pixel format information.
    /// It is used to test early resource-limit rejection without allocating a full large image.
    /// </summary>
    /// <param name="width">Width written to the PNG IHDR chunk.</param>
    /// <param name="height">Height written to the PNG IHDR chunk.</param>
    /// <param name="bitDepth">PNG channel bit depth.</param>
    /// <param name="colorType">PNG color-type identifier.</param>
    /// <returns>The minimal PNG structure encoded as Base64.</returns>
    public static string CreatePngHeader(int width, int height, byte bitDepth = 8, byte colorType = 6)
    {
        using var stream = new MemoryStream();
        stream.Write(PngSignature);

        Span<byte> header = stackalloc byte[PngHeaderLength];
        BinaryPrimitives.WriteInt32BigEndian(header, width);
        BinaryPrimitives.WriteInt32BigEndian(header[4..], height);
        header[8] = bitDepth;
        header[9] = colorType;
        WritePngChunk(stream, "IHDR", header);
        WritePngChunk(stream, "IEND", []);

        return Convert.ToBase64String(stream.ToArray());
    }

    /// <summary>
    /// Selects an ImageSharp encoder for a test MIME type.
    /// </summary>
    /// <param name="mimeType">MIME type to encode.</param>
    /// <returns>An encoder for the requested test format.</returns>
    private static IImageEncoder GetEncoder(string mimeType)
    {
        return mimeType.ToLowerInvariant() switch
        {
            ImageMimeTypes.Jpeg or ImageMimeTypes.Jpg => new JpegEncoder(),
            ImageMimeTypes.Png => new PngEncoder(),
            ImageMimeTypes.Webp => new WebpEncoder(),
            GifMimeType => new GifEncoder(),
            _ => throw new ArgumentOutOfRangeException(nameof(mimeType), mimeType, null)
        };
    }

    /// <summary>
    /// Writes a PNG chunk as length, ASCII chunk type, data, and CRC-32 checksum.
    /// </summary>
    /// <param name="stream">Destination PNG stream.</param>
    /// <param name="type">Four-character PNG chunk type.</param>
    /// <param name="data">Chunk payload.</param>
    private static void WritePngChunk(Stream stream, string type, ReadOnlySpan<byte> data)
    {
        Span<byte> length = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(length, data.Length);
        stream.Write(length);

        byte[] typeBytes = Encoding.ASCII.GetBytes(type);
        stream.Write(typeBytes);
        stream.Write(data);

        byte[] crcInput = new byte[typeBytes.Length + data.Length];
        typeBytes.CopyTo(crcInput, 0);
        data.CopyTo(crcInput.AsSpan(typeBytes.Length));

        Span<byte> crc = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(crc, ComputeCrc32(crcInput));
        stream.Write(crc);
    }

    /// <summary>
    /// Computes the CRC-32 checksum required by PNG chunks using the reversed
    /// IEEE polynomial <c>0xEDB88320</c>. Each input byte is processed bit by bit;
    /// the final complement is the checksum stored in the PNG stream.
    /// </summary>
    /// <param name="data">PNG chunk type and payload bytes.</param>
    /// <returns>The CRC-32 checksum for the supplied bytes.</returns>
    private static uint ComputeCrc32(ReadOnlySpan<byte> data)
    {
        uint crc = uint.MaxValue;
        foreach (byte value in data)
        {
            crc ^= value;
            for (var bit = 0; bit < BitsPerByte; bit++)
            {
                crc = (crc & Crc32LeastSignificantBitMask) == Crc32LeastSignificantBitMask
                    ? (crc >> 1) ^ Crc32Polynomial
                    : crc >> 1;
            }
        }

        return ~crc;
    }
}
