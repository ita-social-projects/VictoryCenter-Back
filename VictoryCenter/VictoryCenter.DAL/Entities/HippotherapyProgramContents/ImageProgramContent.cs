using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.HippotherapyProgramContents;

public class ImageProgramContent : ProgramSectionContent
{
    [Audited]
    public long ImageId { get; set; }

    public Image Image { get; set; } = null!;
}
