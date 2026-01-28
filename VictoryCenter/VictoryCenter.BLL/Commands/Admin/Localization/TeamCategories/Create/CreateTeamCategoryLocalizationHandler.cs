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

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Create;
public class CreateTeamCategoryLocalizationHandler : IRequestHandler<CreateTeamCategoryLocalizationCommand, Result<TeamCategoryLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<TeamCategory, TeamCategoryLocalization> _localizationService;
    private readonly IValidator<CreateTeamCategoryLocalizationCommand> _validator;

    public CreateTeamCategoryLocalizationHandler(IMapper mapper, ILocalizationService<TeamCategory, TeamCategoryLocalization> localizationService, IValidator<CreateTeamCategoryLocalizationCommand> validator)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
    }

    public async Task<Result<TeamCategoryLocalizationDto>> Handle(CreateTeamCategoryLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            TeamCategoryLocalization entity = _mapper.Map<TeamCategoryLocalization>(request.CreateTeamCategoryLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
            TeamCategoryLocalizationDto responseDto = _mapper.Map<TeamCategoryLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<TeamCategoryLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<TeamCategoryLocalizationDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(TeamCategoryLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<TeamCategoryLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<TeamCategoryLocalizationDto>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(TeamCategoryLocalization)));
        }
    }
}
