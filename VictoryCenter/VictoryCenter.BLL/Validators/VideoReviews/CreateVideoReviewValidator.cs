using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.VideoReviews.Create;

namespace VictoryCenter.BLL.Validators.VideoReviews;

public class CreateVideoReviewValidator : AbstractValidator<CreateVideoReviewCommand>
{
    public CreateVideoReviewValidator(BaseVideoReviewDtoValidator baseVideoReviewDtoValidator)
    {
        RuleFor(command => command.VideoReview)
            .NotNull()
            .SetValidator(baseVideoReviewDtoValidator);
    }
}
