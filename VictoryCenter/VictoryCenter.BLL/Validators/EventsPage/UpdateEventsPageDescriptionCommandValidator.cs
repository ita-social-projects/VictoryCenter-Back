using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.EventsPage.Update;

namespace VictoryCenter.BLL.Validators.EventsPage;

public class UpdateEventsPageDescriptionCommandValidator : AbstractValidator<UpdateEventsPageDescriptionCommand>
{
    public UpdateEventsPageDescriptionCommandValidator(UpdateEventsPageDescriptionDtoValidator dtoValidator)
    {
        RuleFor(x => x.Dto).SetValidator(dtoValidator);
    }
}
