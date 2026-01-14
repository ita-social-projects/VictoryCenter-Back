using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.HippotherapyPrograms.GetBySlug;

public class GetHippotherapyProgramBySlugHandler
    : IRequestHandler<GetHippotherapyProgramBySlugQuery, Result<HippotherapyProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHippotherapyProgramBySlugHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<HippotherapyProgramDto>> Handle(
        GetHippotherapyProgramBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HippotherapyProgram>
        {
            Filter = program => program.Slug == request.Slug,
            Include = program => program
                .Include(p => p.Categories)
                .Include(p => p.PreviewImage)!
                .Include(p => p.BackgroundImage)!
                .Include(p => p.Sections)
                    .ThenInclude(s => s.Contents)
        };

        var program = await _repositoryWrapper
            .HippotherapyProgramsRepository
            .GetFirstOrDefaultAsync(queryOptions);

        if (program is null)
        {
            return Result.Fail<HippotherapyProgramDto>(
                ErrorMessagesConstants.NotFound(request.Slug, typeof(HippotherapyProgram)));
        }

        var imageIds = program.Sections
            .SelectMany(s => s.Contents)
            .OfType<ImageProgramContent>()
            .Select(c => c.ImageId);

        var imagesByIdResult = await ImageValidationHelper
            .ValidateAndGetImagesByIdsAsync(_repositoryWrapper, imageIds);

        if (imagesByIdResult.IsFailed)
        {
            return Result.Fail<HippotherapyProgramDto>(imagesByIdResult.Errors);
        }

        var imagesById = imagesByIdResult.Value;

        foreach (var content in program.Sections.SelectMany(s => s.Contents).OfType<ImageProgramContent>())
        {
            content.Image = imagesById[content.ImageId];
        }

        return Result.Ok(_mapper.Map<HippotherapyProgramDto>(program));
    }
}
