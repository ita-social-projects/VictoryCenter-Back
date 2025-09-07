using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.Delete;

public class DeleteLocalizationLanguageHandler : IRequestHandler<DeleteLocalizationLanguageCommand, Result<long>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteLocalizationLanguageHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<long>> Handle(DeleteLocalizationLanguageCommand request, CancellationToken cancellationToken)
    {
        LocalizationLanguage? entityToDelete = await _repositoryWrapper.LocalizationLanguagesRepository
            .GetFirstOrDefaultAsync(new QueryOptions<LocalizationLanguage>
            {
                Filter = localizationLanguage => localizationLanguage.Id == request.Id,
            });

        if (entityToDelete is null)
        {
            return Result.Fail<long>(ErrorMessagesConstants
                .NotFound(request.Id, typeof(LocalizationLanguage)));
        }

        _repositoryWrapper.LocalizationLanguagesRepository.Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(entityToDelete.Id);
        }

        return Result.Fail(ErrorMessagesConstants.FailedToDeleteEntity(typeof(LocalizationLanguage)));
    }
}
