using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.IntegrationTests.Utils.Images;

public static class ImageTestData
{
    public static string CreateBase64(string mimeType, int width = 2, int height = 2)
    {
        using var image = new Image<Rgba32>(width, height, Color.Red);
        using var stream = new MemoryStream();
        image.Save(stream, GetEncoder(mimeType));
        return Convert.ToBase64String(stream.ToArray());
    }

    private static IImageEncoder GetEncoder(string mimeType)
    {
        return mimeType.ToLowerInvariant() switch
        {
            ImageMimeTypes.Jpeg or ImageMimeTypes.Jpg => new JpegEncoder(),
            ImageMimeTypes.Png => new PngEncoder(),
            ImageMimeTypes.Webp => new WebpEncoder(),
            _ => throw new ArgumentOutOfRangeException(nameof(mimeType), mimeType, null)
        };
    }
}
