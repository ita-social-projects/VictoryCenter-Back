using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeder.ImageSeeder;

public class ImagesDataSeeder
{
    private static readonly List<(string blobName, string base64Data, string mimeType)> _imageData =
    [
        (
            blobName: "testname1",
            base64Data: "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQACWAJYAAD/2wCEAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0vUn0YG+V6bB7UAJ3Od1ssbbS.....nsVYLELMzP5HdONT1td26bVasH0M35MFoLPpdsgQcxqNd5Tt+i2R+xO2cbHIr06KuKc/ccv46GGhTneNAJprfabXh+hm/LhndHl2THteKt4E0TZoyx3dRsEbAwbDoGkDRFwAqVaLXi+mPbmskcw1aorQ2TI5HnSStjFSpZ3SZdvwEVqLcn5hMe14q08okAVKltYGTEXFxqfwTXFpq0qO19nprg4VB42yktTG5NzKkldJ8j+Ga4tNWlMtZHzCZMx+x4TpGt+RT7YPsCfI9/wAj+LZM9mxTbYfuCbaozvkmva7Y6RIG5TrRG3unWwfaE60SO7/khI4bFC0Sjuha5O6/WP8AAX6x/gL9Y/wEbXJ+yNplPdGV53KOf/gn/9k=",
            mimeType: "image/jpeg"
        ),
        (
            blobName: "testname2",
            base64Data: "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAkLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwsLCwv/wQARCAFnAoADABEAAREA..../aMLVZ4+qlZ+sjxwwVrEdyeDn7hfbR4ssrofK1JfLCcHCu7ltIQ9xnRf8AD4L6cftmTDgr1+BxeG93p1P8yuf0JBNmmEYgHuYGgRmgKAdTC6Kx+NaWGoU8+d20wXypp8z5gbLLh+47HWZcPiMPR69/3YZLJhu5HR1Xu878V5UuCh6tX85GS0YbBYTBx1cNh6aPe64xIrcABQAACJCgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAB//9k=",
            mimeType: "image/jpeg"
        )
    ];

    public static async Task SeedData(VictoryCenterDbContext dbContext, IBlobService blobService)
    {
        var images = new List<Image>();

        foreach (var imageData in _imageData)
        {
            var fileName = await blobService.SaveFileInStorageAsync(
                imageData.base64Data,
                imageData.blobName,
                imageData.mimeType);

            var fileUrl = blobService.GetFileUrl(imageData.blobName, imageData.mimeType);

            var image = new Image
            {
                BlobName = imageData.blobName,
                Url = fileUrl,
                MimeType = imageData.mimeType,
                CreatedAt = DateTime.UtcNow
            };

            images.Add(image);
        }

        dbContext.Images.AddRange(images);
        await dbContext.SaveChangesAsync();
    }
}
