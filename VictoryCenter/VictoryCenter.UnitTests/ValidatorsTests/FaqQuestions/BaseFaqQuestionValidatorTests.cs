using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Validators.FaqQuestions;

namespace VictoryCenter.UnitTests.ValidatorsTests.FaqQuestions;

public class BaseFaqQuestionValidatorTests
{
    private readonly string _validQuestionText = new('Q', BaseFaqQuestionValidator.QuestionTextMinLength + 1);
    private readonly string _validAnswerText = new('A', BaseFaqQuestionValidator.AnswerTextMinLength + 1);

    private readonly string _tooShortQuestionText = new('Q', BaseFaqQuestionValidator.QuestionTextMinLength - 1);
    private readonly string _tooLongQuestionText = new('Q', BaseFaqQuestionValidator.QuestionTextMaxLength + 1);

    private readonly string _tooShortAnswerText = new('A', BaseFaqQuestionValidator.AnswerTextMinLength - 1);
    private readonly string _tooLongAnswerText = new('A', BaseFaqQuestionValidator.AnswerTextMaxLength + 1);
    private readonly BaseFaqQuestionValidator _validator;

    public BaseFaqQuestionValidatorTests()
    {
        _validator = new BaseFaqQuestionValidator();
    }

    [Fact]
    public void Validate_QuestionTextIsEmpty_ShouldHaveError()
    {
        var model = new CreateFaqQuestionDto { QuestionText = "", AnswerText = _validAnswerText, };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuestionText)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFaqQuestionDto.QuestionText)));
    }

    [Fact]
    public void Validate_QuestionTextIsTooShort_ShouldHaveError()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _tooShortQuestionText, AnswerText = _validAnswerText, };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuestionText)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateFaqQuestionDto.QuestionText), BaseFaqQuestionValidator.QuestionTextMinLength));
    }

    [Fact]
    public void Validate_QuestionTextIsTooLong_ShouldHaveError()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _tooLongQuestionText, AnswerText = _validAnswerText, };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QuestionText)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateFaqQuestionDto.QuestionText), BaseFaqQuestionValidator.QuestionTextMaxLength));
    }

    [Fact]
    public void Validate_AnswerTextIsEmpty_ShouldHaveError()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _validQuestionText, AnswerText = "", };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AnswerText)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFaqQuestionDto.AnswerText)));
    }

    [Fact]
    public void Validate_AnswerTextIsTooShort_ShouldHaveError()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _validQuestionText, AnswerText = _tooShortAnswerText, };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AnswerText)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateFaqQuestionDto.AnswerText), BaseFaqQuestionValidator.AnswerTextMinLength));
    }

    [Fact]
    public void Validate_AnswerTextIsTooLong_ShouldHaveError()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _validQuestionText, AnswerText = _tooLongAnswerText, };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AnswerText)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateFaqQuestionDto.AnswerText), BaseFaqQuestionValidator.AnswerTextMaxLength));
    }

    [Fact]
    public void Validate_PageIdsIsEmpty_ShouldHaveError()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _validQuestionText, AnswerText = _validAnswerText, PageIds = [] };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PageIds)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(CreateFaqQuestionDto.PageIds)));
    }

    [Fact]
    public void Validate_PageIdsContainsDuplicates_ShouldHaveError()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _validQuestionText, AnswerText = _validAnswerText, PageIds = [1, 1, 2] };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PageIds)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateFaqQuestionDto.PageIds)));
    }

    [Fact]
    public void Validate_PageIdsContainsNegativeValues_ShouldHaveError()
    {
        var model = new CreateFaqQuestionDto { QuestionText = _validQuestionText, AnswerText = _validAnswerText, PageIds = [-1] };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PageIds)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateFaqQuestionDto.PageIds)));
    }

    [Fact]
    public void Validate_StatusIsUnknown_ShouldHaveError()
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
    public void Validate_ValidModel_ShouldNotHaveErrors()
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
