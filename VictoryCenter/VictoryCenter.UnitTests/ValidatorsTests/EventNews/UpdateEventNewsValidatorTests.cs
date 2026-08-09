using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.EventNews.Update;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Validators.EventNews;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.EventNews;

public class UpdateEventNewsValidatorTests
{
    private readonly UpdateEventNewsValidator _validator = new(new BaseEventNewsValidator());

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenIdIsNotPositive(long id)
    {
        var command = new UpdateEventNewsCommand(id, new UpdateEventNewsDto { Status = Status.Draft });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.Id);
    }

    [Fact]
    public void Validate_ShouldHaveErrors_WhenPublishedFieldsAreMissing()
    {
        var command = new UpdateEventNewsCommand(1, new UpdateEventNewsDto { Status = Status.Published });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.EventNews.PublishedAt);
        result.ShouldHaveValidationErrorFor(item => item.EventNews.PreviewImageId);
        result.ShouldHaveValidationErrorFor(item => item.EventNews.CategoryIds);
        result.ShouldHaveValidationErrorFor(item => item.EventNews.Localizations);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDraftIsEmpty()
    {
        var command = new UpdateEventNewsCommand(1, new UpdateEventNewsDto { Status = Status.Draft });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
