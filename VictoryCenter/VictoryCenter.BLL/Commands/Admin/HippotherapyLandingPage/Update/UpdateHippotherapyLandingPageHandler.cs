using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyLandingPage.Update;

public class UpdateHippotherapyLandingPageHandler : IRequestHandler<UpdateHippotherapyLandingPageCommand, Result<HippotherapyLandingPageDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateHippotherapyLandingPageCommand> _validator;
    private readonly IReorderService _reorderService;
    private readonly IHippotherapyLandingPageResponseFactory _responseFactory;

    public UpdateHippotherapyLandingPageHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<UpdateHippotherapyLandingPageCommand> validator,
        IReorderService reorderService,
        IHippotherapyLandingPageResponseFactory hippotherapyLandingPageResponseFactory)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
        _reorderService = reorderService;
        _responseFactory = hippotherapyLandingPageResponseFactory;
    }

    public async Task<Result<HippotherapyLandingPageDto>> Handle(UpdateHippotherapyLandingPageCommand request, CancellationToken cancellationToken)
    {
        var imageIdsToDelete = new List<long>();

        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var dto = request.Dto;

            var requestedImageIds = new List<long?>
            {
                dto.IntroSection.ImageId,
                dto.QuoteSection.ImageId,
                dto.HippoventionCenterSection.ImageId,
                dto.AnotherQuoteSection.ImageId,
                dto.EthicsSection.ImageId,
            }
            .Concat(dto.AdvantagesSection.Cards.Select(c => c.ImageId))
            .Concat(dto.ParticipantsSection.Cards.Select(c => c.ImageId))
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToHashSet();

            var imagesResult = await ImageValidationHelper.ValidateAndGetImagesByIdsAsync(_repositoryWrapper, requestedImageIds);
            if (imagesResult.IsFailed)
            {
                return Result.Fail<HippotherapyLandingPageDto>(imagesResult.Errors);
            }

            long sectionIdForReorder;

            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                var entity = await _repositoryWrapper.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(new QueryOptions<DAL.Entities.HippotherapyLandingPage>
                {
                    AsNoTracking = false,
                    Include = HippotherapyLandingPageIncludeHelper.IncludeFullGraph,
                    AsSplitQuery = true,
                });

                if (entity == null)
                {
                    entity = BuildNewEntity(dto);
                    await _repositoryWrapper.HippotherapyLandingPagesRepository.CreateAsync(entity);
                }
                else
                {
                    TrackImageChange(entity.IntroSection!.ImageId, dto.IntroSection.ImageId, imageIdsToDelete);
                    _mapper.Map(dto.IntroSection, entity.IntroSection);
                    MarkIntroSectionLocalizationsOutdated(entity.IntroSection);

                    _mapper.Map(dto.DescriptionSection, entity.DescriptionSection);
                    if (entity.DescriptionSection is not null)
                    {
                        MarkDescriptionSectionLocalizationsOutdated(entity.DescriptionSection);
                    }

                    TrackImageChange(entity.QuoteSection!.ImageId, dto.QuoteSection.ImageId, imageIdsToDelete);
                    _mapper.Map(dto.QuoteSection, entity.QuoteSection);
                    MarkQuoteSectionLocalizationsOutdated(entity.QuoteSection);

                    _mapper.Map(dto.HippoventionSection, entity.HippoventionSection);
                    if (entity.HippoventionSection is not null)
                    {
                        MarkHippoventionSectionLocalizationsOutdated(entity.HippoventionSection);
                    }

                    TrackImageChange(entity.HippoventionCenterSection!.ImageId, dto.HippoventionCenterSection.ImageId, imageIdsToDelete);
                    _mapper.Map(dto.HippoventionCenterSection, entity.HippoventionCenterSection);
                    MarkHippoventionCenterSectionLocalizationsOutdated(entity.HippoventionCenterSection);

                    _mapper.Map(dto.AdvantagesSection, entity.AdvantagesSection);
                    var advantageCardsResult = UpdateGalleryCards(
                        entity.AdvantagesSection!.AdvantageCards,
                        dto.AdvantagesSection.Cards,
                        imageIdsToDelete,
                        c => c.Id,
                        d => new HippotherapyLandingPageAdvantageCard
                        {
                            Description = d.Description,
                            ImageId = d.ImageId,
                            CreatedAt = DateTimeOffset.UtcNow,
                        },
                        removed => _repositoryWrapper.HippotherapyLandingPageAdvantageCardsRepository.DeleteRange(removed));
                    if (advantageCardsResult.IsFailed)
                    {
                        return Result.Fail<HippotherapyLandingPageDto>(advantageCardsResult.Errors);
                    }

                    MarkAdvantagesSectionLocalizationsOutdated(entity.AdvantagesSection, advantageCardsResult.Value);

                    _mapper.Map(dto.AnalysisSection, entity.AnalysisSection);
                    if (entity.AnalysisSection is not null)
                    {
                        MarkAnalysisSectionLocalizationsOutdated(entity.AnalysisSection);
                    }

                    _mapper.Map(dto.ScientificReferencesSection, entity.ScientificReferencesSection);
                    var referencesResult = UpdateScientificReferences(entity.ScientificReferencesSection!, dto.ScientificReferencesSection);
                    if (referencesResult.IsFailed)
                    {
                        return Result.Fail<HippotherapyLandingPageDto>(referencesResult.Errors);
                    }

                    MarkScientificReferencesSectionLocalizationsOutdated(entity.ScientificReferencesSection!, referencesResult.Value);

                    TrackImageChange(entity.AnotherQuoteSection!.ImageId, dto.AnotherQuoteSection.ImageId, imageIdsToDelete);
                    _mapper.Map(dto.AnotherQuoteSection, entity.AnotherQuoteSection);
                    MarkAnotherQuoteSectionLocalizationsOutdated(entity.AnotherQuoteSection);

                    _mapper.Map(dto.ParticipantsSection, entity.ParticipantsSection);
                    var participantCardsResult = UpdateGalleryCards(
                        entity.ParticipantsSection!.ParticipantCards,
                        dto.ParticipantsSection.Cards,
                        imageIdsToDelete,
                        c => c.Id,
                        d => new HippotherapyLandingPageParticipantCard
                        {
                            Description = d.Description,
                            ImageId = d.ImageId,
                            CreatedAt = DateTimeOffset.UtcNow,
                        },
                        removed => _repositoryWrapper.HippotherapyLandingPageParticipantCardsRepository.DeleteRange(removed));
                    if (participantCardsResult.IsFailed)
                    {
                        return Result.Fail<HippotherapyLandingPageDto>(participantCardsResult.Errors);
                    }

                    MarkParticipantsSectionLocalizationsOutdated(entity.ParticipantsSection, participantCardsResult.Value);

                    TrackImageChange(entity.EthicsSection!.ImageId, dto.EthicsSection.ImageId, imageIdsToDelete);
                    _mapper.Map(dto.EthicsSection, entity.EthicsSection);
                    var principlesResult = UpdateEthicsPrinciples(entity.EthicsSection, dto.EthicsSection.Principles);
                    if (principlesResult.IsFailed)
                    {
                        return Result.Fail<HippotherapyLandingPageDto>(principlesResult.Errors);
                    }

                    MarkEthicsSectionLocalizationsOutdated(entity.EthicsSection, principlesResult.Value);

                    _repositoryWrapper.HippotherapyLandingPagesRepository.Update(entity);
                }

                await _repositoryWrapper.SaveChangesAsync();

                if (ApplyFinalPriorities(entity))
                {
                    await _repositoryWrapper.SaveChangesAsync();
                }

                sectionIdForReorder = entity.ScientificReferencesSection!.Id;
                await _reorderService.RenumberPriorityAsync<HippotherapyLandingPageScientificReference>(
                    groupSelector: r => r.ScientificReferencesSectionId == sectionIdForReorder);

                var idsToActuallyDelete = imageIdsToDelete.Where(id => !requestedImageIds.Contains(id)).ToList();
                if (idsToActuallyDelete.Count > 0)
                {
                    var imagesToDelete = await _repositoryWrapper.ImageRepository.GetAllAsync(new QueryOptions<Image>
                    {
                        Filter = i => idsToActuallyDelete.Contains(i.Id),
                        AsNoTracking = false,
                    });
                    _repositoryWrapper.ImageRepository.DeleteRange(imagesToDelete);
                    await _repositoryWrapper.SaveChangesAsync();
                }

                scope.Complete();
            }

            var result = await _repositoryWrapper.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(new QueryOptions<DAL.Entities.HippotherapyLandingPage>
            {
                Include = HippotherapyLandingPageIncludeHelper.IncludeFullGraph,
                AsSplitQuery = true,
            });

            return Result.Ok(_responseFactory.Create(result!));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (ReorderException ex)
        {
            return Result.Fail<HippotherapyLandingPageDto>(ReorderConstants.ErrorWithReordering(ex.Message));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(DAL.Entities.HippotherapyLandingPage)));
        }
    }

    private static void MarkIntroSectionLocalizationsOutdated(HippotherapyLandingPageIntroSection introSection)
    {
        foreach (var loc in introSection.Localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static void MarkDescriptionSectionLocalizationsOutdated(HippotherapyLandingPageDescriptionSection descriptionSection)
    {
        foreach (var loc in descriptionSection.Localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static void MarkHippoventionSectionLocalizationsOutdated(HippotherapyLandingPageHippoventionSection hippoventionSection)
    {
        foreach (var loc in hippoventionSection.Localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static void MarkAnalysisSectionLocalizationsOutdated(HippotherapyLandingPageAnalysisSection analysisSection)
    {
        foreach (var loc in analysisSection.Localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static void MarkScientificReferencesSectionLocalizationsOutdated(
        HippotherapyLandingPageScientificReferencesSection scientificReferencesSection,
        List<HippotherapyLandingPageScientificReference> changedReferences)
    {
        foreach (var loc in scientificReferencesSection.Localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }

        foreach (var loc in changedReferences.SelectMany(r => r.Localizations))
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static void MarkEthicsSectionLocalizationsOutdated(
        HippotherapyLandingPageEthicsSection ethicsSection,
        List<HippotherapyLandingPageEthicsPrinciple> changedPrinciples)
    {
        foreach (var loc in ethicsSection.Localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }

        foreach (var loc in changedPrinciples.SelectMany(p => p.Localizations))
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static void MarkQuoteSectionLocalizationsOutdated(HippotherapyLandingPageQuoteSection quoteSection)
    {
        foreach (var loc in quoteSection.Localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static void MarkAnotherQuoteSectionLocalizationsOutdated(HippotherapyLandingPageAnotherQuoteSection anotherQuoteSection)
    {
        foreach (var loc in anotherQuoteSection.Localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static void MarkParticipantsSectionLocalizationsOutdated(
        HippotherapyLandingPageParticipantsSection participantsSection,
        List<HippotherapyLandingPageParticipantCard> changedCards)
    {
        foreach (var loc in participantsSection.Localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }

        foreach (var loc in changedCards.SelectMany(c => c.Localizations))
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static void MarkHippoventionCenterSectionLocalizationsOutdated(HippotherapyLandingPageHippoventionCenterSection hippoventionCenterSection)
    {
        foreach (var loc in hippoventionCenterSection.Localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static void MarkAdvantagesSectionLocalizationsOutdated(
        HippotherapyLandingPageAdvantagesSection advantagesSection,
        List<HippotherapyLandingPageAdvantageCard> changedCards)
    {
        foreach (var loc in advantagesSection.Localizations)
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }

        foreach (var loc in changedCards.SelectMany(c => c.Localizations))
        {
            loc.TranslationStatus = TranslationStatus.Outdated;
        }
    }

    private static void TrackImageChange(long? oldImageId, long? newImageId, List<long> imageIdsToDelete)
    {
        if (oldImageId.HasValue && oldImageId != newImageId)
        {
            imageIdsToDelete.Add(oldImageId.Value);
        }
    }

    private Result<List<TCard>> UpdateGalleryCards<TCard>(
    ICollection<TCard> existing,
    List<UpdateGalleryCardDto> incoming,
    List<long> imageIdsToDelete,
    Func<TCard, long> idSelector,
    Func<UpdateGalleryCardDto, TCard> create,
    Action<List<TCard>> deleteRange)
    where TCard : class, IGalleryCard
    {
        return SyncOrderedItems(
            existing,
            incoming,
            d => d.Id,
            idSelector,
            (card, d) =>
            {
                TrackImageChange(card.ImageId, d.ImageId, imageIdsToDelete);
                var changed = !string.Equals(card.Description, d.Description, StringComparison.Ordinal);
                card.ImageId = d.ImageId;
                card.Description = d.Description;
                return changed;
            },
            create,
            (card, priority) => card.Priority = priority,
            removed =>
            {
                foreach (var card in removed)
                {
                    if (card.ImageId.HasValue)
                    {
                        imageIdsToDelete.Add(card.ImageId.Value);
                    }
                }

                deleteRange(removed);
            });
    }

    private Result<List<HippotherapyLandingPageEthicsPrinciple>> UpdateEthicsPrinciples(
        HippotherapyLandingPageEthicsSection section,
        List<UpdateEthicsPrincipleDto> incoming)
    {
        return SyncOrderedItems(
            section.EthicsPrinciples,
            incoming,
            d => d.Id,
            p => p.Id,
            (principle, d) =>
            {
                var changed = !string.Equals(principle.Text, d.Text, StringComparison.Ordinal);
                principle.Text = d.Text;
                return changed;
            },
            d => new HippotherapyLandingPageEthicsPrinciple { Text = d.Text, CreatedAt = DateTimeOffset.UtcNow },
            (principle, priority) => principle.Priority = priority,
            removed => _repositoryWrapper.HippotherapyLandingPageEthicsPrinciplesRepository.DeleteRange(removed));
    }

    private static Result<List<TItem>> SyncOrderedItems<TItem, TDto>(
        ICollection<TItem> existing,
        IReadOnlyList<TDto> incoming,
        Func<TDto, long?> dtoId,
        Func<TItem, long> itemId,
        Func<TItem, TDto, bool> update,
        Func<TDto, TItem> create,
        Action<TItem, int> setPriority,
        Action<List<TItem>> deleteRange)
        where TItem : class
    {
        var requestedIds = incoming
            .Select(dtoId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToList();

        if (requestedIds.Count != requestedIds.Distinct().Count())
        {
            return Result.Fail<List<TItem>>(ErrorMessagesConstants.DuplicateItemIds(typeof(TItem)));
        }

        var requestedIdSet = requestedIds.ToHashSet();
        var existingById = existing.ToDictionary(itemId);
        foreach (var id in requestedIds)
        {
            if (!existingById.ContainsKey(id))
            {
                return Result.Fail<List<TItem>>(ErrorMessagesConstants.NotFound(id, typeof(TItem)));
            }
        }

        var removed = existing.Where(item => !requestedIdSet.Contains(itemId(item))).ToList();

        var changedItems = new List<TItem>();
        for (var index = 0; index < incoming.Count; index++)
        {
            var dto = incoming[index];
            var id = dtoId(dto);
            TItem item;

            if (id.HasValue)
            {
                item = existingById[id.Value];
                if (update(item, dto))
                {
                    changedItems.Add(item);
                }
            }
            else
            {
                item = create(dto);
                existing.Add(item);
            }

            setPriority(item, -(index + 1));
        }

        if (removed.Count > 0)
        {
            deleteRange(removed);
        }

        return Result.Ok(changedItems);
    }

    private Result<List<HippotherapyLandingPageScientificReference>> UpdateScientificReferences(
        HippotherapyLandingPageScientificReferencesSection section,
        UpdateScientificReferencesSectionDto dto)
    {
        var changedReferences = new List<HippotherapyLandingPageScientificReference>();
        var existingById = section.ScientificReferences.ToDictionary(r => r.Id);
        var incomingIds = dto.ScientificReferences
            .Where(r => r.Id.HasValue)
            .Select(r => r.Id!.Value)
            .ToHashSet();

        var toDelete = section.ScientificReferences.Where(r => !incomingIds.Contains(r.Id)).ToList();
        if (toDelete.Count > 0)
        {
            _repositoryWrapper.HippotherapyLandingPageScientificReferencesRepository.DeleteRange(toDelete);
        }

        long nextPriority = section.ScientificReferences.Count > 0
            ? section.ScientificReferences.Max(r => r.Priority) + 1
            : 1;

        foreach (var referenceDto in dto.ScientificReferences)
        {
            if (referenceDto.Id.HasValue)
            {
                if (!existingById.TryGetValue(referenceDto.Id.Value, out var existingReference))
                {
                    return Result.Fail(ErrorMessagesConstants.NotFound(
                        referenceDto.Id.Value, typeof(HippotherapyLandingPageScientificReference)));
                }

                if (!string.Equals(existingReference.Name, referenceDto.Name, StringComparison.Ordinal))
                {
                    changedReferences.Add(existingReference);
                }

                _mapper.Map(referenceDto, existingReference);
            }
            else
            {
                var newReference = _mapper.Map<HippotherapyLandingPageScientificReference>(referenceDto);
                newReference.CreatedAt = DateTimeOffset.UtcNow;
                newReference.Priority = nextPriority++;
                section.ScientificReferences.Add(newReference);
            }
        }

        return Result.Ok(changedReferences);
    }

    private DAL.Entities.HippotherapyLandingPage BuildNewEntity(UpdateHippotherapyLandingPageDto dto)
    {
        var now = DateTimeOffset.UtcNow;

        var introSection = _mapper.Map<HippotherapyLandingPageIntroSection>(dto.IntroSection);
        introSection.CreatedAt = now;

        var descriptionSection = _mapper.Map<HippotherapyLandingPageDescriptionSection>(dto.DescriptionSection);
        descriptionSection.CreatedAt = now;

        var quoteSection = _mapper.Map<HippotherapyLandingPageQuoteSection>(dto.QuoteSection);
        quoteSection.CreatedAt = now;

        var hippoventionSection = _mapper.Map<HippotherapyLandingPageHippoventionSection>(dto.HippoventionSection);
        hippoventionSection.CreatedAt = now;

        var hippoventionCenterSection = _mapper.Map<HippotherapyLandingPageHippoventionCenterSection>(dto.HippoventionCenterSection);
        hippoventionCenterSection.CreatedAt = now;

        var advantagesSection = _mapper.Map<HippotherapyLandingPageAdvantagesSection>(dto.AdvantagesSection);
        advantagesSection.CreatedAt = now;
        advantagesSection.AdvantageCards = dto.AdvantagesSection.Cards
            .Select((card, index) => new HippotherapyLandingPageAdvantageCard
            {
                Description = card.Description,
                ImageId = card.ImageId,
                Priority = index + 1,
                CreatedAt = now,
            })
            .ToList();

        var analysisSection = _mapper.Map<HippotherapyLandingPageAnalysisSection>(dto.AnalysisSection);
        analysisSection.CreatedAt = now;

        var scientificReferencesSection = _mapper.Map<HippotherapyLandingPageScientificReferencesSection>(dto.ScientificReferencesSection);
        scientificReferencesSection.CreatedAt = now;
        scientificReferencesSection.ScientificReferences = dto.ScientificReferencesSection.ScientificReferences
            .Select((reference, index) => new HippotherapyLandingPageScientificReference
            {
                Name = reference.Name,
                Url = reference.Url,
                Priority = index + 1,
                CreatedAt = now,
            })
            .ToList();

        var anotherQuoteSection = _mapper.Map<HippotherapyLandingPageAnotherQuoteSection>(dto.AnotherQuoteSection);
        anotherQuoteSection.CreatedAt = now;

        var participantsSection = _mapper.Map<HippotherapyLandingPageParticipantsSection>(dto.ParticipantsSection);
        participantsSection.CreatedAt = now;
        participantsSection.ParticipantCards = dto.ParticipantsSection.Cards
            .Select((card, index) => new HippotherapyLandingPageParticipantCard
            {
                Description = card.Description,
                ImageId = card.ImageId,
                Priority = index + 1,
                CreatedAt = now,
            })
            .ToList();

        var ethicsSection = _mapper.Map<HippotherapyLandingPageEthicsSection>(dto.EthicsSection);
        ethicsSection.CreatedAt = now;
        ethicsSection.EthicsPrinciples = dto.EthicsSection.Principles
        .Select((principle, index) => new HippotherapyLandingPageEthicsPrinciple
        {
            Text = principle.Text,
            Priority = index + 1,
            CreatedAt = now,
        })
        .ToList();

        return new DAL.Entities.HippotherapyLandingPage
        {
            CreatedAt = now,
            IntroSection = introSection,
            DescriptionSection = descriptionSection,
            QuoteSection = quoteSection,
            HippoventionSection = hippoventionSection,
            HippoventionCenterSection = hippoventionCenterSection,
            AdvantagesSection = advantagesSection,
            AnalysisSection = analysisSection,
            ScientificReferencesSection = scientificReferencesSection,
            AnotherQuoteSection = anotherQuoteSection,
            ParticipantsSection = participantsSection,
            EthicsSection = ethicsSection,
        };
    }

    private static bool ApplyFinalPriorities(DAL.Entities.HippotherapyLandingPage entity)
    {
        var changed = FinalizePriorities(
            entity.AdvantagesSection?.AdvantageCards,
            c => c.Priority,
            (c, priority) => c.Priority = priority);
        changed |= FinalizePriorities(
            entity.ParticipantsSection?.ParticipantCards,
            c => c.Priority,
            (c, priority) => c.Priority = priority);
        changed |= FinalizePriorities(
            entity.EthicsSection?.EthicsPrinciples,
            p => p.Priority,
            (p, priority) => p.Priority = priority);
        return changed;
    }

    private static bool FinalizePriorities<TItem>(
        IEnumerable<TItem>? items,
        Func<TItem, long> getPriority,
        Action<TItem, long> setPriority)
    {
        var changed = false;
        foreach (var item in items ?? Enumerable.Empty<TItem>())
        {
            var temporary = getPriority(item);
            if (temporary < 0)
            {
                setPriority(item, -temporary);
                changed = true;
            }
        }

        return changed;
    }
}
