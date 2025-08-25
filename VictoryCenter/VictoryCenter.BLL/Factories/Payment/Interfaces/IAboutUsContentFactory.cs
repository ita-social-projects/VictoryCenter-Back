using VictoryCenter.BLL.DTOs.AboutUsContent;
using VictoryCenter.DAL.Entities.AboutUsContents;

namespace VictoryCenter.BLL.Factories.Payment.Interfaces;

public interface IAboutUsContentFactory
{
    TitleContent CreateTitle(AboutUsContentDto dto);

    ImageContent CreateImage(AboutUsContentDto dto);

    DescriptionContent CreateDescription(AboutUsContentDto dto);

    CardContent CreateCard(AboutUsContentDto dto);

    TitleContent UpdateTitle(AboutUsContentDto dto, AboutUsContent content);

    ImageContent UpdateImage(AboutUsContentDto dto, AboutUsContent content);

    DescriptionContent UpdateDescription(AboutUsContentDto dto, AboutUsContent content);

    CardContent UpdateCard(AboutUsContentDto dto, AboutUsContent content);
}
