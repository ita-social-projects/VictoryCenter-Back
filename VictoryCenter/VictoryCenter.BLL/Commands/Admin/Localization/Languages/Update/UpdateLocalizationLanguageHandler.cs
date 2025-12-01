using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.Languages;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.Languages.Update;

public class UpdateLocalizationLanguageHandler : IRequestHandler<UpdateLocalizationLanguageCommand, Result<LocalizationLanguageDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateLocalizationLanguageCommand> _validator;

    public UpdateLocalizationLanguageHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateLocalizationLanguageCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<LocalizationLanguageDto>> Handle(UpdateLocalizationLanguageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var existingEntity =
                await _repositoryWrapper.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(new QueryOptions<LocalizationLanguage>
                {
                    Filter = entity => entity.Id == request.Id
                });

            if (existingEntity is null)
            {
                return Result.Fail<LocalizationLanguageDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(LocalizationLanguage)));
            }

            var entityToUpdate = _mapper.Map<UpdateLocalizationLanguageDto, LocalizationLanguage>(request.UpdateLocalizationLanguageDto);
            entityToUpdate.Id = request.Id;
            entityToUpdate.CreatedAt = existingEntity.CreatedAt;

            _repositoryWrapper.LocalizationLanguagesRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var resultDto = _mapper.Map<LocalizationLanguage, LocalizationLanguageDto>(entityToUpdate);
                return Result.Ok(resultDto);
            }

            return Result.Fail<LocalizationLanguageDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(LocalizationLanguage)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<LocalizationLanguageDto>(ex.Message);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<LocalizationLanguageDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(LocalizationLanguage)));
        }
    }
}
