using VictoryCenter.BLL.DTOs.AboutUsContent;
using VictoryCenter.BLL.Factories.Payment.Interfaces;
using VictoryCenter.DAL.Entities.AboutUsContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Factories.Payment.Implementations;

public class AboutUsContentFactory : IAboutUsContentFactory
{
    public TitleContent CreateTitle(AboutUsContentDto dto)
    {
        return new TitleContent()
        {
            ContentType = ContentType.Title,
            Title = dto.Title
        };
    }

    public ImageContent CreateImage(AboutUsContentDto dto)
    {
        throw new NotImplementedException();
    }

    public DescriptionContent CreateDescription(AboutUsContentDto dto)
    {
        throw new NotImplementedException();
    }

    public CardContent CreateCard(AboutUsContentDto dto)
    {
        throw new NotImplementedException();
    }

    public TitleContent UpdateTitle(AboutUsContentDto dto, AboutUsContent entity)
    {
        var result = entity as TitleContent ?? throw new InvalidOperationException("Entity is not Title");

        result.Title = dto.Title;
        return result;
    }

    public ImageContent UpdateImage(AboutUsContentDto dto, AboutUsContent entity)
    {
        var result = entity as ImageContent ?? throw new InvalidOperationException("Entity is not Image");

        result.ImageId = dto.ImageId;

        return result;
    }

    public DescriptionContent UpdateDescription(AboutUsContentDto dto, AboutUsContent entity)
    {
        var result = entity as DescriptionContent ?? throw new InvalidOperationException("Entity is not Description");

        result.Description = dto.Description;
        return result;
    }

    public CardContent UpdateCard(AboutUsContentDto dto, AboutUsContent entity)
    {
        var result = entity as CardContent ?? throw new InvalidOperationException("Entity is not Image");
        result.ImageId = dto.ImageId;
        result.Description = dto.Description;
        return result;
    }
}
