using FluentResults;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Helpers;

internal static class WhoWeAreContentLocalizationHandlerHelper
{
    internal static async Task<Result<List<UpdateWhoWeAreContentLocalizationDto>>> ValidateAndSanitizeAsync(
        IRepositoryWrapper repository,
        SectionType sectionType,
        IEnumerable<UpdateWhoWeAreContentLocalizationDto> contentLocalizationDtos)
    {
        var contentLocalizationDtosList = contentLocalizationDtos.ToList();
        var dictEntities = await GetContentToLocalizeMappedToDictionary(repository, contentLocalizationDtosList);
        var sectionId = await GetSectionIdByType(repository, sectionType) ??
                        throw new ArgumentException(ErrorMessagesConstants.PropertyMustBeValidEnum("SectionType"));

        var sanitizedDtos = new List<UpdateWhoWeAreContentLocalizationDto>(contentLocalizationDtosList.Count);

        foreach (var dto in contentLocalizationDtosList)
        {
            if (!dictEntities.TryGetValue(dto.EntityId, out var whoWeAreContent))
            {
                return Result.Fail(ErrorMessagesConstants.NotFound(dto.EntityId, typeof(WhoWeAreContent)));
            }

            if (whoWeAreContent.SectionId != sectionId)
            {
                return Result.Fail(WhoWeAreConstants.EntityDoesNotBelongToTheSection(typeof(WhoWeAreContent), sectionType));
            }

            var validationError = ValidateDtoFieldsMatchContentType(dto, whoWeAreContent);
            if (validationError != null)
            {
                return Result.Fail(validationError);
            }

            sanitizedDtos.Add(SanitizeDtoBasedOnContentType(dto, whoWeAreContent));
        }

        return Result.Ok(sanitizedDtos);
    }

    private static async Task<Dictionary<long, WhoWeAreContent>> GetContentToLocalizeMappedToDictionary(
        IRepositoryWrapper repository,
        List<UpdateWhoWeAreContentLocalizationDto> content)
    {
        var contentIds = content.Select(x => x.EntityId).ToList();

        var entities = await repository.WhoWeAreContentsRepository.GetAllAsync(new QueryOptions<WhoWeAreContent>
        {
            Filter = w => contentIds.Contains(w.Id)
        });

        return entities.ToDictionary(x => x.Id, x => x);
    }

    private static async Task<long?> GetSectionIdByType(IRepositoryWrapper repository, SectionType sectionType)
    {
        var section = await repository.WhoWeAreSectionsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<WhoWeAreSection>
            {
                Filter = x => x.SectionType == sectionType
            });

        return section?.Id;
    }

    private static string? ValidateDtoFieldsMatchContentType(UpdateWhoWeAreContentLocalizationDto dto, WhoWeAreContent content)
    {
        return content switch
        {
            ImageContent => WhoWeAreContentLocalizationConstants.CannotCreateLocalizationForContentType(typeof(ImageContent), dto.EntityId),
            TitleContent when string.IsNullOrWhiteSpace(dto.Title) =>
                WhoWeAreContentLocalizationConstants.FieldIsRequiredForContentType(nameof(dto.Title), typeof(TitleContent), dto.EntityId),
            DescriptionContent when string.IsNullOrWhiteSpace(dto.Description) =>
                WhoWeAreContentLocalizationConstants.FieldIsRequiredForContentType(nameof(dto.Description), typeof(DescriptionContent), dto.EntityId),
            CardContent when string.IsNullOrWhiteSpace(dto.Description) =>
                WhoWeAreContentLocalizationConstants.FieldIsRequiredForContentType(nameof(dto.Description), typeof(CardContent), dto.EntityId),
            _ => null
        };
    }

    private static UpdateWhoWeAreContentLocalizationDto SanitizeDtoBasedOnContentType(UpdateWhoWeAreContentLocalizationDto dto, WhoWeAreContent content)
    {
        return content switch
        {
            TitleContent => dto with { Description = null },
            DescriptionContent => dto with { Title = null },
            CardContent => dto with { Title = null },
            _ => dto
        };
    }
}
