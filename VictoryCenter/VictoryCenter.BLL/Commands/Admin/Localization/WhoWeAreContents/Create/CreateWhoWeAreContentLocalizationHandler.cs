using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.Localization.WhoWeAreContents.Create;

public class CreateWhoWeAreContentLocalizationHandler : IRequestHandler<CreateWhoWeAreContentLocalizationCommand, Result<List<WhoWeAreContentLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<CreateWhoWeAreContentLocalizationCommand> _validator;
    private readonly ILocalizationService<WhoWeAreContent, WhoWeAreContentLocalization> _localizationService;
    private readonly IRepositoryWrapper _repository;

    public CreateWhoWeAreContentLocalizationHandler(
        IMapper mapper,
        IValidator<CreateWhoWeAreContentLocalizationCommand> validator,
        ILocalizationService<WhoWeAreContent, WhoWeAreContentLocalization> localizationService,
        IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
        _repository = repository;
    }

    public async Task<Result<List<WhoWeAreContentLocalizationDto>>> Handle(CreateWhoWeAreContentLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = _repository.BeginTransaction();

            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var sanitizedDtosResult = await WhoWeAreContentLocalizationHandlerHelper.ValidateAndSanitizeAsync(
                _repository,
                request.SectionType,
                request.ContentLocalizationDtos);

            if (sanitizedDtosResult.IsFailed)
            {
                return Result.Fail<List<WhoWeAreContentLocalizationDto>>(sanitizedDtosResult.Errors.Select(e => e.Message));
            }

            var createdLocalizations = new List<WhoWeAreContentLocalizationDto>();

            foreach (var sanitizedDto in sanitizedDtosResult.Value)
            {
                WhoWeAreContentLocalization entity = _mapper.Map<WhoWeAreContentLocalization>(sanitizedDto);
                var result = await _localizationService.CreateEntityLocalizationAsync(entity);
                WhoWeAreContentLocalizationDto responseDto = _mapper.Map<WhoWeAreContentLocalizationDto>(result);
                createdLocalizations.Add(responseDto);
            }

            transaction.Complete();
            return Result.Ok(createdLocalizations);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(knfex.Message);
        }
        catch (ArgumentException aex)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(aex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(ErrorMessagesConstants.FailedToCreateEntity(typeof(WhoWeAreContentLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<List<WhoWeAreContentLocalizationDto>>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(WhoWeAreContentLocalization)));
        }
    }
}
