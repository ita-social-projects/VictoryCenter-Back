using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.DTOs.Public.ContactUs;
using VictoryCenter.BLL.Errors;
using VictoryCenter.BLL.Interfaces.Captcha;
using VictoryCenter.BLL.Interfaces.Email;
using VictoryCenter.BLL.Options.Email;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Public.ContactUs;

public class SubmitContactFormCommandHandler : IRequestHandler<SubmitContactFormCommand, Result<ContactUsFormDto>>
{
    private readonly ICaptchaResponseTokenValidationService _captchaResponseTokenValidationService;
    private readonly ContactUsEmailOptions _contactUsEmailOptions;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<SubmitContactFormCommandHandler> _logger;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<SubmitContactFormCommand> _validator;

    public SubmitContactFormCommandHandler(
        IValidator<SubmitContactFormCommand> validator,
        ICaptchaResponseTokenValidationService captchaResponseTokenValidationService,
        IEmailSender emailSender,
        IOptions<ContactUsEmailOptions> emailOptions,
        IRepositoryWrapper repositoryWrapper,
        ILogger<SubmitContactFormCommandHandler> logger)
    {
        _validator = validator;
        _captchaResponseTokenValidationService = captchaResponseTokenValidationService;
        _emailSender = emailSender;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _contactUsEmailOptions = emailOptions.Value;
    }

    public async Task<Result<ContactUsFormDto>> Handle(
        SubmitContactFormCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var trimmedDto = TrimWhiteCharactersInDtoFields(request.Dto);
            var trimmedRequest = new SubmitContactFormCommand(trimmedDto);

            await _validator.ValidateAndThrowAsync(trimmedRequest, cancellationToken);

            var captchaValidationResult =
                await _captchaResponseTokenValidationService.ValidateTokenAsync(trimmedDto.CaptchaResponseToken);

            if (captchaValidationResult.IsFailed)
            {
                return Result.Fail(captchaValidationResult.Errors);
            }

            var companyCorrespondenceEmailAddress = await GetCompanyCorrespondenceEmailAddressAsync();

            if (companyCorrespondenceEmailAddress is null)
            {
                const string errorMessage = "Company correspondence email address is not set.";
                _logger.LogCritical(errorMessage);
                return Result.Fail(new InternalError(errorMessage));
            }

            var emailToSend = ComposeEmail(trimmedDto, companyCorrespondenceEmailAddress);

            var sendEmailResult = await _emailSender.SendEmailAsync(emailToSend);

            if (sendEmailResult.IsFailed)
            {
                return Result.Fail(sendEmailResult.Errors);
            }

            return Result.Ok<ContactUsFormDto>(trimmedDto);
        }
        catch (ValidationException validationException)
        {
            return Result.Fail(validationException.Errors.Select(error => error.ErrorMessage));
        }
    }

    private async Task<string?> GetCompanyCorrespondenceEmailAddressAsync()
    {
        var emailAddress = await _repositoryWrapper.CompanyProfileContactRepository.GetFirstOrDefaultProjectedAsync(
            projection => projection.CorrespondenceEmail,
            new QueryOptions<CompanyProfileContact>
            {
                Limit = 1,
                AsNoTracking = true
            });

        return emailAddress;
    }

    private EmailDto ComposeEmail(ContactUsFormDto dto, string companyCorrespondenceEmailAddress)
    {
        return new EmailDto
        {
            From = _contactUsEmailOptions.FromAddress,
            To = [companyCorrespondenceEmailAddress],
            Subject = ContactUsConstants.EmailSubjectTemplate(dto),
            TextBody = ContactUsConstants.EmailTextBodyTemplate(dto),
            ReplyTo = [dto.FromEmail]
        };
    }

    private SubmitContactUsFormDto TrimWhiteCharactersInDtoFields(SubmitContactUsFormDto dto)
    {
        return new SubmitContactUsFormDto
        {
            FromName = dto.FromName.Trim(),
            FromEmail = dto.FromEmail.Trim(),
            Subject = dto.Subject.Trim(),
            Message = dto.Message.Trim(),
            CaptchaResponseToken = dto.CaptchaResponseToken
        };
    }
}
