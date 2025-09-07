using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.Localization.Create;

public class CreateLocalizationLanguageHandler : IRequestHandler<CreateLocalizationLanguageCommand, Result<LocalizationLanguageDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateLocalizationLanguageCommand> _validator;

    public CreateLocalizationLanguageHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IValidator<CreateLocalizationLanguageCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<LocalizationLanguageDto>> Handle(CreateLocalizationLanguageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var entity = _mapper.Map<LocalizationLanguage>(request.CreateLocalizationLanguageDto);
            entity.CreatedAt = DateTime.UtcNow;

            await _repositoryWrapper.LocalizationLanguagesRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var resultDto = _mapper.Map<LocalizationLanguageDto>(entity);
                return Result.Ok(resultDto);
            }

            return Result.Fail<LocalizationLanguageDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(LocalizationLanguage)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<LocalizationLanguageDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail<LocalizationLanguageDto>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(LocalizationLanguage)) + ex.Message);
        }
    }
}
