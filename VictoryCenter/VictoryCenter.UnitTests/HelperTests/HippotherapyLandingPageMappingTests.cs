using AutoMapper;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.Mapping.HippotherapyLandingPage;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.UnitTests.Utils.HippotherapyLandingPageTestDataHelper;

public class HippotherapyLandingPageMappingTests
{
    private readonly IMapper _mapper = new MapperConfiguration(c => c.AddProfile<HippotherapyLandingPageProfile>())
        .CreateMapper();

    [Fact]
    public void Map_ShouldReturnCardIdsAndPrincipleIdsInPriorityOrder()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
        page.AdvantagesSection!.AdvantageCards =
        [
            new HippotherapyLandingPageAdvantageCard { Id = 20, Priority = 2, Description = "b" },
            new HippotherapyLandingPageAdvantageCard { Id = 10, Priority = 1, Description = "a" },
        ];
        page.EthicsSection!.EthicsPrinciples =
        [
            new HippotherapyLandingPageEthicsPrinciple { Id = 8, Priority = 2, Text = "p2" },
            new HippotherapyLandingPageEthicsPrinciple { Id = 7, Priority = 1, Text = "p1" },
        ];

        var dto = _mapper.Map<HippotherapyLandingPageDto>(page);

        Assert.Equal([20L, 10L], dto.AdvantagesSection.Cards.Select(c => c.Id).ToArray());
        Assert.Equal([7L, 8L], dto.EthicsSection.Principles.Select(p => p.Id).ToArray());
        Assert.Equal(["p1", "p2"], dto.EthicsSection.Principles.Select(p => p.Text).ToArray());
    }
}
