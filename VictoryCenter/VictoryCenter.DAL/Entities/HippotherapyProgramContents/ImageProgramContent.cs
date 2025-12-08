namespace VictoryCenter.DAL.Entities.HippotherapyProgramContents;

public class ImageProgramContent : ProgramSectionContent
{
    public long ImageId { get; set; }

    public Image Image { get; set; } = null!;
}
