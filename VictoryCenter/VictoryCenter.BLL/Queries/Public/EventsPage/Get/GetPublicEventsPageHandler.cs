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
        var intro = await _repositoryWrapper.EventsIntroSectionsRepository.GetFirstOrDefaultAsync();
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
            EventsBlockTitle = introDto.EventsBlockTitle,
            PageDescription = introDto.PageDescription,
            Categories = _mapper.Map<List<EventNewsCategoryDto>>(categories)
        });
    }
}
