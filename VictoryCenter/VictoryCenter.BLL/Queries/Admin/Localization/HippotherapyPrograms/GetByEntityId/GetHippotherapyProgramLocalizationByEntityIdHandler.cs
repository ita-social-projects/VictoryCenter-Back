using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyPrograms.GetByEntityId;

public class GetHippotherapyProgramLocalizationByEntityIdHandler : IRequestHandler<GetHippotherapyProgramLocalizationByEntityIdQuery, Result<IEnumerable<HippotherapyProgramLocalizationDto>>>
{
    private readonly IRepositoryWrapper _wrapper;
    private readonly IMapper _mapper;

    public GetHippotherapyProgramLocalizationByEntityIdHandler(IRepositoryWrapper wrapper, IMapper mapper)
    {
        _wrapper = wrapper;
        _mapper = mapper;
    }

    public Task<Result<IEnumerable<HippotherapyProgramLocalizationDto>>> Handle(GetHippotherapyProgramLocalizationByEntityIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
