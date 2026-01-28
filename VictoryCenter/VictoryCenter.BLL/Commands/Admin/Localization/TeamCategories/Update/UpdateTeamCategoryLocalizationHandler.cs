using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Update;
public class UpdateTeamCategoryLocalizationHandler : IRequestHandler<UpdateTeamCategoryLocalizationCommand, Result<TeamCategoryLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<TeamCategory, TeamCategoryLocalization> _localizationService;
    private readonly IValidator<UpdateTeamCategoryLocalizationCommand> _validator;

    public UpdateTeamCategoryLocalizationHandler(IMapper mapper, ILocalizationService<TeamCategory, TeamCategoryLocalization> localizationService, IValidator<UpdateTeamCategoryLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<TeamCategoryLocalizationDto>> Handle(UpdateTeamCategoryLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            TeamCategoryLocalization entity = _mapper.Map<TeamCategoryLocalization>(request.UpdateTeamCategoryLocalizationDto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            TeamCategoryLocalizationDto responseDto = _mapper.Map<TeamCategoryLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<TeamCategoryLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<TeamCategoryLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamCategoryLocalization)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<TeamCategoryLocalizationDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<TeamCategoryLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(TeamCategoryLocalization)));
        }
    }
}
