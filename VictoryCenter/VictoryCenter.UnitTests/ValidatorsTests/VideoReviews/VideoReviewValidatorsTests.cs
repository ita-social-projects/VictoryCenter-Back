using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.VideoReviews.Create;
using VictoryCenter.BLL.Commands.Admin.VideoReviews.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.BLL.Validators.VideoReviews;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.VideoReviews;

public class VideoReviewValidatorsTests
{
    private readonly CreateVideoReviewValidator _createValidator = new(new BaseVideoReviewDtoValidator());
    private readonly UpdateVideoReviewValidator _updateValidator = new(new BaseVideoReviewDtoValidator());

    [Fact]
    public void Create_ShouldNotHaveErrors_WhenDtoIsValid()
    {
        var command = new CreateVideoReviewCommand(ValidDto());

        var result = _createValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldHaveError_WhenTitleIsEmptyOrWhitespaceOnly(string? title)
    {
        var command = new CreateVideoReviewCommand(ValidDto() with { Title = title! });

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.VideoReview.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldHaveError_WhenLinkIsEmptyOrWhitespaceOnly(string? link)
    {
        var command = new CreateVideoReviewCommand(ValidDto() with { Link = link! });

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.VideoReview.Link);
    }

    [Fact]
    public void Create_ShouldHaveError_WhenTitleIsShorterThanMinLengthAfterTrimming()
    {
        var command = new CreateVideoReviewCommand(
            ValidDto() with { Title = "  " + new string('a', VideoReviewConstants.TitleMinLength - 1) + "  " });

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.VideoReview.Title);
    }

    [Fact]
    public void Create_ShouldNotHaveError_WhenTitleIsExactlyAtMinLengthAfterTrimming()
    {
        var command = new CreateVideoReviewCommand(
            ValidDto() with { Title = "  " + new string('a', VideoReviewConstants.TitleMinLength) + "  " });

        var result = _createValidator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(item => item.VideoReview.Title);
    }

    [Fact]
    public void Create_ShouldHaveError_WhenTitleExceedsMaxLength()
    {
        var command = new CreateVideoReviewCommand(
            ValidDto() with { Title = new string('a', VideoReviewConstants.TitleMaxLength + 1) });

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.VideoReview.Title);
    }

    [Fact]
    public void Create_ShouldNotHaveError_WhenTitleIsExactlyAtMaxLength()
    {
        var command = new CreateVideoReviewCommand(
            ValidDto() with { Title = new string('a', VideoReviewConstants.TitleMaxLength) });

        var result = _createValidator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(item => item.VideoReview.Title);
    }

    [Fact]
    public void Create_ShouldHaveError_WhenLinkIsShorterThanMinLengthAfterTrimming()
    {
        var shortLink = new string('a', VideoReviewConstants.LinkMinLength - 1);
        var command = new CreateVideoReviewCommand(ValidDto() with { Link = "  " + shortLink + "  " });

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.VideoReview.Link);
    }

    [Theory]
    [InlineData("not a valid link at all")]
    [InlineData("/video/12345")]
    [InlineData("www.example.com/video")]
    public void Create_ShouldHaveError_WhenLinkIsNotAnAbsoluteUri(string link)
    {
        Assert.True(link.Length >= VideoReviewConstants.LinkMinLength);
        var command = new CreateVideoReviewCommand(ValidDto() with { Link = link });

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.VideoReview.Link)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(CreateVideoReviewDto.Link)));
    }

    [Theory]
    [InlineData("javascript:alert(document.cookie)")]
    [InlineData("data:text/html;base64,PHNjcmlwdD4=")]
    [InlineData("file:///etc/passwd")]
    public void Create_ShouldHaveError_WhenLinkUsesADisallowedScheme(string link)
    {
        var command = new CreateVideoReviewCommand(ValidDto() with { Link = link });

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.VideoReview.Link)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(CreateVideoReviewDto.Link)));
    }

    [Theory]
    [InlineData("https://example.com/video")]
    [InlineData("http://example.com/video?query=1")]
    public void Create_ShouldNotHaveError_WhenLinkIsAValidHttpOrHttpsUri(string link)
    {
        var command = new CreateVideoReviewCommand(ValidDto() with { Link = link });

        var result = _createValidator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(item => item.VideoReview.Link);
    }

    [Fact]
    public void Create_ShouldHaveError_WhenLinkExceedsMaxLength()
    {
        var overlyLongLink = "https://example.com/" + new string('a', VideoReviewConstants.LinkMaxLength);
        var command = new CreateVideoReviewCommand(ValidDto() with { Link = overlyLongLink });

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.VideoReview.Link);
    }

    [Fact]
    public void Update_ShouldHaveError_WhenIdIsNotPositive()
    {
        var command = new UpdateVideoReviewCommand(0, new UpdateVideoReviewDto
        {
            Title = ValidDto().Title,
            Link = ValidDto().Link
        });

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.Id);
    }

    [Fact]
    public void Update_ShouldHaveError_WhenLinkIsMissing()
    {
        var command = new UpdateVideoReviewCommand(1, new UpdateVideoReviewDto
        {
            Title = ValidDto().Title,
            Link = string.Empty
        });

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.VideoReview.Link);
    }

    [Fact]
    public void Update_ShouldNotHaveErrors_WhenDtoIsValid()
    {
        var command = new UpdateVideoReviewCommand(1, new UpdateVideoReviewDto
        {
            Title = ValidDto().Title,
            Link = ValidDto().Link
        });

        var result = _updateValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(Status.Draft)]
    [InlineData(Status.Published)]
    public void Create_ShouldNotHaveError_WhenStatusIsValid(Status status)
    {
        var command = new CreateVideoReviewCommand(ValidDto() with { Status = status });

        var result = _createValidator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(item => item.VideoReview.Status);
    }

    [Fact]
    public void Create_ShouldHaveError_WhenStatusIsNotAValidEnumValue()
    {
        var command = new CreateVideoReviewCommand(ValidDto() with { Status = (Status)999 });

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.VideoReview.Status)
            .WithErrorMessage(ErrorMessagesConstants.UnknownStatusValue);
    }

    private static CreateVideoReviewDto ValidDto() => new()
    {
        Title = "Valid title",
        Link = "https://example.com/video",
        Status = Status.Draft
    };
}
