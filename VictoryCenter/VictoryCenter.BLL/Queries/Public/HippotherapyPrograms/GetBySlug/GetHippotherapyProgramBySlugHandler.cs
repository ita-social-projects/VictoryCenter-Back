using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.SlugService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Queries.Public.HippotherapyPrograms.GetBySlug;

public class GetHippotherapyProgramBySlugHandler
    : IRequestHandler<GetHippotherapyProgramBySlugQuery, Result<HippotherapyProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ISlugService _slugService;

    public GetHippotherapyProgramBySlugHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ISlugService slugService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _slugService = slugService;
    }

    public async Task<Result<HippotherapyProgramDto>> Handle(
        GetHippotherapyProgramBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var program = await _slugService.GetHippotherapyProgramBySlugAsync(request.Slug, cancellationToken);

        if (program is null || program.Status != Status.Published)
        {
            return Result.Fail<HippotherapyProgramDto>(
                ErrorMessagesConstants.NotFound(request.Slug, typeof(HippotherapyProgram)));
        }

        var assignImagesResult = await ImageValidationHelper
            .ValidateAndAssignSectionContentImagesAsync(_repositoryWrapper, program.Sections);

        if (assignImagesResult.IsFailed)
        {
            return Result.Fail<HippotherapyProgramDto>(assignImagesResult.Errors);
        }

        return Result.Ok(_mapper.Map<HippotherapyProgramDto>(program));
    }
}
