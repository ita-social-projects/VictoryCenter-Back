using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Public.ContactUs;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.ContactUs;
using VictoryCenter.BLL.Validators.ContactUs;

namespace VictoryCenter.UnitTests.ValidatorsTests.ContactUs;

public class SubmitContactFormCommandValidatorTests
{
    private readonly SubmitContactFormCommandValidator _validator;

    public SubmitContactFormCommandValidatorTests()
    {
        _validator = new SubmitContactFormCommandValidator();
    }

    [Fact]
    public void Validate_ValidInput_ShouldNotHaveValidationErrors()
    {
        var dto = new SubmitContactUsFormDto
        {
            FromEmail = "test@test.com",
            FromName = "Test Name",
            Subject = "Test Subject",
            Message = "Test message content",
            CaptchaResponseToken = "captcha_token"
        };
        var command = new SubmitContactFormCommand(dto);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_FromEmailIsEmpty_ShouldHaveValidationError(string? email)
    {
        var dto = new SubmitContactUsFormDto
        {
            FromEmail = email!,
            FromName = "Test Name",
            Subject = "Test Subject",
            Message = "Test message content",
            CaptchaResponseToken = "captcha_token"
        };
        var command = new SubmitContactFormCommand(dto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.FromEmail);
    }

    [Theory]
    [InlineData("invalid-email")]
    public void Validate_FromEmailIsInvalid_ShouldHaveValidationError(string email)
    {
        var dto = new SubmitContactUsFormDto
        {
            FromEmail = email,
            FromName = "Test Name",
            Subject = "Test Subject",
            Message = "Test message content",
            CaptchaResponseToken = "captcha_token"
        };
        var command = new SubmitContactFormCommand(dto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.FromEmail)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeValidEmail(nameof(ContactUsFormDto.FromEmail)));
    }

