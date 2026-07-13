using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;

namespace VictoryCenter.BLL.Validators.EventNews;

public class GetPublishedEventNewsQueryValidator : AbstractValidator<GetPublishedEventNewsQuery>
{
    public GetPublishedEventNewsQueryValidator()
    {
        RuleFor(query => query.Take)
            .GreaterThanOrEqualTo(EventNewsConstants.PublishedTakeMinValue)
            .When(query => query.Take.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(GetPublishedEventNewsQuery.Take),
                EventNewsConstants.PublishedTakeMinValue));

        RuleFor(query => query.Take)
            .LessThanOrEqualTo(EventNewsConstants.PublishedTakeMaxValue)
            .When(query => query.Take.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                nameof(GetPublishedEventNewsQuery.Take),
                EventNewsConstants.PublishedTakeMaxValue));
    }
}
