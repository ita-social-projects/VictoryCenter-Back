using FluentValidation;
using VictoryCenter.BLL.Commands.Donations.CreateUah;

namespace VictoryCenter.BLL.Validators.Donations;

public class CreateUahBankDetailsValidator : AbstractValidator<CreateUahBankDetailsCommand>
{
    public CreateUahBankDetailsValidator()
    {
        RuleFor(x => x.CreateUahBankDetailsDto.Edrpou)
            .NotEmpty()
            .WithMessage("ЄДРПОУ є обов'язковим полем")
            .Matches("^[0-9]*$")
            .WithMessage("Мають бути цифри")
            .Length(8)
            .WithMessage("ЄДРПОУ повинен містити рівно 8 цифр");

        RuleFor(x => x.CreateUahBankDetailsDto.Iban)
            .NotEmpty()
            .WithMessage("IBAN є обов'язковим полем")
            .Matches("^UA[0-9]{27}$")
            .WithMessage("Мають бути цифри")
            .Length(29)
            .WithMessage("IBAN повинен містити рівно 27 цифр");
    }
}
