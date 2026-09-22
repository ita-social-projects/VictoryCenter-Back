using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.EventsPage.Update;

namespace VictoryCenter.BLL.Validators.EventsPage;

public class UpdateEventsBlockTitleCommandValidator : AbstractValidator<UpdateEventsBlockTitleCommand>
{
    public UpdateEventsBlockTitleCommandValidator(UpdateEventsBlockTitleDtoValidator dtoValidator)
    {
        RuleFor(x => x.Dto).SetValidator(dtoValidator);
    }
}
