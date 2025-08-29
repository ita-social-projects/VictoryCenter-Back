using System.Text.Json.Serialization;
using ContentType = VictoryCenter.DAL.Enums.ContentType;

namespace VictoryCenter.BLL.DTOs.AboutUsContent;

[JsonDerivedType(typeof(DescriptionContentDto), typeDiscriminator: "description")]
[JsonDerivedType(typeof(ImageContentDto), typeDiscriminator: "image")]
[JsonDerivedType(typeof(TitleContentDto), typeDiscriminator: "title")]
public class AboutUsContentDto
{
    public ContentType ContentType { get; set; }

    public long Id { get; set; }
}
