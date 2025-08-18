using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Validators.FaqQuestions;

namespace VictoryCenter.UnitTests.ValidatorsTests.FaqQuestions;

public class BaseFaqQuestionValidatorTests
{
    private readonly string _validQuestionText = new('Q', 15);
    private readonly string _validAnswerText = new('A', 60);

    private readonly string _tooShortQuestionText = "Q";
    private readonly string _tooLongQuestionText = new('Q', 160);

    private readonly string _tooShortAnswerText = "A";
    private readonly string _tooLongAnswerText = new('A', 1050);
    private readonly BaseFaqQuestionValidator _validator;

    public BaseFaqQuestionValidatorTests()
    {
        _validator = new BaseFaqQuestionValidator();
    }

    [Fact]
    public void BaseFaqQuestionValidator_ShouldHaveError_WhenQuestionTextIsEmpty()
    {
        var model = new CreateFaqQuestionDto { QuestionText = "", AnswerText = _validAnswerText, };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuestionText)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFaqQuestionDto.QuestionText)));
    }

    [Fact]
    public void BaseFaqQuestionValidator_ShouldHaveError_WhenQuestionTextIsShort()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _tooShortQuestionText, AnswerText = _validAnswerText, };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuestionText)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateFaqQuestionDto.QuestionText), 10));
    }

    [Fact]
    public void BaseFaqQuestionValidator_ShouldHaveError_WhenQuestionTextIsTooLong()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _tooLongQuestionText, AnswerText = _validAnswerText, };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuestionText)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateFaqQuestionDto.QuestionText), 150));
    }

    [Fact]
    public void BaseFaqQuestionValidator_ShouldHaveError_WhenAnswerTextIsEmpty()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _validQuestionText, AnswerText = "", };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AnswerText)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFaqQuestionDto.AnswerText)));
    }

    [Fact]
    public void BaseFaqQuestionValidator_ShouldHaveError_WhenAnswerTextIsShort()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _validQuestionText, AnswerText = _tooShortAnswerText, };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AnswerText)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateFaqQuestionDto.AnswerText), 50));
    }

    [Fact]
    public void BaseFaqQuestionValidator_ShouldHaveError_WhenAnswerTextIsTooLong()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _validQuestionText, AnswerText = _tooLongAnswerText, };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AnswerText)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateFaqQuestionDto.AnswerText), 1000));
    }

    [Fact]
    public void BaseFaqQuestionValidator_ShouldHaveError_WhenPageIdsIsEmpty()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _validQuestionText, AnswerText = _validAnswerText, PageIds = [] };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PageIds)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(CreateFaqQuestionDto.PageIds)));
    }

    [Fact]
    public void BaseFaqQuestionValidator_ShouldHaveError_WhenPageIdsIsNotPositive()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _validQuestionText, AnswerText = _validAnswerText, PageIds = [-1] };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PageIds)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateFaqQuestionDto.PageIds)));
    }

    [Fact]
    public void BaseFaqQuestionValidator_ShouldHaveError_WhenStatusIsUnknown()
    {
        var model = new CreateFaqQuestionDto
        {
            QuestionText = _validQuestionText,
            AnswerText = _validAnswerText,
            PageIds = [1],
            Status = (DAL.Enums.Status)999
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage(ErrorMessagesConstants.UnknownStatusValue);
    }

    [Fact]
    public void BaseFaqQuestionValidator_ShouldNotHaveErrors_ForValidModel()
    {
        var model = new CreateFaqQuestionDto
        {
            QuestionText = _validQuestionText,
            AnswerText = _validAnswerText,
            PageIds = [1],
            Status = DAL.Enums.Status.Published
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