    [Fact]
    public void Validate_FromEmailExceedsMaxLength_ShouldHaveValidationError()
    {
        var email = new string('a', ContactUsConstants.EmailAddressMaxLength) + "@test.com";
        var dto = new SubmitContactUsFormDto
        {
            FromEmail = email,
            FromName = "Test Name",
            Subject = "Test Subject",
            Message = "Test message content",
            CaptchaResponseToken = "captcha_token"
        };
        var command = new SubmitContactFormCommand(dto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.FromEmail)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(SubmitContactUsFormDto.FromEmail), ContactUsConstants.EmailAddressMaxLength));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_FromNameIsEmpty_ShouldHaveValidationError(string? name)
    {
        var dto = new SubmitContactUsFormDto
        {
            FromEmail = "test@test.com",
            FromName = name!,
            Subject = "Test Subject",
            Message = "Test message content",
            CaptchaResponseToken = "captcha_token"
        };
        var command = new SubmitContactFormCommand(dto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.FromName);
    }

    // because currently ContactUsConstants.NameMinLength = 1, a name with length less than 1 (0) is an empty string,
    // which fails the NotEmpty validation rule before it even reaches the MinimumLength one
    // [Fact]
    // public void Validate_FromNameLessThanMinLength_ShouldHaveValidationError()
    // {
    //     var name = new string('a', ContactUsConstants.NameMinLength - 1);
    //     var dto = new SubmitContactUsFormDto
    //     {
    //         FromEmail = "test@test.com",
    //         FromName = name,
    //         Subject = "Test Subject",
    //         Message = "Test message content",
    //         CaptchaResponseToken = "captcha_token"
    //     };
    //     var command = new SubmitContactFormCommand(dto);
    //
    //     var result = _validator.TestValidate(command);
    //
    //     result.ShouldHaveValidationErrorFor(x => x.Dto.FromName)
    //           .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
    //               nameof(SubmitContactUsFormDto.FromName), ContactUsConstants.NameMinLength));
    // }

    [Fact]
    public void Validate_FromNameExceedsMaxLength_ShouldHaveValidationError()
    {
        var name = new string('a', ContactUsConstants.NameMaxLength + 1);
        var dto = new SubmitContactUsFormDto
        {
            FromEmail = "test@test.com",
            FromName = name,
            Subject = "Test Subject",
            Message = "Test message content",
            CaptchaResponseToken = "captcha_token"
        };
        var command = new SubmitContactFormCommand(dto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.FromName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(SubmitContactUsFormDto.FromName), ContactUsConstants.NameMaxLength));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_SubjectIsEmpty_ShouldHaveValidationError(string? subject)
    {
        var dto = new SubmitContactUsFormDto
        {
            FromEmail = "test@test.com",
            FromName = "Test Name",
            Subject = subject!,
            Message = "Test message content",
            CaptchaResponseToken = "captcha_token"
        };
        var command = new SubmitContactFormCommand(dto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.Subject);
    }

    [Fact]
    public void Validate_SubjectLessThanMinLength_ShouldHaveValidationError()
    {
        var subject = new string('a', ContactUsConstants.EmailSubjectMinLength - 1);
        var dto = new SubmitContactUsFormDto
        {
            FromEmail = "test@test.com",
            FromName = "Test Name",
            Subject = subject,
            Message = "Test message content",
            CaptchaResponseToken = "captcha_token"
        };
        var command = new SubmitContactFormCommand(dto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.Subject)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(SubmitContactUsFormDto.Subject), ContactUsConstants.EmailSubjectMinLength));
    }

    [Fact]
    public void Validate_SubjectExceedsMaxLength_ShouldHaveValidationError()
    {
        var subject = new string('a', ContactUsConstants.EmailSubjectMaxLength + 1);
        var dto = new SubmitContactUsFormDto
        {
            FromEmail = "test@test.com",
            FromName = "Test Name",
            Subject = subject,
            Message = "Test message content",
            CaptchaResponseToken = "captcha_token"
        };
        var command = new SubmitContactFormCommand(dto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.Subject)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(SubmitContactUsFormDto.Subject), ContactUsConstants.EmailSubjectMaxLength));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_MessageIsEmpty_ShouldHaveValidationError(string? message)
    {
        var dto = new SubmitContactUsFormDto
        {
            FromEmail = "test@test.com",
            FromName = "Test Name",
            Subject = "Test Subject",
            Message = message!,
            CaptchaResponseToken = "captcha_token"
        };
        var command = new SubmitContactFormCommand(dto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.Message);
    }

    [Fact]
    public void Validate_MessageLessThanMinLength_ShouldHaveValidationError()
    {
        var message = new string('a', ContactUsConstants.EmailMessageMinLength - 1);
        var dto = new SubmitContactUsFormDto
        {
            FromEmail = "test@test.com",
            FromName = "Test Name",
            Subject = "Test Subject",
            Message = message,
            CaptchaResponseToken = "captcha_token"
        };
        var command = new SubmitContactFormCommand(dto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.Message)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(SubmitContactUsFormDto.Message), ContactUsConstants.EmailMessageMinLength));
    }

    [Fact]
    public void Validate_MessageExceedsMaxLength_ShouldHaveValidationError()
    {
        var message = new string('a', ContactUsConstants.EmailMessageMaxLength + 1);
        var dto = new SubmitContactUsFormDto
        {
            FromEmail = "test@test.com",
            FromName = "Test Name",
            Subject = "Test Subject",
            Message = message,
            CaptchaResponseToken = "captcha_token"
        };
        var command = new SubmitContactFormCommand(dto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.Message)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(SubmitContactUsFormDto.Message), ContactUsConstants.EmailMessageMaxLength));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_CaptchaResponseTokenIsEmpty_ShouldHaveValidationError(string? token)
    {
        var dto = new SubmitContactUsFormDto
        {
            FromEmail = "test@test.com",
            FromName = "Test Name",
            Subject = "Test Subject",
            Message = "Test message content",
            CaptchaResponseToken = token!
        };
        var command = new SubmitContactFormCommand(dto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto.CaptchaResponseToken);
    }
}
