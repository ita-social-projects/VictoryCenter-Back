using VictoryCenter.BLL.Helpers;
using VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyLandingPage;
using VictoryCenter.UnitTests.Utils.HippotherapyLandingPageTestDataHelper;

namespace VictoryCenter.UnitTests.HelperTests;

public class HippotherapyLandingPageIncludeHelperTests
{
    [Fact]
    public void HasAllSections_WhenAllSectionsPresent_ShouldReturnTrue()
    {
        var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();

        Assert.True(page.HasAllSections());
    }

    [Fact]
    public void HasAllSections_WhenSectionIsMissing_ShouldReturnFalseForEachSection()
    {
        var removers = GetHippotherapyLandingPageHandlerTests.SectionRemoverActions().ToList();

        Assert.Equal(11, removers.Count);
        foreach (var remove in removers)
        {
            var page = HippotherapyLandingPageTestDataHelper.CreateLandingPageEntity();
            remove(page);

            Assert.False(page.HasAllSections());
        }
    }
}
