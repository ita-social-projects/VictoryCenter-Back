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
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var dictEntities = await GetContentMappedToDictionary(request.Contents);
            var sectionId = await GetSectionIdByType(request.SectionType) ??
                            throw new ArgumentException(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(request.SectionType)));

            foreach (var dto in request.Contents)
            {
                if (!dictEntities.TryGetValue(dto.Id, out var entity))
                {
                    return Result.Fail(ErrorMessagesConstants.NotFound(dto.Id, typeof(WhoWeAreContent)));
                }

                if (entity.SectionId != sectionId)
                {
                    return Result.Fail(WhoWeAreConstants.EntityDoesNotBelongToTheSection(typeof(WhoWeAreContent), sectionId));
                }

                if (entity.ContentType != dto.ContentType)
                {
                    return Result.Fail(WhoWeAreConstants.WrongContentType);
                }

                UpdateContent(dto, entity);
            }

            await _repository.SaveChangesAsync();

            var updatedSection = await GetSection(request.SectionType);
            return Result.Ok(_mapper.Map<WhoWeAreSectionDto>(updatedSection));
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

    private async Task<Dictionary<long, WhoWeAreContent>> GetContentMappedToDictionary(List<CreateWhoWeAreContentDto> content)
    {
        var contentIds = content.Select(x => x.Id).ToList();

        var entities = await _repository.WhoWeAreContentsRepository.GetAllAsync(new QueryOptions<WhoWeAreContent>
        {
            Filter = w => contentIds.Contains(w.Id)
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
            });
    }

    private void UpdateContent(CreateWhoWeAreContentDto contentDto, WhoWeAreContent entity)
    {
        switch (contentDto.ContentType)
        {
            case ContentType.Description:
                _factory.UpdateDescription(contentDto, entity);
                break;

            case ContentType.Card:
                _factory.UpdateCard(contentDto, entity);
                break;

            case ContentType.Title:
                _factory.UpdateTitle(contentDto, entity);
                break;

            case ContentType.Image:
                _factory.UpdateImage(contentDto, entity);
                break;
        }

        _repository.WhoWeAreContentsRepository.Update(entity);
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
