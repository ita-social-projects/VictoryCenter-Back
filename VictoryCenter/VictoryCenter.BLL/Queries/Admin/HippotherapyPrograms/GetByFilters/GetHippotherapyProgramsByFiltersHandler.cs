using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Enums;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
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
        var languageCount = await _repositoryWrapper.LocalizationLanguagesRepository.CountAsync();
        Status? status = request.RequestDto?.Status;
        List<long>? programCategories = request.RequestDto?.CategoryId;
        var translationStatusFilter = request.RequestDto?.TranslationStatusFilter;
        Expression<Func<HippotherapyProgram, bool>> filter =
            t => (status == null || t.Status == status) &&
                 (programCategories == null || programCategories.Count == 0 ||
                  t.Categories.Any(c => programCategories.Contains(c.Id))) &&
            (translationStatusFilter == null ||
            translationStatusFilter == TranslationStatusFilter.All ||
            (translationStatusFilter == TranslationStatusFilter.Outdated &&
            t.Localizations.Any(l => l.TranslationStatus == TranslationStatus.Outdated)) ||
            (translationStatusFilter == TranslationStatusFilter.Missing &&
            t.Localizations.Count < languageCount));

        var queryOptions = new QueryOptions<HippotherapyProgram>
        {
            Offset = request.RequestDto?.Offset is > 0 ? (int)request.RequestDto.Offset : 0,
            Limit = request.RequestDto?.Limit is > 0 ? (int)request.RequestDto.Limit : 0,
            Filter = filter,
            Include = program => program
                .Include(p => p.BackgroundImage)
                .Include(p => p.PreviewImage)
                .Include(p => p.Categories)
                .Include(p => p.Sections)
                    .ThenInclude(s => s.Contents)
                        .ThenInclude(c => c.Localizations)
                            .ThenInclude(l => l.Language)
                .Include(p => p.Localizations)
                    .ThenInclude(l => l.Language)
        };

        IEnumerable<HippotherapyProgram> programs = await _repositoryWrapper.HippotherapyProgramsRepository.GetAllAsync(queryOptions);
        var totalCount = await _repositoryWrapper.HippotherapyProgramsRepository.CountAsync(queryOptions with { Offset = 0, Limit = 0 });

        var allImageIds = programs
            .SelectMany(p => p.Sections)
            .SelectMany(s => s.Contents)
            .OfType<ImageProgramContent>()
            .Select(c => c.ImageId)
            .Distinct();

        var imagesByIdResult = await ImageValidationHelper
            .ValidateAndGetImagesByIdsAsync(_repositoryWrapper, allImageIds);

        if (imagesByIdResult.IsFailed)
        {
            return Result.Fail<PaginationResult<HippotherapyProgramDto>>(imagesByIdResult.Errors);
        }

        var imagesById = imagesByIdResult.Value;

        foreach (var program in programs)
        {
            foreach (var content in program.Sections.SelectMany(s => s.Contents).OfType<ImageProgramContent>()
                         .Where(c => c.ImageId > 0))
            {
                content.Image = imagesById[content.ImageId];
            }
        }

        var assignFaqQuestionsResult = await FaqQuestionHelper
            .AssignSectionContentFaqQuestionsAsync(_repositoryWrapper, programs.SelectMany(p => p.Sections));

        if (assignFaqQuestionsResult.IsFailed)
        {
            return Result.Fail<PaginationResult<HippotherapyProgramDto>>(assignFaqQuestionsResult.Errors);
        }

        var programDto = _mapper.Map<IEnumerable<HippotherapyProgramDto>>(programs).ToList();

        return Result.Ok(new PaginationResult<HippotherapyProgramDto>([.. programDto], totalCount));
    }
}
