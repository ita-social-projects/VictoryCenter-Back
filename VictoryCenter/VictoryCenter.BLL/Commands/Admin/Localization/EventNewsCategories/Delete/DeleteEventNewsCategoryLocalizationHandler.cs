using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Delete;

public class DeleteEventNewsCategoryLocalizationHandler
    : IRequestHandler<DeleteEventNewsCategoryLocalizationCommand, Result<DeleteEventNewsCategoryLocalizationDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteEventNewsCategoryLocalizationHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<DeleteEventNewsCategoryLocalizationDto>> Handle(
        DeleteEventNewsCategoryLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        var localization = await _repositoryWrapper.EventNewsCategoryLocalizationsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<EventNewsCategoryLocalization>
            {
                Filter = entity => entity.EntityId == request.EntityId && entity.LanguageId == request.LanguageId
            });

        if (localization is null)
        {
            return Result.Fail<DeleteEventNewsCategoryLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(EventNewsCategoryLocalization)));
        }

        _repositoryWrapper.EventNewsCategoryLocalizationsRepository.Delete(localization);

        try
        {
            return await _repositoryWrapper.SaveChangesAsync() > 0
                ? Result.Ok(new DeleteEventNewsCategoryLocalizationDto(request.EntityId, request.LanguageId))
                : Result.Fail<DeleteEventNewsCategoryLocalizationDto>(
                    ErrorMessagesConstants.FailedToDeleteEntity(typeof(EventNewsCategoryLocalization)));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<DeleteEventNewsCategoryLocalizationDto>(ErrorMessagesConstants.NotFound(
                (request.EntityId, request.LanguageId),
                typeof(EventNewsCategoryLocalization)));
        }
    }
}
