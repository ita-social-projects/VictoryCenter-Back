using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.BLL.Validators.HistorySections;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.HistorySections;

public class UpdateHistorySectionValidatorTests
{
    private readonly UpdateHistorySectionValidator _validator = new();

    [Fact]
    public void Validate_InvalidTemplate_ShouldHaveError()
    {
        var model = ValidSection(HistorySectionTemplate.TextOnly) with
        {
            Template = (HistorySectionTemplate)999
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Template)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(UpdateHistorySectionDto.Template)));
    }

    [Fact]
    public void Validate_NegativeOrder_ShouldHaveError()
    {
        var model = ValidSection(HistorySectionTemplate.TextOnly) with
        {
            Order = -1
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Order)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan(nameof(UpdateHistorySectionDto.Order), -1));
    }

    [Fact]
    public void Validate_NullContents_ShouldHaveError()
    {
        var model = ValidSection(HistorySectionTemplate.TextOnly) with
        {
            Contents = null
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHistorySectionDto.Contents)));
    }

    [Fact]
    public void Validate_DuplicateOrders_ShouldHaveError()
    {
        var model = ValidSection(HistorySectionTemplate.TextOnly) with
        {
            Contents =
            [
                Title(order: 0, title: "Valid title"),
                Description(order: 0, description: "Valid description")
            ]
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateHistorySectionContentDto.Order)));
    }

    [Fact]
    public void Validate_DuplicateImageIds_ShouldHaveError()
    {
        var model = ValidSection(HistorySectionTemplate.DualImagesBottom) with
        {
            Contents =
            [
                Title(order: 0, title: "Valid title"),
                Description(order: 1, description: "Valid description"),
                Image(order: 2, imageId: 7),
                Image(order: 3, imageId: 7)
            ]
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateHistorySectionContentDto.ImageId)));
    }

    [Fact]
    public void Validate_TitleIsEmpty_ShouldHaveError()
    {
        var model = ValidSection(HistorySectionTemplate.TextOnly) with
        {
            Contents =
            [
                Title(order: 0, title: " "),
                Description(order: 1, description: "Valid description")
            ]
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateHistorySectionContentDto.Title)));
    }

    [Fact]
    public void Validate_TitleLengthTooShort_ShouldHaveError()
    {
        var model = ValidSection(HistorySectionTemplate.TextOnly) with
        {
            Contents =
            [
                Title(order: 0, title: "abcd"),
                Description(order: 1, description: "Valid description")
            ]
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HistorySectionConstants.GetTitleLengthErrorMessage(model));
    }

    [Fact]
    public void Validate_DescriptionIsEmpty_ShouldHaveError()
    {
        var model = ValidSection(HistorySectionTemplate.TextOnly) with
        {
            Contents =
            [
                Title(order: 0, title: "Valid title"),
                Description(order: 1, description: " ")
            ]
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateHistorySectionContentDto.Description)));
    }

    [Fact]
    public void Validate_DescriptionLengthTooShort_ShouldHaveError()
    {
        var model = ValidSection(HistorySectionTemplate.TextOnly) with
        {
            Contents =
            [
                Title(order: 0, title: "Valid title"),
                Description(order: 1, description: "short")
            ]
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HistorySectionConstants.GetDescriptionLengthErrorMessage(model));
    }

    [Fact]
    public void Validate_ImageIdIsNotPositive_ShouldHaveError()
    {
        var model = ValidSection(HistorySectionTemplate.SingleImageBottom) with
        {
            Contents =
            [
                Title(order: 0, title: "Valid title"),
                Description(order: 1, description: "Valid description"),
                Image(order: 2, imageId: 0)
            ]
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateHistorySectionContentDto.ImageId)));
    }

    [Fact]
    public void Validate_ValidModel_ShouldNotHaveErrors()
    {
        var model = ValidSection(HistorySectionTemplate.DualImagesBottom);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateHistorySectionDto ValidSection(HistorySectionTemplate template)
    {
        return template switch
        {
            HistorySectionTemplate.DualImagesBottom => new UpdateHistorySectionDto
            {
                Id = 1,
                Template = template,
                Order = 0,
                Contents =
                [
                    Title(order: 0, title: "Valid title"),
                    Description(order: 1, description: "Valid description"),
                    Image(order: 2, imageId: 1),
                    Image(order: 3, imageId: 2)
                ]
            },
            HistorySectionTemplate.SingleImageBottom => new UpdateHistorySectionDto
            {
                Id = 1,
                Template = template,
                Order = 0,
                Contents =
                [
                    Title(order: 0, title: "Valid title"),
                    Description(order: 1, description: "Valid description"),
                    Image(order: 2, imageId: 1)
                ]
            },
            _ => new UpdateHistorySectionDto
            {
                Id = 1,
                Template = HistorySectionTemplate.TextOnly,
                Order = 0,
                Contents =
                [
                    Title(order: 0, title: "Valid title"),
                    Description(order: 1, description: "Valid description")
                ]
            }
        };
    }

    private static CreateHistorySectionContentDto Title(int order, string title)
        => new()
        {
            ContentType = ContentType.Title,
            Order = order,
            Title = title
        };

    private static CreateHistorySectionContentDto Description(int order, string description)
        => new()
        {
            ContentType = ContentType.Description,
            Order = order,
            Description = description
        };

    private static CreateHistorySectionContentDto Image(int order, long imageId)
        => new()
        {
            ContentType = ContentType.Image,
            Order = order,
            ImageId = imageId
        };
}