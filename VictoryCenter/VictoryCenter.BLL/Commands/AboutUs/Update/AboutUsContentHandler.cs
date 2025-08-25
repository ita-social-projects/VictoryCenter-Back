using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.AboutUsContent;
using VictoryCenter.BLL.Factories.Payment.Interfaces;
using VictoryCenter.DAL.Entities.AboutUsContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.AboutUs.Update;

public class AboutUsContentHandler : IRequestHandler<AboutUsContentCommand, Result<AboutUsSectionDto>>
{
    private IAboutUsContentFactory _factory;
    private IRepositoryWrapper _repository;

    public AboutUsContentHandler(IAboutUsContentFactory factory, IRepositoryWrapper repository)
    {
        _factory = factory;
        _repository = repository;
    }

    public async Task<Result<AboutUsSectionDto>> Handle(AboutUsContentCommand request, CancellationToken cancellationToken)
    {
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
    }
}
