using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;

namespace VictoryCenter.BLL.Validators.EventNews;

public class GetPublishedEventNewsQueryValidator : AbstractValidator<GetPublishedEventNewsQuery>
{
    public GetPublishedEventNewsQueryValidator()
    {
        RuleFor(query => query.Offset)
            .GreaterThanOrEqualTo(EventNewsConstants.PublishedOffsetMinValue)
            .When(query => query.Offset.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(GetPublishedEventNewsQuery.Offset),
                EventNewsConstants.PublishedOffsetMinValue));

        RuleFor(query => query.Limit)
            .GreaterThanOrEqualTo(EventNewsConstants.PublishedTakeMinValue)
            .When(query => query.Limit.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(GetPublishedEventNewsQuery.Limit),
                EventNewsConstants.PublishedTakeMinValue));

        RuleFor(query => query.Limit)
            .LessThanOrEqualTo(EventNewsConstants.PublishedTakeMaxValue)
            .When(query => query.Limit.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                nameof(GetPublishedEventNewsQuery.Limit),
                EventNewsConstants.PublishedTakeMaxValue));

        RuleFor(query => query.CategoryId)
            .GreaterThanOrEqualTo(1)
            .When(query => query.CategoryId.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(GetPublishedEventNewsQuery.CategoryId),
                1));
    }
}
