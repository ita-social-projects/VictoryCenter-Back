using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.BLL.DTOs.Public.EventsPage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.EventsPage.Get;

public class GetPublicEventsPageHandler : IRequestHandler<GetPublicEventsPageQuery, Result<PublicEventsPageDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetPublicEventsPageHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<PublicEventsPageDto>> Handle(
        GetPublicEventsPageQuery request,
        CancellationToken cancellationToken)
    {
        var intro = await _repositoryWrapper.EventsIntroSectionsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<EventsIntroSection>
            {
                Include = query => query.Include(section => section.Localizations)
                    .ThenInclude(localization => localization.Language),
                AsNoTracking = true,
            });

        if (intro == null)
        {
            return Result.Fail<PublicEventsPageDto>(ErrorMessagesConstants.NotFound());
        }

        var categories = await _repositoryWrapper.EventNewsCategoryRepository.GetAllAsync(
            new QueryOptions<EventNewsCategory>
            {
                Include = query => query
                    .Include(category => category.Localizations)
                    .ThenInclude(localization => localization.Language),
                OrderByASC = category => category.Name,
                AsNoTracking = true
            });

        var introDto = _mapper.Map<EventsIntroSectionDto>(intro);

        return Result.Ok(new PublicEventsPageDto
        {
            EventsBlockTitle = introDto.IsEventsBlockTitleHidden ? string.Empty : introDto.EventsBlockTitle,
            PageDescription = introDto.IsPageDescriptionHidden ? string.Empty : introDto.PageDescription,
            IsEventsBlockTitleHidden = introDto.IsEventsBlockTitleHidden,
            IsPageDescriptionHidden = introDto.IsPageDescriptionHidden,
            Localizations = _mapper.Map<List<PublicEventsIntroSectionLocalizationDto>>(intro.Localizations),
            Categories = _mapper.Map<List<EventNewsCategoryDto>>(categories)
        });
    }
}
