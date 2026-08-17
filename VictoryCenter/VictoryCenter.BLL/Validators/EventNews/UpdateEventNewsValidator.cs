using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.EventNews.Update;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.EventNews;

public class UpdateEventNewsValidator : AbstractValidator<UpdateEventNewsCommand>
{
    public UpdateEventNewsValidator(BaseEventNewsValidator baseEventNewsValidator)
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateEventNewsCommand.Id)));

        RuleFor(command => command.EventNews)
            .NotNull()
            .SetValidator(baseEventNewsValidator);
    }
}
