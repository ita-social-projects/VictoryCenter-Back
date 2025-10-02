using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.HypotherapyPrograms.GetByFilters;

public class GetHypotherapyProgramsByFiltersHandler : IRequestHandler<GetHypotherapyProgramsByFiltersQuery, Result<PaginationResult<HypotherapyProgramDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHypotherapyProgramsByFiltersHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PaginationResult<HypotherapyProgramDto>>> Handle(GetHypotherapyProgramsByFiltersQuery request, CancellationToken cancellationToken)
    {
        Status? status = request.RequestDto?.Status;
        List<long>? programCategories = request.RequestDto?.CategoryId;
        Expression<Func<HypotherapyProgram, bool>> filter =
            t => (status == null || t.Status == status) &&
                 (programCategories == null || programCategories.Count == 0 ||
                  t.Categories.Any(c => programCategories.Contains(c.Id)));

        var queryOptions = new QueryOptions<HypotherapyProgram>
        {
            Offset = request.RequestDto?.Offset is > 0 ? (int)request.RequestDto.Offset : 0,
            Limit = request.RequestDto?.Limit is > 0 ? (int)request.RequestDto.Limit : 0,
            Filter = filter,
            Include = program => program
                .Include(p => p.Image)
                .Include(p => p.Categories)
        };

        IEnumerable<HypotherapyProgram> programs = await _repositoryWrapper.HypotherapyProgramsRepository.GetAllAsync(queryOptions);
        var totalCount = await _repositoryWrapper.HypotherapyProgramsRepository.CountAsync(queryOptions with { Offset = 0, Limit = 0 });
        var programDto = _mapper.Map<IEnumerable<HypotherapyProgramDto>>(programs).ToList();

        return Result.Ok(new PaginationResult<HypotherapyProgramDto>([.. programDto], totalCount));
    }
}
