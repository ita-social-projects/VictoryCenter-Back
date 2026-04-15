using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.MainPage.Create;
using VictoryCenter.BLL.Validators.MainPage.Dto;

namespace VictoryCenter.BLL.Validators.MainPage.Commands;

public class CreateMainPageCommandValidator : AbstractValidator<CreateMainPageCommand>
{
    public CreateMainPageCommandValidator()
    {
        RuleFor(x => x.CreateMainPageDto)
            .NotNull()
            .SetValidator(new CreateMainPageDtoValidator());
    }
}
