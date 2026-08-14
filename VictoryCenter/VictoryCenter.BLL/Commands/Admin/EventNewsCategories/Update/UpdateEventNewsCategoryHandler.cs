using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Update;

public class UpdateEventNewsCategoryHandler
    : IRequestHandler<UpdateEventNewsCategoryCommand, Result<AdminEventNewsCategoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public UpdateEventNewsCategoryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<AdminEventNewsCategoryDto>> Handle(
        UpdateEventNewsCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _repositoryWrapper.EventNewsCategoryRepository.GetFirstOrDefaultAsync(
            new QueryOptions<EventNewsCategory>
            {
                Filter = entity => entity.Id == request.Id,
                Include = query => query
                    .Include(entity => entity.Localizations)
                    .ThenInclude(localization => localization.Language),
                AsNoTracking = false
            });

        if (category is null)
        {
            return Result.Fail<AdminEventNewsCategoryDto>(
                ErrorMessagesConstants.NotFound(request.Id, typeof(EventNewsCategory)));
        }

        var normalizedName = request.Category.Name.Trim();
        if (await _repositoryWrapper.EventNewsCategoryRepository.ExistsAsync(
                entity => entity.Id != request.Id && entity.Name == normalizedName))
        {
            return Result.Fail<AdminEventNewsCategoryDto>(EventNewsCategoryConstants.DuplicateCategoryName);
        }

        if (string.Equals(category.Name, normalizedName, StringComparison.Ordinal))
        {
            return Result.Ok(_mapper.Map<AdminEventNewsCategoryDto>(category));
        }

        foreach (var localization in category.Localizations)
        {
            localization.TranslationStatus = TranslationStatus.Outdated;
        }

        category.Name = normalizedName;

        try
        {
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<AdminEventNewsCategoryDto>(category));
            }
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintException())
        {
            return Result.Fail<AdminEventNewsCategoryDto>(EventNewsCategoryConstants.DuplicateCategoryName);
        }

        return Result.Fail<AdminEventNewsCategoryDto>(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(EventNewsCategory)));
    }
}
