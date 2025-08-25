using FluentValidation;
using VictoryCenter.BLL.DTOs.Donations.ForeignBank;

namespace VictoryCenter.BLL.Validators.Donations;

public class CreateCorrespondentBankValidator : AbstractValidator<CreateCorrespondentBankDto>
{
    public CreateCorrespondentBankValidator()
    {
        RuleFor(x => x.SwiftCode)
            .NotEmpty()
            .WithMessage("SWIFT є обов'язковим полем")
            .Length(11)
            .WithMessage("SWIFT повинен містити рівно 11 символів");
    }
}
