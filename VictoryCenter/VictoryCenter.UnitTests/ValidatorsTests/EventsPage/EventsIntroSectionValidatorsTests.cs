using FluentValidation.TestHelper;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;
using VictoryCenter.BLL.Validators.EventsPage;

namespace VictoryCenter.UnitTests.ValidatorsTests.EventsPage;

public class EventsIntroSectionValidatorsTests
{
    [Theory]
    [InlineData("   ")]
    [InlineData("<p><br></p>")]
    public void Description_WhenHtmlHasNoVisibleText_HasValidationError(string value)
    {
        var result = new UpdateEventsPageDescriptionDtoValidator().TestValidate(new UpdateEventsPageDescriptionDto
        {
            PageDescription = value,
        });

        result.ShouldHaveValidationErrorFor(x => x.PageDescription);
    }

    [Fact]
    public void Description_WhenVisibleTextExceedsLimit_HasValidationError()
    {
        var result = new UpdateEventsPageDescriptionDtoValidator().TestValidate(new UpdateEventsPageDescriptionDto
        {
            PageDescription = $"<p>{new string('A', 1001)}</p>",
        });

        result.ShouldHaveValidationErrorFor(x => x.PageDescription);
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("<p><br></p>")]
    public void Title_WhenHtmlHasNoVisibleText_HasValidationError(string value)
    {
        var result = new UpdateEventsBlockTitleDtoValidator().TestValidate(new UpdateEventsBlockTitleDto
        {
            EventsBlockTitle = value,
        });

        result.ShouldHaveValidationErrorFor(x => x.EventsBlockTitle);
    }

    [Fact]
    public void Title_WhenVisibleTextIsWithinLimit_HasNoValidationError()
    {
        var result = new UpdateEventsBlockTitleDtoValidator().TestValidate(new UpdateEventsBlockTitleDto
        {
            EventsBlockTitle = $"<p>{new string('A', 100)}</p>",
        });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
