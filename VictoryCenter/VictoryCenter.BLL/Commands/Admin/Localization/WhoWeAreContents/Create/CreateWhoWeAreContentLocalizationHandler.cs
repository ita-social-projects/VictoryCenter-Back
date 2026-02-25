using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.WhoWeAreContents.Create;

public class CreateWhoWeAreContentLocalizationHandler : IRequestHandler<CreateWhoWeAreContentLocalizationCommand, Result<List<WhoWeAreContentLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<CreateWhoWeAreContentLocalizationCommand> _validator;
    private readonly ILocalizationService<WhoWeAreContent, WhoWeAreContentLocalization> _localizationService;
    private readonly IRepositoryWrapper _repository;

    public CreateWhoWeAreContentLocalizationHandler(
        IMapper mapper,
        IValidator<CreateWhoWeAreContentLocalizationCommand> validator,
        ILocalizationService<WhoWeAreContent, WhoWeAreContentLocalization> localizationService,
        IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
        _repository = repository;
    }

    public async Task<Result<List<WhoWeAreContentLocalizationDto>>> Handle(CreateWhoWeAreContentLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = _repository.BeginTransaction();

            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var dictEntities = await GetContentToLocalizeMappedToDictionary(request.ContentLocalizationDtos);
            var sectionId = await GetSectionIdByType(request.SectionType) ??
                            throw new ArgumentException(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(request.SectionType)));

            var createdLocalizations = new List<WhoWeAreContentLocalizationDto>();

            foreach (var dto in request.ContentLocalizationDtos)
            {
                if (!dictEntities.TryGetValue(dto.EntityId, out var whoWeAreContent))
                {
                    return Result.Fail(ErrorMessagesConstants.NotFound(dto.EntityId, typeof(WhoWeAreContent)));
                }

                if (whoWeAreContent.SectionId != sectionId)
                {
                    return Result.Fail(WhoWeAreConstants.EntityDoesNotBelongToTheSection(typeof(WhoWeAreContent), request.SectionType));
                }

                var validationError = ValidateDtoFieldsMatchContentType(dto, whoWeAreContent);
                if (validationError != null)
                {
                    return Result.Fail(validationError);
                }

                var sanitizedDto = SanitizeDtoBasedOnContentType(dto, whoWeAreContent);

                WhoWeAreContentLocalization entity = _mapper.Map<WhoWeAreContentLocalization>(sanitizedDto);
                var result = await _localizationService.CreateEntityLocalizationAsync(entity);
                WhoWeAreContentLocalizationDto responseDto = _mapper.Map<WhoWeAreContentLocalizationDto>(result);
                createdLocalizations.Add(responseDto);
            }

            transaction.Complete();
            return Result.Ok(createdLocalizations);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(knfex.Message);
        }
        catch (ArgumentException aex)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(aex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(ErrorMessagesConstants.FailedToCreateEntity(typeof(WhoWeAreContentLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(WhoWeAreContentLocalization)));
        }
    }

    private async Task<Dictionary<long, WhoWeAreContent>> GetContentToLocalizeMappedToDictionary(List<CreateWhoWeAreContentLocalizationDto> content)
    {
        var contentIds = content.Select(x => x.EntityId).ToList();

        var entities = await _repository.WhoWeAreContentsRepository.GetAllAsync(new QueryOptions<WhoWeAreContent>
        {
            Filter = w => contentIds.Contains(w.Id)
        });

        return entities.ToDictionary(x => x.Id, x => x);
    }

    private async Task<long?> GetSectionIdByType(SectionType sectionType)
    {
        var section = await _repository.WhoWeAreSectionsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<WhoWeAreSection>
            {
                Filter = x => x.SectionType == sectionType
            });

        return section?.Id;
    }

    private static string? ValidateDtoFieldsMatchContentType(CreateWhoWeAreContentLocalizationDto dto, WhoWeAreContent content)
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

    private static CreateWhoWeAreContentLocalizationDto SanitizeDtoBasedOnContentType(CreateWhoWeAreContentLocalizationDto dto, WhoWeAreContent content)
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
