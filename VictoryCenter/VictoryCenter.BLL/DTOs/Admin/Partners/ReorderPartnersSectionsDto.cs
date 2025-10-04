namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record ReorderPartnersSectionsDto
{
    public List<long> OrderedIds { get; init; } = [];
}
