using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.HippotherapyLandingPage.Get;

public class GetHippotherapyLandingPageHandler : IRequestHandler<GetHippotherapyLandingPageQuery, Result<HippotherapyLandingPageDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetHippotherapyLandingPageHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<HippotherapyLandingPageDto>> Handle(GetHippotherapyLandingPageQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repositoryWrapper.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(new QueryOptions<DAL.Entities.HippotherapyLandingPage>
        {
            Include = q => q
                .Include(e => e.IntroSection).ThenInclude(s => s!.Image)
                .Include(e => e.DescriptionSection)
                .Include(e => e.QuoteSection).ThenInclude(s => s!.Image)
                .Include(e => e.HippoventionSection)
                .Include(e => e.HippoventionCenterSection).ThenInclude(s => s!.Image)
                .Include(e => e.HippoventionCenterSection).ThenInclude(s => s!.HippoventionPros.OrderBy(p => p.Priority))
                .Include(e => e.AdvantagesSection).ThenInclude(s => s!.AdvantageCards.OrderBy(c => c.Priority)).ThenInclude(c => c.Image)
                .Include(e => e.AnalysisSection)
                .Include(e => e.ScientificReferencesSection).ThenInclude(s => s!.ScientificReferences.OrderBy(r => r.Priority))
                .Include(e => e.AnotherQuoteSection).ThenInclude(s => s!.Image)
                .Include(e => e.ParticipantsSection).ThenInclude(s => s!.ParticipantCards.OrderBy(c => c.Priority)).ThenInclude(c => c.Image)
                .Include(e => e.EthicsSection).ThenInclude(s => s!.Image)
                .Include(e => e.EthicsSection).ThenInclude(s => s!.EthicsPrinciples.OrderBy(p => p.Priority)),
        });

        if (entity == null)
        {
            return Result.Fail<HippotherapyLandingPageDto>(ErrorMessagesConstants.NotFound());
        }

        return Result.Ok(_mapper.Map<HippotherapyLandingPageDto>(entity));
    }
}
