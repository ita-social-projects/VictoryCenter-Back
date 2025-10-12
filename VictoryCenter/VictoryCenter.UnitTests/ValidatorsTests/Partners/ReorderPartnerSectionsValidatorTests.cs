using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Partners.ReorderSections;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners.Commands;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class ReorderPartnerSectionsValidatorTests
{
    private readonly ReorderPartnersSectionsCommandValidator _validator;

    public ReorderPartnerSectionsValidatorTests()
    {
        _validator = new ReorderPartnersSectionsCommandValidator();
    }

    [Fact]
    public void Validate_OrderedIdsIsEmpty_ShouldHaveError()
    {
        // Arrange
        var command = new ReorderPartnersSectionsCommand(new ReorderPartnersSectionsDto { OrderedIds = new List<long>() });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReorderDto.OrderedIds)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(ReorderPartnersSectionsDto.OrderedIds)));
    }

    [Fact]
    public void Validate_OrderedIdsContainDuplicates_ShouldHaveError()
    {
        // Arrange
        var command = new ReorderPartnersSectionsCommand(new ReorderPartnersSectionsDto { OrderedIds = new List<long> { 1, 2, 1 } });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReorderDto.OrderedIds)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(ReorderPartnersSectionsDto.OrderedIds)));
    }

    [Fact]
    public void Validate_OrderedIdsContainNonPositiveValue_ShouldHaveError()
    {
        // Arrange
        var command = new ReorderPartnersSectionsCommand(new ReorderPartnersSectionsDto { OrderedIds = new List<long> { 1, 0, 2 } });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        // RuleForEach генерує помилку для всієї колекції
        result.ShouldHaveValidationErrorFor(x => x.ReorderDto.OrderedIds);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new ReorderPartnersSectionsCommand(new ReorderPartnersSectionsDto { OrderedIds = new List<long> { 3, 1, 2 } });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
