using VictoryCenter.BLL.DTOs.AboutUsContent;
using VictoryCenter.BLL.Factories.Payment.Interfaces;
using VictoryCenter.DAL.Entities.AboutUsContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Factories.Payment.Implementations;

public class AboutUsContentFactory : IAboutUsContentFactory
{
    public TitleContent CreateTitle(CreateAboutUsContentDto dto)
    {
        return new TitleContent()
        {
            ContentType = ContentType.Title,
            Title = dto.Title
        };
    }

    public ImageContent CreateImage(CreateAboutUsContentDto dto)
    {
        throw new NotImplementedException();
    }

    public DescriptionContent CreateDescription(CreateAboutUsContentDto dto)
    {
        throw new NotImplementedException();
    }

    public CardContent CreateCard(CreateAboutUsContentDto dto)
    {
        throw new NotImplementedException();
    }

    public TitleContent UpdateTitle(CreateAboutUsContentDto dto, AboutUsContent entity)
    {
        var result = entity as TitleContent ?? throw new InvalidOperationException("Entity is not Title");

        result.Title = dto.Title;
        return result;
    }

    public ImageContent UpdateImage(CreateAboutUsContentDto dto, AboutUsContent entity)
    {
        var result = entity as ImageContent ?? throw new InvalidOperationException("Entity is not Image");

        result.ImageId = dto.ImageId;

        return result;
    }

    public DescriptionContent UpdateDescription(CreateAboutUsContentDto dto, AboutUsContent entity)
    {
        var result = entity as DescriptionContent ?? throw new InvalidOperationException("Entity is not Description");

        result.Description = dto.Description;
        return result;
    }

    public CardContent UpdateCard(CreateAboutUsContentDto dto, AboutUsContent entity)
    {
        var result = entity as CardContent ?? throw new InvalidOperationException("Entity is not Image");
        result.ImageId = dto.ImageId;
        result.Description = dto.Description;
        return result;
    }
}
