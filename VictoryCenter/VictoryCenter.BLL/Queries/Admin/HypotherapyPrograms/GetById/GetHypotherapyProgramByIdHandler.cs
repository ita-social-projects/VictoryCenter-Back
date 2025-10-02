using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.HypotherapyPrograms.GetById;

public class GetHypotherapyProgramByIdHandler : IRequestHandler<GetHypotherapyProgramByIdQuery, Result<HypotherapyProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHypotherapyProgramByIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<HypotherapyProgramDto>> Handle(GetHypotherapyProgramByIdQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HypotherapyProgram>
        {
            Filter = program => program.Id == request.Id,
            Include = program => program
                .Include(p => p.Categories)
                .Include(p => p.Image)!
        };

        HypotherapyProgram? program = await _repositoryWrapper.HypotherapyProgramsRepository.GetFirstOrDefaultAsync(queryOptions);

        if (program is null)
        {
            return Result.Fail<HypotherapyProgramDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(HypotherapyProgram)));
        }

        HypotherapyProgramDto responseDto = _mapper.Map<HypotherapyProgramDto>(program);
        return Result.Ok(responseDto);
    }
}
