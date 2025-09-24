using System.Text.Json.Serialization;
using ContentType = VictoryCenter.DAL.Enums.ContentType;

namespace VictoryCenter.BLL.DTOs.Common.WhoWeAreContent;

[JsonDerivedType(typeof(DescriptionContentDto), typeDiscriminator: "description")]
[JsonDerivedType(typeof(ImageContentDto), typeDiscriminator: "image")]
[JsonDerivedType(typeof(TitleContentDto), typeDiscriminator: "title")]
[JsonDerivedType(typeof(CardContentDto), typeDiscriminator: "card")]
public class WhoWeAreContentDto
{
    public long Id { get; set; }

    public ContentType ContentType { get; set; }
}
