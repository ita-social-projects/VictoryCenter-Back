using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.SlugService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
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
