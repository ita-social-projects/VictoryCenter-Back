using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.EventNews.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Validators.EventNews;

namespace VictoryCenter.UnitTests.ValidatorsTests.EventNews;

public class ReorderEventNewsValidatorTests
{
    private readonly ReorderEventNewsValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = CreateCommand(
            categoryId: 1,
            ids: [1, 2, 3]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_CategoryIdIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var command = CreateCommand(
            categoryId: EventNewsConstants.ZeroCategoryId,
            ids: [1]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Dto.CategoryId)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustBePositive(
                    nameof(ReorderEventNewsDto.CategoryId)));
    }

    [Fact]
    public void Validate_CategoryIdIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var command = CreateCommand(
            categoryId: -1,
            ids: [1]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Dto.CategoryId)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustBePositive(
                    nameof(ReorderEventNewsDto.CategoryId)));
    }

    [Fact]
    public void Validate_IdsIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var command = CreateCommand(
            categoryId: 1,
            ids: null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Dto.Ids)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyIsRequired(
                    nameof(ReorderEventNewsDto.Ids)));
    }

    [Fact]
    public void Validate_IdsIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = CreateCommand(
            categoryId: 1,
            ids: []);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Dto.Ids)
            .WithErrorMessage(
                ErrorMessagesConstants.CollectionCannotBeEmpty(
                    nameof(ReorderEventNewsDto.Ids)));
    }

    [Fact]
    public void Validate_IdsExceedMaxCount_ShouldHaveValidationError()
    {
        // Arrange
        var ids = Enumerable
            .Range(1, ReorderConstants.MaxElementsSwapCount + 1)
            .Select(id => (long)id)
            .ToList();

        var command = CreateCommand(
            categoryId: 1,
            ids: ids);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Dto.Ids)
            .WithErrorMessage(
                ReorderConstants.ExceededMaxElementsSwapCount(ids.Count));
    }

    [Fact]
    public void Validate_IdsCountEqualsMaxCount_ShouldNotHaveValidationError()
    {
        // Arrange
        var ids = Enumerable
            .Range(1, ReorderConstants.MaxElementsSwapCount)
            .Select(id => (long)id)
            .ToList();

        var command = CreateCommand(
            categoryId: 1,
            ids: ids);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Dto.Ids);
    }

    [Fact]
    public void Validate_IdsContainDuplicates_ShouldHaveValidationError()
    {
        // Arrange
        var command = CreateCommand(
            categoryId: 1,
            ids: [1, 2, 2]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Dto.Ids)
            .WithErrorMessage(
                ErrorMessagesConstants.CollectionMustContainUniqueValues(
                    nameof(ReorderEventNewsDto.Ids)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_IdIsNotPositive_ShouldHaveValidationError(long invalidId)
    {
        // Arrange
        var command = CreateCommand(
            categoryId: 1,
            ids: [1, invalidId, 2]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Dto.Ids[1]")
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustBePositive(
                    $"Each {nameof(ReorderEventNewsDto.Ids)} element."));
    }

    private static ReorderEventNewsCommand CreateCommand(
        long categoryId,
        List<long> ids)
    {
        return new ReorderEventNewsCommand(
            new ReorderEventNewsDto
            {
                CategoryId = categoryId,
                Ids = ids
            });
    }
}
