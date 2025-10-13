using FluentValidation.TestHelper;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Validators.Common;

namespace VictoryCenter.UnitTests.ValidatorsTests.Common;

public class BaseReorderValidatorTests
{
    [Fact]
    public void Validate_OrderedIdsIsNull_ShouldHaveError()
    {
        // Arrange
        var validator = new TestReorderValidator();
        var model = new TestReorderDto { OrderedIds = null! };

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OrderedIds);
    }

    [Fact]
    public void Validate_OrderedIdsIsEmpty_ShouldHaveError()
    {
        // Arrange
        var validator = new TestReorderValidator();
        var model = new TestReorderDto { OrderedIds = new List<long>() };

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OrderedIds);
    }

    [Fact]
    public void Validate_OrderedIdsContainDuplicates_ShouldHaveError()
    {
        // Arrange
        var validator = new TestReorderValidator();
        var model = new TestReorderDto { OrderedIds = new List<long> { 1, 2, 1 } };

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OrderedIds);
    }

    [Fact]
    public void Validate_OrderedIdsContainNonPositiveValue_ShouldHaveError()
    {
        // Arrange
        var validator = new TestReorderValidator();
        var model = new TestReorderDto { OrderedIds = new List<long> { 1, 0, 2 } };

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor($"{nameof(BaseReorderDto.OrderedIds)}[1]");
    }

    [Fact]
    public void Validate_MaxCountIsSetAndExceeded_ShouldHaveError()
    {
        // Arrange
        const int maxCount = 2;
        var validator = new TestReorderValidator(maxCount);
        var model = new TestReorderDto { OrderedIds = new List<long> { 1, 2, 3 } };

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OrderedIds);
    }

    [Fact]
    public void Validate_MaxCountIsSetAndNotExceeded_ShouldNotHaveError()
    {
        // Arrange
        const int maxCount = 3;
        var validator = new TestReorderValidator(maxCount);
        var model = new TestReorderDto { OrderedIds = new List<long> { 1, 2, 3 } };

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.OrderedIds);
    }

    [Fact]
    public void Validate_ValidModel_ShouldNotHaveAnyErrors()
    {
        // Arrange
        var validator = new TestReorderValidator();
        var model = new TestReorderDto { OrderedIds = new List<long> { 3, 1, 2 } };

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private record TestReorderDto : BaseReorderDto
    {
    }

    private class TestReorderValidator : BaseReorderValidator<TestReorderDto>
    {
        public TestReorderValidator(int? maxCount = null)
            : base(maxCount)
        {
        }
    }
}
