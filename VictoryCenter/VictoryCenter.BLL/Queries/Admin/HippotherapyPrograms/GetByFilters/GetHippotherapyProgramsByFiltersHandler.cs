using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.GetByFilters;

public class GetHippotherapyProgramsByFiltersHandler : IRequestHandler<GetHippotherapyProgramsByFiltersQuery, Result<PaginationResult<HippotherapyProgramDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHippotherapyProgramsByFiltersHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PaginationResult<HippotherapyProgramDto>>> Handle(GetHippotherapyProgramsByFiltersQuery request, CancellationToken cancellationToken)
    {
        Status? status = request.RequestDto?.Status;
        List<long>? programCategories = request.RequestDto?.CategoryId;
        Expression<Func<HippotherapyProgram, bool>> filter =
            t => (status == null || t.Status == status) &&
                 (programCategories == null || programCategories.Count == 0 ||
                  t.Categories.Any(c => programCategories.Contains(c.Id)));

        var queryOptions = new QueryOptions<HippotherapyProgram>
        {
            Offset = request.RequestDto?.Offset is > 0 ? (int)request.RequestDto.Offset : 0,
            Limit = request.RequestDto?.Limit is > 0 ? (int)request.RequestDto.Limit : 0,
            Filter = filter,
            Include = program => program
                .Include(p => p.PreviewImage)
                .Include(p => p.Categories)
        };

        IEnumerable<HippotherapyProgram> programs = await _repositoryWrapper.HippotherapyProgramsRepository.GetAllAsync(queryOptions);
        var totalCount = await _repositoryWrapper.HippotherapyProgramsRepository.CountAsync(queryOptions with { Offset = 0, Limit = 0 });
        var programDto = _mapper.Map<IEnumerable<HippotherapyProgramDto>>(programs).ToList();

        return Result.Ok(new PaginationResult<HippotherapyProgramDto>([.. programDto], totalCount));
    }
}
