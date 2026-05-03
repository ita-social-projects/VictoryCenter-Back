using System.Linq.Expressions;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Commands.Public.ContactUs;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.DTOs.Public.ContactUs;
using VictoryCenter.BLL.Interfaces.Captcha;
using VictoryCenter.BLL.Interfaces.Email;
using VictoryCenter.BLL.Options.Email;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ContactUs;

public class SubmitContactFormCommandHandlerTests
{
    private readonly SubmitContactFormCommandHandler _handler;
    private readonly Mock<ICaptchaResponseTokenValidationService> _mockCaptchaService;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IValidator<SubmitContactFormCommand>> _mockValidator;

    public SubmitContactFormCommandHandlerTests()
    {
        _mockValidator = new Mock<IValidator<SubmitContactFormCommand>>();
        _mockCaptchaService = new Mock<ICaptchaResponseTokenValidationService>();
        _mockEmailSender = new Mock<IEmailSender>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        var mockLogger = new Mock<ILogger<SubmitContactFormCommandHandler>>();

        var emailOptions = new ContactUsEmailOptions { FromAddress = "from@test.com" };
        var mockEmailOptions = new Mock<IOptions<ContactUsEmailOptions>>();
        mockEmailOptions.Setup(x => x.Value).Returns(emailOptions);

        _handler = new SubmitContactFormCommandHandler(
            _mockValidator.Object,
            _mockCaptchaService.Object,
            _mockEmailSender.Object,
            mockEmailOptions.Object,
            _mockRepositoryWrapper.Object,
            mockLogger.Object);
    }

    [Fact]
    public async Task Handle_GivenValidationFails_ReturnsFail()
    {
        var dto = new SubmitContactUsFormDto
            { CaptchaResponseToken = "token", FromEmail = "a@a.com", FromName = "a", Message = "m", Subject = "s" };
        var cmd = new SubmitContactFormCommand(dto);
        var validationFailures = new List<ValidationFailure> { new("Prop", "Error message") };
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(validationFailures));

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Error message", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_GivenCaptchaFails_ReturnsFail()
    {
        var dto = new SubmitContactUsFormDto
            { CaptchaResponseToken = "token", FromEmail = "a@a.com", FromName = "a", Message = "m", Subject = "s" };
        var cmd = new SubmitContactFormCommand(dto);
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockCaptchaService
            .Setup(c => c.ValidateTokenAsync("token", It.IsAny<string?>()))
            .ReturnsAsync(Result.Fail("Captcha error"));

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Captcha error", result.Errors[0].Message);
        _mockValidator.Verify(
            v =>
                v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_GivenCompanyEmailIsNull_ReturnsFail()
    {
        var dto = new SubmitContactUsFormDto
            { CaptchaResponseToken = "token", FromEmail = "a@a.com", FromName = "a", Message = "m", Subject = "s" };
        var cmd = new SubmitContactFormCommand(dto);
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockCaptchaService
            .Setup(c => c.ValidateTokenAsync("token", It.IsAny<string?>()))
            .ReturnsAsync(Result.Ok());
        _mockRepositoryWrapper
            .Setup(r => r.CompanyProfileContactRepository.GetFirstOrDefaultProjectedAsync(
                It.IsAny<Expression<Func<CompanyProfileContact, string>>>(),
                It.IsAny<QueryOptions<CompanyProfileContact>>()))
            .ReturnsAsync((string?)null);

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Company correspondence email address is not set.", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_GivenEmailSenderFails_ReturnsFail()
    {
        var dto = new SubmitContactUsFormDto
            { CaptchaResponseToken = "token", FromEmail = "a@a.com", FromName = "a", Message = "m", Subject = "s" };
        var cmd = new SubmitContactFormCommand(dto);
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockCaptchaService
            .Setup(c => c.ValidateTokenAsync("token", It.IsAny<string?>()))
            .ReturnsAsync(Result.Ok());
        _mockRepositoryWrapper
            .Setup(r => r.CompanyProfileContactRepository.GetFirstOrDefaultProjectedAsync(
                It.IsAny<Expression<Func<CompanyProfileContact, string>>>(),
                It.IsAny<QueryOptions<CompanyProfileContact>>()))
            .ReturnsAsync("company@test.com");
        _mockEmailSender
            .Setup(e => e.SendEmailAsync(It.IsAny<EmailDto>()))
            .ReturnsAsync(Result.Fail("Email error"));

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Email error", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_GivenValidData_ReturnsOk()
    {
        var dto = new SubmitContactUsFormDto
            { CaptchaResponseToken = "token", FromEmail = "a@a.com", FromName = "a", Message = "m", Subject = "s" };
        var cmd = new SubmitContactFormCommand(dto);
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockCaptchaService
            .Setup(c => c.ValidateTokenAsync("token", It.IsAny<string?>()))
            .ReturnsAsync(Result.Ok());
        _mockRepositoryWrapper
            .Setup(r => r.CompanyProfileContactRepository.GetFirstOrDefaultProjectedAsync(
                It.IsAny<Expression<Func<CompanyProfileContact, string>>>(),
                It.IsAny<QueryOptions<CompanyProfileContact>>()))
            .ReturnsAsync("company@test.com");
        _mockEmailSender
            .Setup(e => e.SendEmailAsync(It.IsAny<EmailDto>()))
            .ReturnsAsync(Result.Ok());

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(dto.FromEmail, result.Value.FromEmail);
        Assert.Equal(dto.FromName, result.Value.FromName);
        Assert.Equal(dto.Message, result.Value.Message);
        Assert.Equal(dto.Subject, result.Value.Subject);
    }

    [Fact]
    public async Task Handle_GivenDataWithWhiteSpaces_TrimsDataAndReturnsOk()
    {
        var dto = new SubmitContactUsFormDto
            { CaptchaResponseToken = "token", FromEmail = " a@a.com ", FromName = " a ", Message = " m ", Subject = " s " };
        var cmd = new SubmitContactFormCommand(dto);
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockCaptchaService
            .Setup(c => c.ValidateTokenAsync("token", It.IsAny<string?>()))
            .ReturnsAsync(Result.Ok());
        _mockRepositoryWrapper
            .Setup(r => r.CompanyProfileContactRepository.GetFirstOrDefaultProjectedAsync(
                It.IsAny<Expression<Func<CompanyProfileContact, string>>>(),
                It.IsAny<QueryOptions<CompanyProfileContact>>()))
            .ReturnsAsync("company@test.com");
        _mockEmailSender
            .Setup(e => e.SendEmailAsync(It.IsAny<EmailDto>()))
            .ReturnsAsync(Result.Ok());

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("a@a.com", result.Value.FromEmail);
        Assert.Equal("a", result.Value.FromName);
        Assert.Equal("m", result.Value.Message);
        Assert.Equal("s", result.Value.Subject);
    }
}
