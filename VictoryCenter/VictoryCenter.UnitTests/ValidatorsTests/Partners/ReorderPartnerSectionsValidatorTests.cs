using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Partners.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class ReorderPartnerSectionsValidatorTests
{
    private readonly ReorderPartnerSectionsValidator _validator;

    public ReorderPartnerSectionsValidatorTests()
    {
        _validator = new ReorderPartnerSectionsValidator();
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
    public void Validate_OrderedIdsCountIsTooLarge_ShouldHaveError()
    {
        // Arrange
        var tooManyIds = Enumerable.Range(1, PartnerConstants.PartnersMaxCount + 1)
                                   .Select(i => (long)i)
                                   .ToList();
        var command = new ReorderPartnersSectionsCommand(new ReorderPartnersSectionsDto { OrderedIds = tooManyIds });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReorderDto.OrderedIds);
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
