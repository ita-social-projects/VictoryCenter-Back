using FluentValidation;
using VictoryCenter.BLL.Commands.Donations.CreateForeign;

namespace VictoryCenter.BLL.Validators.Donations;

public class CreateForeignBankDetailsValidator : AbstractValidator<CreateForeignBankDetailsCommand>
{
    public CreateForeignBankDetailsValidator()
    {
        RuleFor(x => x.CreateForeignBankDetailsDto.Iban)
            .NotEmpty()
            .WithMessage("IBAN є обов'язковим полем")
            .Length(27)
            .WithMessage("IBAN повинен містити рівно 27 символів");

        RuleFor(x => x.CreateForeignBankDetailsDto.SwiftCode)
            .NotEmpty()
            .WithMessage("SWIFT-код банку є обов'язковим полем")
            .Length(11)
            .WithMessage("SWIFT-код банку повинен містити рівно 11 символів");

        RuleForEach(x => x.CreateForeignBankDetailsDto.CorrespondentBanks)
            .SetValidator(new CreateCorrespondentBankValidator());
    }
}
