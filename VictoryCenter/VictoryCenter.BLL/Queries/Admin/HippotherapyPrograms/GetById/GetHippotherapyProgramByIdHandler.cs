using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.GetById;

public class GetHippotherapyProgramByIdHandler
    : IRequestHandler<GetHippotherapyProgramByIdQuery, Result<HippotherapyProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHippotherapyProgramByIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<HippotherapyProgramDto>> Handle(
        GetHippotherapyProgramByIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HippotherapyProgram>
        {
            Filter = program => program.Id == request.Id,
            Include = program => program
                .Include(p => p.Categories)
                .Include(p => p.PreviewImage)!
                .Include(p => p.BackgroundImage)!
                .Include(p => p.Sections)
                    .ThenInclude(s => s.Contents)
                        .ThenInclude(c => c.Localizations)
                            .ThenInclude(l => l.Language)
                .Include(p => p.Localizations)
                    .ThenInclude(l => l.Language)
        };

        var program = await _repositoryWrapper
            .HippotherapyProgramsRepository
            .GetFirstOrDefaultAsync(queryOptions);

        if (program is null)
        {
            return Result.Fail<HippotherapyProgramDto>(
                ErrorMessagesConstants.NotFound(request.Id, typeof(HippotherapyProgram)));
        }

        var assignImagesResult = await ImageValidationHelper
            .ValidateAndAssignSectionContentImagesAsync(_repositoryWrapper, program.Sections);

        if (assignImagesResult.IsFailed)
        {
            return Result.Fail<HippotherapyProgramDto>(assignImagesResult.Errors);
        }

        var assignFaqQuestionsResult = await FaqQuestionHelper
            .AssignSectionContentFaqQuestionsAsync(_repositoryWrapper, program.Sections);

        if (assignFaqQuestionsResult.IsFailed)
        {
            return Result.Fail<HippotherapyProgramDto>(assignFaqQuestionsResult.Errors);
        }

        return Result.Ok(_mapper.Map<HippotherapyProgramDto>(program));
    }
}
