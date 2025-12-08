using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Create;

public class CreateTeamMemberLocalizationHandler : IRequestHandler<CreateTeamMemberLocalizationCommand, Result<TeamMemberLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<CreateTeamMemberLocalizationCommand> _validator;
    private readonly ILocalizationService<TeamMember, TeamMemberLocalization> _localizationService;

    public CreateTeamMemberLocalizationHandler(
        IMapper mapper,
        IValidator<CreateTeamMemberLocalizationCommand> validator,
        ILocalizationService<TeamMember, TeamMemberLocalization> localizationService)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
    }

    public async Task<Result<TeamMemberLocalizationDto>> Handle(CreateTeamMemberLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            TeamMemberLocalization entity = _mapper.Map<TeamMemberLocalization>(request.CreateTeamMemberLocalizationDto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);
            TeamMemberLocalizationDto responseDto = _mapper.Map<TeamMemberLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<TeamMemberLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<TeamMemberLocalizationDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(TeamMemberLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<TeamMemberLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<TeamMemberLocalizationDto>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(TeamMemberLocalization)));
        }
    }
}
