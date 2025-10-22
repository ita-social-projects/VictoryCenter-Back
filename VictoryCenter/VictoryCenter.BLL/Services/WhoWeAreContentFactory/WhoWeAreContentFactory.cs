using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.BLL.Interfaces.WhoWeAreContentFactory;
using VictoryCenter.DAL.Entities.WhoWeAreContents;

namespace VictoryCenter.BLL.Services.WhoWeAreContentFactory;

public class WhoWeAreContentFactory : IWhoWeAreContentFactory
{
    public TitleContent UpdateTitle(UpdateWhoWeAreContentDto dto, WhoWeAreContent entity)
    {
        var result = entity as TitleContent
                     ?? throw new InvalidOperationException(WhoWeAreConstants.EntityIsNotRightContent(typeof(TitleContent)));

        result.Title = dto.Title;
        return result;
    }

    public ImageContent UpdateImage(UpdateWhoWeAreContentDto dto, WhoWeAreContent entity)
    {
        var result = entity as ImageContent
                     ?? throw new InvalidOperationException(WhoWeAreConstants.EntityIsNotRightContent(typeof(ImageContent)));

        result.ImageId = dto.ImageId;

        return result;
    }

    public DescriptionContent UpdateDescription(UpdateWhoWeAreContentDto dto, WhoWeAreContent entity)
    {
        var result = entity as DescriptionContent
                     ?? throw new InvalidOperationException(WhoWeAreConstants.EntityIsNotRightContent(typeof(DescriptionContent)));

        result.Description = dto.Description;
        return result;
    }

    public CardContent UpdateCard(UpdateWhoWeAreContentDto dto, WhoWeAreContent entity)
    {
        var result = entity as CardContent
                     ?? throw new InvalidOperationException(WhoWeAreConstants.EntityIsNotRightContent(typeof(CardContent)));

        result.ImageId = dto.ImageId;
        result.Description = dto.Description;
        return result;
    }
}
