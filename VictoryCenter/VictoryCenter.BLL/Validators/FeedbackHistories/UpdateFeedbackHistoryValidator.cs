using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Update;

namespace VictoryCenter.BLL.Validators.FeedbackHistories;

public class UpdateFeedbackHistoryValidator : AbstractValidator<UpdateFeedbackHistoryCommand>
{
    public UpdateFeedbackHistoryValidator(BaseFeedbackHistoryValidator baseFeedbackHistoryValidator)
    {
        RuleFor(x => x.UpdateFeedbackHistoryDto).SetValidator(baseFeedbackHistoryValidator);
    }
}
