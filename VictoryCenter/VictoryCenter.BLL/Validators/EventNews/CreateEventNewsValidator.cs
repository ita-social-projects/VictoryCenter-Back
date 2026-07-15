using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.EventNews.Create;

namespace VictoryCenter.BLL.Validators.EventNews;

public class CreateEventNewsValidator : AbstractValidator<CreateEventNewsCommand>
{
    public CreateEventNewsValidator(BaseEventNewsValidator baseEventNewsValidator)
    {
        RuleFor(command => command.CreateEventNewsDto)
            .NotNull()
            .SetValidator(baseEventNewsValidator);
    }
}
