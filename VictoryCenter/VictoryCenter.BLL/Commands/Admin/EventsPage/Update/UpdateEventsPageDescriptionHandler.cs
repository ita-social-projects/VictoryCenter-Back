using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.EventsPage.Update;

public class UpdateEventsPageDescriptionHandler : IRequestHandler<UpdateEventsPageDescriptionCommand, Result<EventsIntroSectionDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public UpdateEventsPageDescriptionHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<EventsIntroSectionDto>> Handle(UpdateEventsPageDescriptionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repositoryWrapper.EventsIntroSectionsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<EventsIntroSection>
            {
                Include = q => q.Include(e => e.Localizations),
                AsNoTracking = false,
            });
        if (entity == null)
        {
            return Result.Fail<EventsIntroSectionDto>(ErrorMessagesConstants.NotFound());
        }

        entity.PageDescription = HtmlContentHelper.NormalizeHtmlContent(request.Dto.PageDescription);
        MarkLocalizationsOutdated(entity);
        await _repositoryWrapper.SaveChangesAsync();

        return Result.Ok(_mapper.Map<EventsIntroSectionDto>(entity));
    }

    private static void MarkLocalizationsOutdated(EventsIntroSection entity)
    {
        foreach (var localization in entity.Localizations)
        {
            localization.TranslationStatus = TranslationStatus.Outdated;
        }
    }
}
