using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.DAL.Entities.WhoWeAreContents;

namespace VictoryCenter.BLL.Interfaces.WhoWeAreContentFactory;

public interface IWhoWeAreContentFactory
{
    TitleContent CreateTitle(CreateWhoWeAreContentDto dto);

    ImageContent CreateImage(CreateWhoWeAreContentDto dto);

    DescriptionContent CreateDescription(CreateWhoWeAreContentDto dto);

    CardContent CreateCard(CreateWhoWeAreContentDto dto);

    TitleContent UpdateTitle(CreateWhoWeAreContentDto dto, WhoWeAreContent content);

    ImageContent UpdateImage(CreateWhoWeAreContentDto dto, WhoWeAreContent content);

    DescriptionContent UpdateDescription(CreateWhoWeAreContentDto dto, WhoWeAreContent content);

    CardContent UpdateCard(CreateWhoWeAreContentDto dto, WhoWeAreContent content);
}
