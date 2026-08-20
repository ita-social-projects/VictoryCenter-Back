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
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyLandingPage.Update;

public class UpdateHippotherapyLandingPageHandler : IRequestHandler<UpdateHippotherapyLandingPageCommand, Result<HippotherapyLandingPageDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateHippotherapyLandingPageCommand> _validator;
    private readonly IReorderService _reorderService;

    public UpdateHippotherapyLandingPageHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<UpdateHippotherapyLandingPageCommand> validator,
        IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
        _reorderService = reorderService;
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

                    _mapper.Map(dto.DescriptionSection, entity.DescriptionSection);

                    TrackImageChange(entity.QuoteSection!.ImageId, dto.QuoteSection.ImageId, imageIdsToDelete);
                    _mapper.Map(dto.QuoteSection, entity.QuoteSection);

                    _mapper.Map(dto.HippoventionSection, entity.HippoventionSection);

                    TrackImageChange(entity.HippoventionCenterSection!.ImageId, dto.HippoventionCenterSection.ImageId, imageIdsToDelete);
                    _mapper.Map(dto.HippoventionCenterSection, entity.HippoventionCenterSection);

                    _mapper.Map(dto.AdvantagesSection, entity.AdvantagesSection);
                    UpdateGalleryCards(entity.AdvantagesSection!.AdvantageCards, dto.AdvantagesSection.Cards, imageIdsToDelete);

                    _mapper.Map(dto.AnalysisSection, entity.AnalysisSection);

                    _mapper.Map(dto.ScientificReferencesSection, entity.ScientificReferencesSection);
                    var referencesResult = UpdateScientificReferences(entity.ScientificReferencesSection!, dto.ScientificReferencesSection);
                    if (referencesResult.IsFailed)
                    {
                        return Result.Fail<HippotherapyLandingPageDto>(referencesResult.Errors);
                    }

                    TrackImageChange(entity.AnotherQuoteSection!.ImageId, dto.AnotherQuoteSection.ImageId, imageIdsToDelete);
                    _mapper.Map(dto.AnotherQuoteSection, entity.AnotherQuoteSection);

                    _mapper.Map(dto.ParticipantsSection, entity.ParticipantsSection);
                    UpdateGalleryCards(entity.ParticipantsSection!.ParticipantCards, dto.ParticipantsSection.Cards, imageIdsToDelete);

                    TrackImageChange(entity.EthicsSection!.ImageId, dto.EthicsSection.ImageId, imageIdsToDelete);
                    _mapper.Map(dto.EthicsSection, entity.EthicsSection);
                    UpdateEthicsPrinciples(entity.EthicsSection.EthicsPrinciples, dto.EthicsSection.Principles);

                    _repositoryWrapper.HippotherapyLandingPagesRepository.Update(entity);
                }

                await _repositoryWrapper.SaveChangesAsync();

                sectionIdForReorder = entity.ScientificReferencesSection!.Id;
                await _reorderService.RenumberPriorityAsync<HippotherapyLandingPageScientificReference>(
                    groupSelector: r => r.ScientificReferencesSectionId == sectionIdForReorder);

                var idsToActuallyDelete = imageIdsToDelete.Where(id => !requestedImageIds.Contains(id)).ToList();
                if (idsToActuallyDelete.Count > 0)
                {
                    var imagesToDelete = await _repositoryWrapper.ImageRepository.GetAllAsync(new QueryOptions<Image>
                    {
                        Filter = i => idsToActuallyDelete.Contains(i.Id),
                    });
                    _repositoryWrapper.ImageRepository.DeleteRange(imagesToDelete);
                    await _repositoryWrapper.SaveChangesAsync();
                }

                scope.Complete();
            }

            var result = await _repositoryWrapper.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(new QueryOptions<DAL.Entities.HippotherapyLandingPage>
            {
                Include = HippotherapyLandingPageIncludeHelper.IncludeFullGraph,
            });

            return Result.Ok(_mapper.Map<HippotherapyLandingPageDto>(result));
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

    private static void TrackImageChange(long? oldImageId, long? newImageId, List<long> imageIdsToDelete)
    {
        if (oldImageId.HasValue && oldImageId != newImageId)
        {
            imageIdsToDelete.Add(oldImageId.Value);
        }
    }

    private static void UpdateEthicsPrinciples(ICollection<HippotherapyLandingPageEthicsPrinciple> existing, List<string> incoming)
    {
        var existingList = existing.OrderBy(p => p.Priority).ToList();
        for (var i = 0; i < existingList.Count && i < incoming.Count; i++)
        {
            existingList[i].Text = incoming[i];
        }
    }

    private static void UpdateGalleryCards<TCard>(
        ICollection<TCard> existing,
        List<UpdateGalleryCardDto> incoming,
        List<long> imageIdsToDelete)
        where TCard : IGalleryCard
    {
        var existingList = existing.OrderBy(c => c.Priority).ToList();
        for (var i = 0; i < existingList.Count && i < incoming.Count; i++)
        {
            var card = existingList[i];
            var cardDto = incoming[i];
            TrackImageChange(card.ImageId, cardDto.ImageId, imageIdsToDelete);
            card.ImageId = cardDto.ImageId;
            card.Description = cardDto.Description;
        }
    }

    private Result UpdateScientificReferences(
        HippotherapyLandingPageScientificReferencesSection section,
        UpdateScientificReferencesSectionDto dto)
    {
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

        return Result.Ok();
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
            .Select((text, index) => new HippotherapyLandingPageEthicsPrinciple { Text = text, Priority = index + 1, CreatedAt = now })
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
}
