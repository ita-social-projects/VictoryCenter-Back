using FluentValidation;
using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportProgramExpendituresRecords;

public class ReportProgramExpendituresRecordsValidationExtensionsTests
{
    private readonly DummyValidator _validator;

    public ReportProgramExpendituresRecordsValidationExtensionsTests()
    {
        _validator = new DummyValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void Validate_ShouldHaveError_WhenAmountIsNotPositive(decimal amount)
    {
        var model = new DummyModel { Amount = amount, Id = 1, Year = 2023 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(DummyModel.Amount)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountHasInvalidFormat()
    {
        var model = new DummyModel { Amount = 1.123m, Id = 1, Year = 2023 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(DummyModel.Amount),
                ReportProgramExpendituresRecordConstants.AmountFormat));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenIdIsNotPositive(long id)
    {
        var model = new DummyModel { Amount = 100.50m, Id = id, Year = 2023 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(DummyModel.Id)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReportingYearIsLessThanMin()
    {
        var model = new DummyModel
        {
            Amount = 100.50m,
            Id = 1,
            Year = ReportProgramExpendituresRecordConstants.ReportingYearMinValue - 1
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Year)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(DummyModel.Year),
                ReportProgramExpendituresRecordConstants.ReportingYearMinValue));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReportingYearIsGreaterThanMax()
    {
        var model = new DummyModel
        {
            Amount = 100.50m,
            Id = 1,
            Year = ReportProgramExpendituresRecordConstants.ReportingYearMaxValue + 1
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Year)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                nameof(DummyModel.Year),
                ReportProgramExpendituresRecordConstants.ReportingYearMaxValue));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        var model = new DummyModel
        {
            Amount = 100.50m,
            Id = 1,
            Year = ReportProgramExpendituresRecordConstants.ReportingYearMinValue,
            Ids = [1, 2, 3]
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIdsAreNotUnique()
    {
        var model = new DummyModel
        {
            Amount = 100.50m,
            Id = 1,
            Year = ReportProgramExpendituresRecordConstants.ReportingYearMinValue,
            Ids = [1, 1, 1]
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Ids)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(DummyModel.Ids)));
    }

    private class DummyModel
    {
        public decimal Amount { get; set; }
        public long Id { get; set; }
        public int Year { get; set; }
        public IEnumerable<long> Ids { get; set; } = [];
    }

    private class DummyValidator : AbstractValidator<DummyModel>
    {
        public DummyValidator()
        {
            RuleFor(x => x.Amount).MustBeValidAmountOfMoney(nameof(DummyModel.Amount));
            RuleFor(x => x.Id).MustBeValidId(nameof(DummyModel.Id));
            RuleFor(x => x.Year).MustBeValidReportingYear(nameof(DummyModel.Year));
            RuleFor(x => x.Ids).MustHaveUniqueIds(nameof(DummyModel.Ids));
        }
    }
}
