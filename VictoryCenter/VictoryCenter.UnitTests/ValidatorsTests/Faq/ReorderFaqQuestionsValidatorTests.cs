using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.FaqQuestions.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Validators.FaqQuestions;

namespace VictoryCenter.UnitTests.ValidatorsTests.Faq;

public class ReorderFaqQuestionsValidatorTests
{
    private readonly ReorderFaqQuestionsValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveErrors()
    {
        var dto = new ReorderFaqQuestionsDto
        {
            PageId = 1,
            OrderedIds = [1, 2, 3]
        };
        var command = new ReorderFaqQuestionsCommand(dto);

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Validate_PageIdIsNotPositive_ShouldHaveError(long pageId)
    {
        var dto = new ReorderFaqQuestionsDto
        {
            PageId = pageId,
            OrderedIds = [1, 2, 3]
        };
        var command = new ReorderFaqQuestionsCommand(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ReorderFaqQuestionsDto.PageId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ReorderFaqQuestionsDto.PageId)));
    }

    [Fact]
    public void Validate_OrderedIdsIsNull_ShouldHaveError()
    {
        var dto = new ReorderFaqQuestionsDto
        {
            PageId = 1,
            OrderedIds = null!
        };
        var command = new ReorderFaqQuestionsCommand(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ReorderFaqQuestionsDto.OrderedIds)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ReorderFaqQuestionsDto.OrderedIds)));
    }

    [Fact]
    public void Validate_OrderedIdsIsEmpty_ShouldHaveError()
    {
        var dto = new ReorderFaqQuestionsDto
        {
            PageId = 1,
            OrderedIds = []
        };
        var command = new ReorderFaqQuestionsCommand(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ReorderFaqQuestionsDto.OrderedIds)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(ReorderFaqQuestionsDto.OrderedIds)));
    }

    [Fact]
    public void Validate_OrderedIdsContainsDuplicates_ShouldHaveError()
    {
        var dto = new ReorderFaqQuestionsDto
        {
            PageId = 1,
            OrderedIds = [1, 2, 3, 2]
        };
        var command = new ReorderFaqQuestionsCommand(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ReorderFaqQuestionsDto.OrderedIds)
            .WithErrorMessage(ErrorMessagesConstants
                .CollectionMustContainUniqueValues(nameof(ReorderFaqQuestionsDto.OrderedIds)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Validate_OrderedIdsContainInvalidId_ShouldHaveError(long id)
    {
        var dto = new ReorderFaqQuestionsDto
        {
            PageId = 1,
            OrderedIds = [1, id, 3]
        };
        var command = new ReorderFaqQuestionsCommand(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ReorderFaqQuestionsDto.OrderedIds)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustBePositive($"Each {nameof(ReorderFaqQuestionsDto.OrderedIds)} element"));
    }

    [Fact]
    public void Validate_OrderedIdsExceedsMaxLimit_ShouldHaveError()
    {
        var idsCount = 501;
        var dto = new ReorderFaqQuestionsDto
        {
            PageId = 1,
            OrderedIds = Enumerable.Range(1, idsCount).Select(i => (long)i).ToList()
        };
        var command = new ReorderFaqQuestionsCommand(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ReorderFaqQuestionsDto.OrderedIds)
            .WithErrorMessage(ErrorMessagesConstants
                .CollectionCannotContainMoreThan(nameof(ReorderFaqQuestionsDto.OrderedIds), 500));
    }
}
