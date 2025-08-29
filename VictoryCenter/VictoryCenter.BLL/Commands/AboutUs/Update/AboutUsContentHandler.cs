using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.AboutUsSectionDto;
using VictoryCenter.BLL.Factories.Payment.Interfaces;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.AboutUsContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.AboutUs.Update;

public class AboutUsContentHandler : IRequestHandler<AboutUsContentCommand, Result<AboutUsSectionDto>>
{
    private IAboutUsContentFactory _factory;
    private IRepositoryWrapper _repository;
    private IMapper _mapper;

    public AboutUsContentHandler(IAboutUsContentFactory factory, IRepositoryWrapper repository, IMapper mapper )
    {
        _factory = factory;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<AboutUsSectionDto>> Handle(AboutUsContentCommand request, CancellationToken cancellationToken)
    {
        // call choose validator method
        foreach (var dto in request.Content)
        {
            var entity = await _repository.AboutUsContentsRepository.GetFirstOrDefaultAsync(new QueryOptions<AboutUsContent>()
            {
                Filter = x => x.Id == dto.Id
            });
            if (entity == null)
            {
                throw new NullReferenceException("good");
            }

            if (entity.SectionId != request.SectionId )
            {
                throw new InvalidCastException("entity didnt belong to this section");
            }

            if (entity.ContentType != dto.ContentType)
            {
                throw new Exception("Wrong Content type");
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
                    _factory.UpdateTitle(dto, entity);
                    break;
            }
        }

        var result = await _repository.AboutUsSectionsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<AboutUsSection>
            {
                Filter = e => e.Id == request.SectionId,
                Include = query => query
                    .Include(section => section.Contents.OfType<CardContent>())
                    .ThenInclude(content => content.Image)
                    .Include(section => section.Contents.OfType<ImageContent>())
                    .ThenInclude(content => content.Image)!
            });

        return Result.Ok(_mapper.Map<AboutUsSectionDto>(result));
    }
}
