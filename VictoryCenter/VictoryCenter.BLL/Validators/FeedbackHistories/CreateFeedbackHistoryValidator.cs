using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Create;

namespace VictoryCenter.BLL.Validators.FeedbackHistories;

public class CreateFeedbackHistoryValidator : AbstractValidator<CreateFeedbackHistoryCommand>
{
    public CreateFeedbackHistoryValidator(BaseFeedbackHistoryValidator baseFeedbackHistoryValidator)
    {
        RuleFor(x => x.CreateFeedbackHistoryDto).SetValidator(baseFeedbackHistoryValidator);
    }
}
