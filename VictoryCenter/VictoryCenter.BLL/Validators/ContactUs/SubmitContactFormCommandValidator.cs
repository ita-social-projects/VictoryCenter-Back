using FluentValidation;
using VictoryCenter.BLL.Commands.Public.ContactUs;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.ContactUs;

namespace VictoryCenter.BLL.Validators.ContactUs;

public class SubmitContactFormCommandValidator : AbstractValidator<SubmitContactFormCommand>
{
    public SubmitContactFormCommandValidator()
    {
        RuleFor(x => x.Dto)
            .ChildRules(dtoValidator =>
            {
                dtoValidator.RuleFor(dto => dto.FromEmail)
                    .NotEmpty()
                    .EmailAddress()
                    .WithMessage(
                        ErrorMessagesConstants.PropertyMustBeValidEmail(nameof(ContactUsFormDto.FromEmail)))
                    .MaximumLength(ContactUsConstants.EmailAddressMaxLength)
                    .WithMessage(
                        ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                            nameof(SubmitContactUsFormDto.FromEmail), ContactUsConstants.EmailAddressMaxLength));

                dtoValidator.RuleFor(dto => dto.FromName)
                    .NotEmpty()
                    .MinimumLength(ContactUsConstants.NameMinLength)
                        .WithMessage(
                            ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                                nameof(SubmitContactUsFormDto.FromName), ContactUsConstants.NameMinLength))
                    .MaximumLength(ContactUsConstants.NameMaxLength)
                        .WithMessage(
                            ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                                nameof(SubmitContactUsFormDto.FromName), ContactUsConstants.NameMaxLength));

                dtoValidator.RuleFor(dto => dto.Subject)
                    .NotEmpty()
                    .MinimumLength(ContactUsConstants.EmailSubjectMinLength)
                        .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                            nameof(SubmitContactUsFormDto.Subject), ContactUsConstants.EmailSubjectMinLength))
                    .MaximumLength(ContactUsConstants.EmailSubjectMaxLength)
                        .WithMessage(
                            ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                                nameof(SubmitContactUsFormDto.Subject), ContactUsConstants.EmailSubjectMaxLength));

                dtoValidator.RuleFor(dto => dto.Message)
                    .NotEmpty()
                    .MinimumLength(ContactUsConstants.EmailMessageMinLength)
                        .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                            nameof(SubmitContactUsFormDto.Message), ContactUsConstants.EmailMessageMinLength))
                    .MaximumLength(ContactUsConstants.EmailMessageMaxLength)
                        .WithMessage(
                            ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                                nameof(SubmitContactUsFormDto.Message), ContactUsConstants.EmailMessageMaxLength));

                dtoValidator.RuleFor(dto => dto.CaptchaResponseToken)
                    .NotEmpty();
            });
    }
}
