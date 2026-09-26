using FluentValidation;
using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.UnitTests.HelperTests;
public class ReportRecordsValidationExtensionsTests
{
    private const decimal MinAmount = 0m;
    private const int AmountPrecision = 18;
    private const int AmountScale = 2;
    private const string AmountFormat = "18 digits, 2 decimals";
    private const int MinYear = 2020;
    private const int MaxYear = 2030;
    private const int MaxBulkDeleteCount = 3;

    private readonly DummyValidator _validator;

    public ReportRecordsValidationExtensionsTests()
    {
        _validator = new DummyValidator();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Validate_ShouldHaveError_WhenAmountIsNotGreaterThanOrEqualToMinValue(decimal amount)
    {
        // Arrange
        var model = GetValidModel();
        model.Amount = amount;

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage(ErrorMessagesConstants.SumMustNotBeNegative(nameof(DummyModel.Amount)));
    }

    [Theory]
    [InlineData(10.123)]
    public void Validate_ShouldHaveError_WhenAmountHasInvalidFormat(decimal amount)
    {
        // Arrange
        var model = GetValidModel();
        model.Amount = amount;

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(DummyModel.Amount),
                AmountFormat));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenIdIsNotPositive(long id)
    {
        // Arrange
        var model = GetValidModel();
        model.Id = id;

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(DummyModel.Id)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReportingYearIsLessThanMin()
    {
        // Arrange
        var model = GetValidModel();
        model.Year = MinYear - 1;

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Year)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(DummyModel.Year),
                MinYear));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReportingYearIsGreaterThanMax()
    {
        // Arrange
        var model = GetValidModel();
        model.Year = MaxYear + 1;

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Year)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                nameof(DummyModel.Year),
                MaxYear));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIdsAreNotUnique()
    {
        // Arrange
        var model = GetValidModel();
        model.UniqueIds = [1, 2, 2];

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UniqueIds)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(DummyModel.UniqueIds)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenBulkDeleteIdsIsEmpty()
    {
        // Arrange
        var model = GetValidModel();
        model.BulkDeleteIds = [];

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BulkDeleteIds)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(DummyModel.BulkDeleteIds)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenBulkDeleteIdsExceedsMaxCount()
    {
        // Arrange
        var model = GetValidModel();
        model.BulkDeleteIds = [1, 2, 3, 4];

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BulkDeleteIds)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(DummyModel.BulkDeleteIds),
                MaxBulkDeleteCount));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenBulkDeleteIdsAreNotUnique()
    {
        // Arrange
        var model = GetValidModel();
        model.BulkDeleteIds = [1, 2, 2];

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BulkDeleteIds)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(DummyModel.BulkDeleteIds)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_ShouldHaveError_WhenBulkDeleteIdsContainNonPositiveId(long invalidId)
    {
        // Arrange
        var model = GetValidModel();
        model.BulkDeleteIds = [1, invalidId];

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BulkDeleteIds)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(DummyModel.Id)));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllDataIsValid()
    {
        // Arrange
        var model = GetValidModel();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static DummyModel GetValidModel() => new()
    {
        Amount = 100.50m,
        Id = 1,
        Year = MinYear,
        UniqueIds = [1, 2, 3],
        BulkDeleteIds = [10, 20]
    };

    private class DummyModel
    {
        public decimal? Amount { get; set; }
        public long Id { get; set; }
        public int Year { get; set; }
        public IEnumerable<long> UniqueIds { get; set; } = [];
        public IEnumerable<long> BulkDeleteIds { get; set; } = [];
    }

    private class DummyValidator : AbstractValidator<DummyModel>
    {
        public DummyValidator()
        {
            RuleFor(x => x.Amount).MustBeValidAmountOfMoney(
                nameof(DummyModel.Amount),
                MinAmount,
                AmountPrecision,
                AmountScale,
                AmountFormat);

            RuleFor(x => x.Id).MustBeValidId(nameof(DummyModel.Id));

            RuleFor(x => x.Year).MustBeValidReportingYear(
                nameof(DummyModel.Year),
                MinYear,
                MaxYear);

            RuleFor(x => x.UniqueIds).MustHaveUniqueIds(nameof(DummyModel.UniqueIds));

            RuleFor(x => x.BulkDeleteIds).MustBeValidBulkDeleteIds(
                nameof(DummyModel.BulkDeleteIds),
                nameof(DummyModel.Id),
                MaxBulkDeleteCount);
        }
    }
}
