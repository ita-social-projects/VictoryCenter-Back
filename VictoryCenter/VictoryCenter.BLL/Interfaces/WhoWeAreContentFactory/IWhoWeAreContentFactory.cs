using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.DAL.Entities.WhoWeAreContents;

namespace VictoryCenter.BLL.Interfaces.WhoWeAreContentFactory;

public interface IWhoWeAreContentFactory
{
    TitleContent UpdateTitle(UpdateWhoWeAreContentDto dto, WhoWeAreContent entity);

    ImageContent UpdateImage(UpdateWhoWeAreContentDto dto, WhoWeAreContent entity);

    DescriptionContent UpdateDescription(UpdateWhoWeAreContentDto dto, WhoWeAreContent entity);

    CardContent UpdateCard(UpdateWhoWeAreContentDto dto, WhoWeAreContent entity);
}
