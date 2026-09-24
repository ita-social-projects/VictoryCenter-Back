using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;
using VictoryCenter.BLL.Validators.EventsPage;

namespace VictoryCenter.UnitTests.ValidatorsTests.EventsPage;

public class EventsIntroSectionValidatorsTests
{
    [Theory]
    [InlineData("   ")]
    [InlineData("<p><br></p>")]
    public void Description_WhenHtmlHasNoVisibleText_HasRequiredValidationError(string value)
    {
        var result = new UpdateEventsPageDescriptionDtoValidator().TestValidate(new UpdateEventsPageDescriptionDto
        {
            PageDescription = value,
        });

        result.ShouldHaveValidationErrorFor(x => x.PageDescription)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEventsPageDescriptionDto.PageDescription)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public void Description_WhenVisibleTextIsShorterThanMinimum_HasValidationError(int length)
    {
        var result = new UpdateEventsPageDescriptionDtoValidator().TestValidate(new UpdateEventsPageDescriptionDto
        {
            PageDescription = $"<p>{new string('A', length)}</p>",
        });

        result.ShouldHaveValidationErrorFor(x => x.PageDescription)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateEventsPageDescriptionDto.PageDescription), EventsPageConstants.PageDescriptionMinLength));
    }

    [Fact]
    public void Description_WhenTrailingWhitespaceMakesVisibleTextShorterThanMinimum_HasValidationError()
    {
        var result = new UpdateEventsPageDescriptionDtoValidator().TestValidate(new UpdateEventsPageDescriptionDto
        {
            PageDescription = $"{new string('A', EventsPageConstants.PageDescriptionMinLength - 1)}   ",
        });

        result.ShouldHaveValidationErrorFor(x => x.PageDescription)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateEventsPageDescriptionDto.PageDescription), EventsPageConstants.PageDescriptionMinLength));
    }

    [Theory]
    [InlineData(10)]
    [InlineData(1000)]
    public void Description_WhenVisibleTextIsAtBoundary_HasNoValidationError(int length)
    {
        var result = new UpdateEventsPageDescriptionDtoValidator().TestValidate(new UpdateEventsPageDescriptionDto
        {
            PageDescription = $"<p>{new string('A', length)}</p>",
        });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Description_WhenVisibleTextExceedsMaximum_HasValidationError()
    {
        var result = new UpdateEventsPageDescriptionDtoValidator().TestValidate(new UpdateEventsPageDescriptionDto
        {
            PageDescription = $"<p>{new string('A', EventsPageConstants.PageDescriptionMaxLength + 1)}</p>",
        });

        result.ShouldHaveValidationErrorFor(x => x.PageDescription)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateEventsPageDescriptionDto.PageDescription), EventsPageConstants.PageDescriptionMaxLength));
    }

    [Fact]
    public void Description_WhenMarkupDoesNotAffectVisibleTextLength_HasNoValidationError()
    {
        var result = new UpdateEventsPageDescriptionDtoValidator().TestValidate(new UpdateEventsPageDescriptionDto
        {
            PageDescription = $"<p>{new string('A', EventsPageConstants.PageDescriptionMaxLength)}</p><strong></strong>",
        });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("<p><br></p>")]
    public void Title_WhenHtmlHasNoVisibleText_HasRequiredValidationError(string value)
    {
        var result = new UpdateEventsBlockTitleDtoValidator().TestValidate(new UpdateEventsBlockTitleDto
        {
            EventsBlockTitle = value,
        });

        result.ShouldHaveValidationErrorFor(x => x.EventsBlockTitle)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEventsBlockTitleDto.EventsBlockTitle)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public void Title_WhenVisibleTextIsShorterThanMinimum_HasValidationError(int length)
    {
        var result = new UpdateEventsBlockTitleDtoValidator().TestValidate(new UpdateEventsBlockTitleDto
        {
            EventsBlockTitle = $"<p>{new string('A', length)}</p>",
        });

        result.ShouldHaveValidationErrorFor(x => x.EventsBlockTitle)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateEventsBlockTitleDto.EventsBlockTitle), EventsPageConstants.EventsBlockTitleMinLength));
    }

    [Fact]
    public void Title_WhenTrailingWhitespaceMakesVisibleTextShorterThanMinimum_HasValidationError()
    {
        var result = new UpdateEventsBlockTitleDtoValidator().TestValidate(new UpdateEventsBlockTitleDto
        {
            EventsBlockTitle = $"{new string('A', EventsPageConstants.EventsBlockTitleMinLength - 1)}   ",
        });

        result.ShouldHaveValidationErrorFor(x => x.EventsBlockTitle)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateEventsBlockTitleDto.EventsBlockTitle), EventsPageConstants.EventsBlockTitleMinLength));
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    public void Title_WhenVisibleTextIsAtBoundary_HasNoValidationError(int length)
    {
        var result = new UpdateEventsBlockTitleDtoValidator().TestValidate(new UpdateEventsBlockTitleDto
        {
            EventsBlockTitle = $"<p>{new string('A', length)}</p>",
        });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Title_WhenVisibleTextExceedsMaximum_HasValidationError()
    {
        var result = new UpdateEventsBlockTitleDtoValidator().TestValidate(new UpdateEventsBlockTitleDto
        {
            EventsBlockTitle = $"<p>{new string('A', EventsPageConstants.EventsBlockTitleMaxLength + 1)}</p>",
        });

        result.ShouldHaveValidationErrorFor(x => x.EventsBlockTitle)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateEventsBlockTitleDto.EventsBlockTitle), EventsPageConstants.EventsBlockTitleMaxLength));
    }

    [Fact]
    public void Title_WhenMarkupDoesNotAffectVisibleTextLength_HasNoValidationError()
    {
        var result = new UpdateEventsBlockTitleDtoValidator().TestValidate(new UpdateEventsBlockTitleDto
        {
            EventsBlockTitle = $"<p>{new string('A', EventsPageConstants.EventsBlockTitleMaxLength)}</p><strong></strong>",
        });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
