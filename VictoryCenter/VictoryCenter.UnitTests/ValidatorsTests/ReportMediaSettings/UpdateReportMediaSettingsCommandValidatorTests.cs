using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.ReportMediaSettings.UpdateReportMediaSettings;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;
using VictoryCenter.BLL.Validators.ReportMediaSettings.Commands;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportMediaSettings;

public class UpdateReportMediaSettingsCommandValidatorTests
{
    private readonly UpdateReportMediaSettingsCommandValidator _validator;

    public UpdateReportMediaSettingsCommandValidatorTests()
    {
        _validator = new UpdateReportMediaSettingsCommandValidator();
    }

    [Fact]
    public void Validate_ChangedLivesBlockTitleIsEmpty_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: string.Empty,
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.Title);
    }

    [Fact]
    public void Validate_ChangedLivesBlockTitleIsNull_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: null!,
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.Title);
    }

    [Fact]
    public void Validate_ChangedLivesBlockTitleTooShort_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Short",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.Title);
    }

    [Fact]
    public void Validate_ChangedLivesBlockTitleTooLong_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: new string('a', ChangedLivesBlockConstants.TitleMaxLength + 1),
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.Title);
    }

    [Fact]
    public void Validate_ChangedLivesBlockTitleAtMinLength_ShouldNotHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: new string('a', ChangedLivesBlockConstants.TitleMinLength),
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.Title);
    }

    [Fact]
    public void Validate_ChangedLivesBlockTitleAtMaxLength_ShouldNotHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: new string('a', ChangedLivesBlockConstants.TitleMaxLength),
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.Title);
    }

    [Fact]
    public void Validate_ChangedLivesBlockTitleValid_ShouldHaveValidationError()
    {
        var command = CreateNotValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.Title);
    }

    [Fact]
    public void Validate_ChangedLivesIsNegative_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: -1,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.ChangedLives);
    }

    [Fact]
    public void Validate_ChangedLivesIsZero_ShouldNotHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 0,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.ChangedLives);
    }

    [Fact]
    public void Validate_ChangedLivesIsValid_ShouldNotHaveValidationError()
    {
        var command = CreateNotValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.ChangedLives);
    }

    [Fact]
    public void Validate_ChangedLivesBlockImageIdIsZero_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: 0,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.ImageId);
    }

    [Fact]
    public void Validate_ChangedLivesBlockImageIdIsNegative_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: -1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.ImageId);
    }

    [Fact]
    public void Validate_ChangedLivesBlockImageIdIsPositive_ShouldNotHaveValidationError()
    {
        var command = CreateNotValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.ImageId);
    }

    [Fact]
    public void Validate_CollectedFundsBlockTitleIsEmpty_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: string.Empty,
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.Title);
    }

    [Fact]
    public void Validate_CollectedFundsBlockTitleIsNull_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: null!,
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.Title);
    }

    [Fact]
    public void Validate_CollectedFundsBlockTitleTooShort_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Short",
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.Title);
    }

    [Fact]
    public void Validate_CollectedFundsBlockTitleTooLong_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: new string('a', CollectedFundsBlockConstants.TitleMaxLength + 1),
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.Title);
    }

    [Fact]
    public void Validate_CollectedFundsBlockTitleAtMinLength_ShouldNotHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: new string('a', CollectedFundsBlockConstants.TitleMinLength),
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.Title);
    }

    [Fact]
    public void Validate_CollectedFundsBlockTitleAtMaxLength_ShouldNotHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: new string('a', CollectedFundsBlockConstants.TitleMaxLength),
            collectedFunds: 500000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.Title);
    }

    [Fact]
    public void Validate_CollectedFundsBlockTitleValid_ShouldNotHaveValidationError()
    {
        var command = CreateNotValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.Title);
    }

    [Fact]
    public void Validate_CollectedFundsIsNegative_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: -1,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.CollectedFunds);
    }

    [Fact]
    public void Validate_CollectedFundsExceeds15Digits_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 1_000_000_000_000_000,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.CollectedFunds);
    }

    [Fact]
    public void Validate_CollectedFundsIs15Digits_ShouldNotHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 111111111111111,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.CollectedFunds);
    }

    [Fact]
    public void Validate_CollectedFundsIsZero_ShouldNotHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 0,
            collectedFundsImageId: 2);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.CollectedFunds);
    }

    [Fact]
    public void Validate_CollectedFundsIsValid_ShouldNotHaveValidationError()
    {
        var command = CreateNotValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.CollectedFunds);
    }

    [Fact]
    public void Validate_CollectedFundsBlockImageIdIsZero_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 500000,
            collectedFundsImageId: 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.ImageId);
    }

    [Fact]
    public void Validate_CollectedFundsBlockImageIdIsNegative_ShouldHaveValidationError()
    {
        var command = CreateCommand(
            changedLivesTitle: "Valid Title",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title",
            collectedFunds: 500000,
            collectedFundsImageId: -1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.ImageId);
    }

    [Fact]
    public void Validate_CollectedFundsBlockImageIdIsPositive_ShouldNotHaveValidationError()
    {
        var command = CreateNotValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.ImageId);
    }

    [Fact]
    public void Validate_MultiplePropertiesInvalid_ShouldHaveMultipleValidationErrors()
    {
        var command = CreateCommand(
            changedLivesTitle: string.Empty,
            changedLives: -1,
            changedLivesImageId: -1,
            collectedFundsTitle: null!,
            collectedFunds: -1,
            collectedFundsImageId: 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.Title);
        result.ShouldHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.ChangedLives);
        result.ShouldHaveValidationErrorFor(x => x.Dto.ChangedLivesBlock.ImageId);
        result.ShouldHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.Title);
        result.ShouldHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.CollectedFunds);
        result.ShouldHaveValidationErrorFor(x => x.Dto.CollectedFundsBlock.ImageId);
    }

    private UpdateReportMediaSettingsCommand CreateNotValidCommand()
    {
        return CreateCommand(
            changedLivesTitle: "Valid Title for Changed Lives",
            changedLives: 1000,
            changedLivesImageId: 1,
            collectedFundsTitle: "Valid Title for Collected Funds",
            collectedFunds: 500000,
            collectedFundsImageId: 2);
    }

    private UpdateReportMediaSettingsCommand CreateCommand(
        string changedLivesTitle,
        int changedLives,
        long changedLivesImageId,
        string collectedFundsTitle,
        long collectedFunds,
        long collectedFundsImageId)
    {
        var dto = new UpdateReportMediaSettingsDto
        {
            ChangedLivesBlock = new UpdateChangedLivesBlockDto
            {
                Title = changedLivesTitle,
                ChangedLives = changedLives,
                ImageId = changedLivesImageId
            },
            CollectedFundsBlock = new UpdateCollectedFundsBlockDto
            {
                Title = collectedFundsTitle,
                CollectedFunds = collectedFunds,
                ImageId = collectedFundsImageId
            }
        };

        return new UpdateReportMediaSettingsCommand(dto);
    }
}
