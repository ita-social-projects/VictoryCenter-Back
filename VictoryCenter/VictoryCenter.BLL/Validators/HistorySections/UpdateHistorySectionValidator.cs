using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.HistorySections;

public class UpdateHistorySectionValidator : AbstractValidator<UpdateHistorySectionDto>
{
    public UpdateHistorySectionValidator()
    {
        RuleFor(x => x.Template)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(UpdateHistorySectionDto.Template)));

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan(nameof(UpdateHistorySectionDto.Order), -1));

        RuleFor(x => x.Contents)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHistorySectionDto.Contents)));

        RuleFor(x => x).Custom(ValidateSection);
    }

    private static void ValidateSection(
        UpdateHistorySectionDto section,
        ValidationContext<UpdateHistorySectionDto> ctx)
    {
        if (section.Contents is null)
        {
            return;
        }

        if (!Enum.IsDefined(section.Template))
        {
            return;
        }

        var contents = section.Contents;

        var req = HistorySectionConstants.GetRequirements(section.Template);
        const string prop = nameof(UpdateHistorySectionDto.Contents);

        ValidateCounts(section, ctx, contents, req, prop);
        ValidateUniqueness(ctx, contents, prop);
        ValidateBasicValues(ctx, contents, prop);
        ValidateTitles(section, ctx, contents, req, prop);
        ValidateDescriptions(section, ctx, contents, req, prop);
        ValidateImages(ctx, contents, prop);
    }

    private static void ValidateCounts(
        UpdateHistorySectionDto section,
        ValidationContext<UpdateHistorySectionDto> ctx,
        IReadOnlyCollection<UpdateHistorySectionContentDto> contents,
        HistorySectionConstants.TemplateRequirementsConfig req,
        string prop)
    {
        if (!InRange(CountByType(contents, ContentType.Title), req.TitleCount))
        {
            ctx.AddFailure(prop, HistorySectionConstants.GetTitlesCountErrorMessage(section.Template, CountByType(contents, ContentType.Title)));
        }

        if (!InRange(CountByType(contents, ContentType.Description), req.DescriptionCount))
        {
            ctx.AddFailure(prop, HistorySectionConstants.GetDescriptionsCountErrorMessage(section.Template, CountByType(contents, ContentType.Description)));
        }

        if (!InRange(CountByType(contents, ContentType.Image), req.ImageCount))
        {
            ctx.AddFailure(prop, HistorySectionConstants.GetImagesCountErrorMessage(section.Template, CountByType(contents, ContentType.Image)));
        }
    }

    private static void ValidateUniqueness(
        ValidationContext<UpdateHistorySectionDto> ctx,
        IReadOnlyCollection<UpdateHistorySectionContentDto> contents,
        string prop)
    {
        if (!HasUniqueOrders(contents))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateHistorySectionContentDto.Order)));
        }

        if (!HasUniqueImageIds(contents))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateHistorySectionContentDto.ImageId)));
        }
    }

    private static void ValidateBasicValues(
        ValidationContext<UpdateHistorySectionDto> ctx,
        IReadOnlyCollection<UpdateHistorySectionContentDto> contents,
        string prop)
    {
        if (contents.Any(c => !Enum.IsDefined(c.ContentType)))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(CreateHistorySectionContentDto.ContentType)));
        }

        if (contents.Any(c => c.Order < 0))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.PropertyMustBeGreaterThan(nameof(CreateHistorySectionContentDto.Order), -1));
        }
    }

    private static void ValidateTitles(
        UpdateHistorySectionDto section,
        ValidationContext<UpdateHistorySectionDto> ctx,
        IReadOnlyCollection<UpdateHistorySectionContentDto> contents,
        HistorySectionConstants.TemplateRequirementsConfig req,
        string prop)
    {
        var titles = contents.Where(c => c.ContentType == ContentType.Title).ToList();

        if (titles.Any(c => string.IsNullOrWhiteSpace(c.Title)))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.PropertyIsRequired(nameof(CreateHistorySectionContentDto.Title)));
        }

        if (titles.Any(c => !HasValidLength(c.Title, req.TitleLength)))
        {
            ctx.AddFailure(prop, HistorySectionConstants.GetTitleLengthErrorMessage(section.Template));
        }
    }

    private static void ValidateDescriptions(
        UpdateHistorySectionDto section,
        ValidationContext<UpdateHistorySectionDto> ctx,
        IReadOnlyCollection<UpdateHistorySectionContentDto> contents,
        HistorySectionConstants.TemplateRequirementsConfig req,
        string prop)
    {
        var descriptions = contents.Where(c => c.ContentType == ContentType.Description).ToList();

        if (descriptions.Any(c => string.IsNullOrWhiteSpace(c.Description)))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.PropertyIsRequired(nameof(CreateHistorySectionContentDto.Description)));
        }

        if (descriptions.Any(c => !HasValidLength(c.Description, req.DescriptionLength)))
        {
            ctx.AddFailure(prop, HistorySectionConstants.GetDescriptionLengthErrorMessage(section.Template));
        }
    }

    private static void ValidateImages(
        ValidationContext<UpdateHistorySectionDto> ctx,
        IReadOnlyCollection<UpdateHistorySectionContentDto> contents,
        string prop)
    {
        var images = contents.Where(c => c.ContentType == ContentType.Image).ToList();

        if (images.Any(c => c.ImageId is null or <= 0))
        {
            ctx.AddFailure(prop, ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateHistorySectionContentDto.ImageId)));
        }
    }

    private static int CountByType(
        IReadOnlyCollection<UpdateHistorySectionContentDto> contents,
        ContentType type)
        => contents.Count(c => c.ContentType == type);

    private static bool HasUniqueOrders(IReadOnlyCollection<UpdateHistorySectionContentDto> contents)
        => contents.Select(c => c.Order).Distinct().Count() == contents.Count;

    private static bool HasUniqueImageIds(IReadOnlyCollection<UpdateHistorySectionContentDto> contents)
    {
        var ids = contents
            .Where(c => c.ContentType == ContentType.Image && c.ImageId.HasValue)
            .Select(c => c.ImageId!.Value)
            .ToList();

        return ids.Distinct().Count() == ids.Count;
    }

    private static bool InRange(int actual, (int Min, int Max) req)
        => (req.Min == 0 && req.Max == 0) ? actual == 0 : actual >= req.Min && actual <= req.Max;

    private static bool HasValidLength(string? text, (int Min, int Max) req)
    {
        var t = text?.Trim();
        return !string.IsNullOrEmpty(t) && t.Length >= req.Min && t.Length <= req.Max;
    }
}
