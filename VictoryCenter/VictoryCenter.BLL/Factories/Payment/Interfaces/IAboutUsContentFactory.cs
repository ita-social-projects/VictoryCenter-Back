using VictoryCenter.BLL.DTOs.AboutUsContent;
using VictoryCenter.DAL.Entities.AboutUsContents;

namespace VictoryCenter.BLL.Factories.Payment.Interfaces;

public interface IAboutUsContentFactory
{
    TitleContent CreateTitle(CreateAboutUsContentDto dto);

    ImageContent CreateImage(CreateAboutUsContentDto dto);

    DescriptionContent CreateDescription(CreateAboutUsContentDto dto);

    CardContent CreateCard(CreateAboutUsContentDto dto);

    TitleContent UpdateTitle(CreateAboutUsContentDto dto, AboutUsContent content);

    ImageContent UpdateImage(CreateAboutUsContentDto dto, AboutUsContent content);

    DescriptionContent UpdateDescription(CreateAboutUsContentDto dto, AboutUsContent content);

    CardContent UpdateCard(CreateAboutUsContentDto dto, AboutUsContent content);
}
