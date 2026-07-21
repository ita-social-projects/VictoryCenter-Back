using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.EventNews;

public class BaseEventNewsValidator : AbstractValidator<CreateEventNewsDto>
{
    public BaseEventNewsValidator()
    {
        RuleFor(dto => dto.Status)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.UnknownStatusValue);

        RuleFor(dto => dto.Resource)
            .MaximumLength(EventNewsConstants.ResourceMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateEventNewsDto.Resource),
                EventNewsConstants.ResourceMaxLength))
            .When(dto => !string.IsNullOrWhiteSpace(dto.Resource));

        RuleFor(dto => dto.PublishedAt)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateEventNewsDto.PublishedAt)))
            .When(dto => dto.Status == Status.Published);

        RuleFor(dto => dto.PreviewImageId)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateEventNewsDto.PreviewImageId)))
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateEventNewsDto.PreviewImageId)))
            .When(dto => dto.Status == Status.Published);

        RuleFor(dto => dto.PreviewImageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateEventNewsDto.PreviewImageId)))
            .When(dto => dto.PreviewImageId.HasValue);

        RuleFor(dto => dto.BackgroundImageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateEventNewsDto.BackgroundImageId)))
            .When(dto => dto.BackgroundImageId.HasValue);

        RuleFor(dto => dto.CategoryIds)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateEventNewsDto.CategoryIds)))
            .When(dto => dto.Status == Status.Published);

        RuleFor(dto => dto.CategoryIds)
            .Must(categoryIds => categoryIds is null || categoryIds.Distinct().Count() == categoryIds.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateEventNewsDto.CategoryIds)));

        RuleForEach(dto => dto.CategoryIds)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateEventNewsDto.CategoryIds)));

        RuleFor(dto => dto.Localizations)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateEventNewsDto.Localizations)))
            .When(dto => dto.Status == Status.Published);

        RuleFor(dto => dto.Localizations)
            .Must(HaveUniqueLanguageIds)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateEventNewsDto.Localizations)));

        RuleFor(dto => dto.Localizations)
            .Custom(ValidateLocalizations);
    }

    private static bool HaveUniqueLanguageIds(ICollection<CreateEventNewsLocalizationDto>? localizations)
    {
        if (localizations is null)
        {
            return true;
        }

        var languageIds = localizations
            .Where(localization => localization is not null && IsMeaningful(localization))
            .Select(localization => localization.LanguageId)
            .ToList();

        return languageIds.Distinct().Count() == languageIds.Count;
    }

    private static void ValidateLocalizations(
        ICollection<CreateEventNewsLocalizationDto>? localizations,
        ValidationContext<CreateEventNewsDto> context)
    {
        if (localizations is null)
        {
            return;
        }

        var isPublished = context.InstanceToValidate.Status == Status.Published;
        var index = 0;

        foreach (var localization in localizations)
        {
            var prefix = $"{nameof(CreateEventNewsDto.Localizations)}[{index}]";

            if (localization is null)
            {
                context.AddFailure(prefix, ErrorMessagesConstants.CollectionCannotContainNullElements(nameof(CreateEventNewsDto.Localizations)));
                index++;
                continue;
            }

            if (!isPublished && !IsMeaningful(localization))
            {
                index++;
                continue;
            }

            if (localization.LanguageId <= 0)
            {
                context.AddFailure(
                    $"{prefix}.{nameof(CreateEventNewsLocalizationDto.LanguageId)}",
                    ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateEventNewsLocalizationDto.LanguageId)));
            }

            ValidateTitle(localization, context, prefix, isPublished);
            ValidateDescription(localization, context, prefix, isPublished);

            index++;
        }
    }

    private static void ValidateTitle(
        CreateEventNewsLocalizationDto localization,
        ValidationContext<CreateEventNewsDto> context,
        string prefix,
        bool isPublished)
    {
        if (string.IsNullOrWhiteSpace(localization.Title))
        {
            if (isPublished || !string.IsNullOrWhiteSpace(localization.Description))
            {
                context.AddFailure(
                    $"{prefix}.{nameof(CreateEventNewsLocalizationDto.Title)}",
                    ErrorMessagesConstants.PropertyIsRequired(nameof(CreateEventNewsLocalizationDto.Title)));
            }

            return;
        }

        if (localization.Title.Length < EventNewsConstants.TitleMinLength)
        {
            context.AddFailure(
                $"{prefix}.{nameof(CreateEventNewsLocalizationDto.Title)}",
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(CreateEventNewsLocalizationDto.Title),
                    EventNewsConstants.TitleMinLength));
        }

        if (localization.Title.Length > EventNewsConstants.TitleMaxLength)
        {
            context.AddFailure(
                $"{prefix}.{nameof(CreateEventNewsLocalizationDto.Title)}",
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(CreateEventNewsLocalizationDto.Title),
                    EventNewsConstants.TitleMaxLength));
        }
    }

    private static void ValidateDescription(
        CreateEventNewsLocalizationDto localization,
        ValidationContext<CreateEventNewsDto> context,
        string prefix,
        bool isPublished)
    {
        if (string.IsNullOrWhiteSpace(localization.Description))
        {
            if (isPublished)
            {
                context.AddFailure(
                    $"{prefix}.{nameof(CreateEventNewsLocalizationDto.Description)}",
                    ErrorMessagesConstants.PropertyIsRequired(nameof(CreateEventNewsLocalizationDto.Description)));
            }

            return;
        }

        if (localization.Description.Length < EventNewsConstants.DescriptionMinLength)
        {
            context.AddFailure(
                $"{prefix}.{nameof(CreateEventNewsLocalizationDto.Description)}",
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(CreateEventNewsLocalizationDto.Description),
                    EventNewsConstants.DescriptionMinLength));
        }

        if (localization.Description.Length > EventNewsConstants.DescriptionMaxLength)
        {
            context.AddFailure(
                $"{prefix}.{nameof(CreateEventNewsLocalizationDto.Description)}",
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(CreateEventNewsLocalizationDto.Description),
                    EventNewsConstants.DescriptionMaxLength));
        }
    }

    private static bool IsMeaningful(CreateEventNewsLocalizationDto localization)
    {
        return localization.LanguageId > 0
               || !string.IsNullOrWhiteSpace(localization.Title)
               || !string.IsNullOrWhiteSpace(localization.Description);
    }
}
