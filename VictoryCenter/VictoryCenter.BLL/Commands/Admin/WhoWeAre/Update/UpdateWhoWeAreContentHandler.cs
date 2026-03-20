using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.BLL.Interfaces.WhoWeAreContentFactory;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.WhoWeAre.Update;

public class UpdateWhoWeAreContentHandler : IRequestHandler<UpdateWhoWeAreContentCommand, Result<WhoWeAreSectionDto>>
{
    private readonly IWhoWeAreContentFactory _factory;
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateWhoWeAreContentCommand> _validator;

    public UpdateWhoWeAreContentHandler(IWhoWeAreContentFactory factory, IRepositoryWrapper repository, IMapper mapper, IValidator<UpdateWhoWeAreContentCommand> validator)
    {
        _factory = factory;
        _repository = repository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<WhoWeAreSectionDto>> Handle(UpdateWhoWeAreContentCommand request, CancellationToken cancellationToken)
    {
        var imageIdsToDelete = new List<long>();
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var dictEntities = await GetContentMappedToDictionary(request.Contents);
            var sectionId = await GetSectionIdByType(request.SectionType) ??
                            throw new ArgumentException(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(request.SectionType)));

            using (TransactionScope scope = _repository.BeginTransaction())
            {
                foreach (var dto in request.Contents)
                {
                    if (!dictEntities.TryGetValue(dto.Id, out var entity))
                    {
                        return Result.Fail(ErrorMessagesConstants.NotFound(dto.Id, typeof(WhoWeAreContent)));
                    }

                    if (entity.SectionId != sectionId)
                    {
                        return Result.Fail(WhoWeAreConstants.EntityDoesNotBelongToTheSection(typeof(WhoWeAreContent), request.SectionType));
                    }

                    if (entity.ContentType != dto.ContentType)
                    {
                        return Result.Fail(WhoWeAreConstants.DtoHasWrongContentType(dto.Id, entity.ContentType, dto.ContentType));
                    }

                    UpdateContent(dto, entity, imageIdsToDelete);
                }

                await _repository.SaveChangesAsync();

                if (imageIdsToDelete.Count > 0)
                {
                    var imagesToDelete = await _repository.ImageRepository.GetAllAsync(new QueryOptions<Image>()
                    {
                        Filter = image => imageIdsToDelete.Contains(image.Id)
                    });

                    _repository.ImageRepository.DeleteRange(imagesToDelete);
                    await _repository.SaveChangesAsync();
                }

                var updatedSection = await GetSection(request.SectionType);

                if (updatedSection == null)
                {
                    return Result.Fail(ErrorMessagesConstants.NotFoundByIdentifier(request.SectionType, typeof(WhoWeAreSection)));
                }

                scope.Complete();

                return Result.Ok(_mapper.Map<WhoWeAreSectionDto>(updatedSection));
            }
        }
        catch (ValidationException vex)
        {
            return Result.Fail(vex.Errors.Select(x => x.ErrorMessage));
        }
        catch (ArgumentException e)
        {
            return Result.Fail(e.Message);
        }
        catch (DbUpdateException)
        {
            return Result.Fail(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(WhoWeAreContent)));
        }
    }

    private async Task<Dictionary<long, WhoWeAreContent>> GetContentMappedToDictionary(List<UpdateWhoWeAreContentDto> content)
    {
        var contentIds = content.Select(x => x.Id).ToList();

        var entities = await _repository.WhoWeAreContentsRepository.GetAllAsync(new QueryOptions<WhoWeAreContent>
        {
            AsNoTracking = false,
            Filter = w => contentIds.Contains(w.Id),
            Include = w => w.Include(w => w.Localizations).ThenInclude(l => l.Language)
        });

        return entities.ToDictionary(x => x.Id, x => x);
    }

    private async Task<WhoWeAreSection?> GetSection(SectionType sectionType)
    {
        return await _repository.WhoWeAreSectionsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<WhoWeAreSection>
            {
                Filter = e => e.SectionType == sectionType,
                Include = s => s
                    .Include(sec => sec.Contents)
                    .ThenInclude(c => (c as ImageContent)!.Image)
                    .Include(sec => sec.Contents)
                    .ThenInclude(c => (c as CardContent)!.Image)!
                    .Include(sec => sec.Contents)
                    .ThenInclude(c => c.Localizations)
                    .ThenInclude(l => l.Language)
            });
    }

    private void UpdateContent(UpdateWhoWeAreContentDto contentDto, WhoWeAreContent entity, List<long> imageIdsToDelete)
    {
        bool isContentChanged = false;

        switch (contentDto.ContentType)
        {
            case ContentType.Description:
                var descContent = entity as DescriptionContent
                                  ?? throw new InvalidOperationException(WhoWeAreConstants.EntityIsNotRightContent(typeof(DescriptionContent)));

                isContentChanged = !string.Equals(contentDto.Description, descContent.Description);
                _factory.UpdateDescription(contentDto, entity);
                break;

            case ContentType.Title:
                var titleContent = entity as TitleContent
                                   ?? throw new InvalidOperationException(WhoWeAreConstants.EntityIsNotRightContent(typeof(TitleContent)));

                isContentChanged = !string.Equals(contentDto.Title, titleContent.Title);
                _factory.UpdateTitle(contentDto, entity);
                break;

            case ContentType.Card:
                var cardContent = entity as CardContent
                                  ?? throw new InvalidOperationException(WhoWeAreConstants.EntityIsNotRightContent(typeof(CardContent)));

                isContentChanged = !string.Equals(contentDto.Description, cardContent.Description);

                if (cardContent.ImageId != null && cardContent.ImageId != contentDto.ImageId)
                {
                    imageIdsToDelete.Add(cardContent.ImageId!.Value);
                }

                _factory.UpdateCard(contentDto, entity);
                break;

            case ContentType.Image:
                var imageContent = entity as ImageContent
                          ?? throw new InvalidOperationException(WhoWeAreConstants.EntityIsNotRightContent(typeof(ImageContent)));

                if (imageContent.ImageId != null && imageContent.ImageId != contentDto.ImageId)
                {
                    imageIdsToDelete.Add(imageContent.ImageId!.Value);
                }

                _factory.UpdateImage(contentDto, entity);
                break;
        }

        if (isContentChanged)
        {
            foreach (var loc in entity.Localizations)
            {
                loc.TranslationStatus = TranslationStatus.Outdated;
            }
        }
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
}
