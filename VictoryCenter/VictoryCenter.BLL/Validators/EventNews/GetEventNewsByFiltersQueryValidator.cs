using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Queries.Admin.EventNews.GetByFilters;

namespace VictoryCenter.BLL.Validators.EventNews;

public class GetEventNewsByFiltersQueryValidator : AbstractValidator<GetEventNewsByFiltersQuery>
{
    public GetEventNewsByFiltersQueryValidator()
    {
        RuleFor(query => query.Filter.Offset)
            .GreaterThanOrEqualTo(0)
            .When(query => query.Filter.Offset.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN("Offset", 0));

        RuleFor(query => query.Filter.Limit)
            .GreaterThan(0)
            .When(query => query.Filter.Limit.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan("Limit", 0));

        RuleFor(query => query.Filter.CategoryId)
            .GreaterThan(0)
            .When(query => query.Filter.CategoryId.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive("CategoryId"));
    }
}
