using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.BLL.Interfaces.WhoWeAreContentFactory;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Services.WhoWeAreContentFactory;

public class WhoWeAreContentFactory : IWhoWeAreContentFactory
{
    public TitleContent CreateTitle(CreateWhoWeAreContentDto dto)
    {
        return new TitleContent()
        {
            ContentType = ContentType.Title,
            Title = dto.Title
        };
    }

    public ImageContent CreateImage(CreateWhoWeAreContentDto dto)
    {
        throw new NotImplementedException();
    }

    public DescriptionContent CreateDescription(CreateWhoWeAreContentDto dto)
    {
        throw new NotImplementedException();
    }

    public CardContent CreateCard(CreateWhoWeAreContentDto dto)
    {
        throw new NotImplementedException();
    }

    public TitleContent UpdateTitle(CreateWhoWeAreContentDto dto, WhoWeAreContent entity)
    {
        var result = entity as TitleContent ?? throw new InvalidOperationException("Entity is not Title");

        result.Title = dto.Title;
        return result;
    }

    public ImageContent UpdateImage(CreateWhoWeAreContentDto dto, WhoWeAreContent entity)
    {
        var result = entity as ImageContent ?? throw new InvalidOperationException("Entity is not Image");

        result.ImageId = dto.ImageId;

        return result;
    }

    public DescriptionContent UpdateDescription(CreateWhoWeAreContentDto dto, WhoWeAreContent entity)
    {
        var result = entity as DescriptionContent ?? throw new InvalidOperationException("Entity is not Description");

        result.Description = dto.Description;
        return result;
    }

    public CardContent UpdateCard(CreateWhoWeAreContentDto dto, WhoWeAreContent entity)
    {
        var result = entity as CardContent ?? throw new InvalidOperationException("Entity is not Image");
        result.ImageId = dto.ImageId;
        result.Description = dto.Description;
        return result;
    }
}
