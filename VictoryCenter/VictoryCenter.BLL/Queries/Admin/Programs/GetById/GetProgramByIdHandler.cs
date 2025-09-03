using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Programs;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Programs.GetById;

public class GetProgramByIdHandler : IRequestHandler<GetProgramByIdQuery, Result<ProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetProgramByIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<ProgramDto>> Handle(GetProgramByIdQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<Program>
        {
            Filter = program => program.Id == request.Id,
            Include = program => program
                .Include(p => p.Categories)
                .Include(p => p.Image)!
        };

        Program? program = await _repositoryWrapper.ProgramsRepository.GetFirstOrDefaultAsync(queryOptions);

        if (program is null)
        {
            return Result.Fail<ProgramDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(Program)));
        }

        ProgramDto responseDto = _mapper.Map<ProgramDto>(program);
        return Result.Ok(responseDto);
    }
}
