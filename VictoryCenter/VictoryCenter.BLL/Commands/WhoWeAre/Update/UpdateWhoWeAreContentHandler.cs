using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.WhoWeAreSection;
using VictoryCenter.BLL.Factories.Payment.Interfaces;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.WhoWeAre.Update;

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

            foreach (var dto in request.Content)
            {
                var entity = await _repository.WhoWeAreContentsRepository.GetFirstOrDefaultAsync(
                    new QueryOptions<WhoWeAreContent>()
                    {
                        Include = x => x.Include(w => w.Section),
                        Filter = x => x.Id == dto.Id
                    });

                if (entity == null)
                {
                    return Result.Fail($"Content was not found");
                }

                if (entity.Section.SectionType != request.SectionType)
                {
                    return Result.Fail("Entity didnt belong to this section");
                }

                if (entity.ContentType != dto.ContentType)
                {
                    return Result.Fail("Wrong Content type");
                }

                switch (dto.ContentType)
                {
                    case ContentType.Description:
                        _factory.UpdateDescription(dto, entity);
                        break;

                    case ContentType.Card:
                        _factory.UpdateCard(dto, entity);
                        break;

                    case ContentType.Title:
                        _factory.UpdateTitle(dto, entity);
                        break;

                    case ContentType.Image:
                        _factory.UpdateImage(dto, entity);
                        break;
                }

                _repository.WhoWeAreContentsRepository.Update(entity);
            }

            await _repository.SaveChangesAsync();
            var result = await _repository.WhoWeAreSectionsRepository.GetFirstOrDefaultAsync(
                new QueryOptions<WhoWeAreSection>
                {
                    Filter = e => e.SectionType == request.SectionType,
                    Include = s => s
                        .Include(sec => sec.Contents)
                        .ThenInclude(c => (c as ImageContent)!.Image)
                        .Include(sec => sec.Contents)
                        .ThenInclude(c => (c as CardContent)!.Image)!
                });

            return Result.Ok(_mapper.Map<WhoWeAreSectionDto>(result));
        }
        catch (ValidationException vex)
        {
            return Result.Fail(vex.Errors.Select(x => x.ErrorMessage));
        }
    }
}
