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

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Update;

public class UpdateTeamMemberLocalizationHandler : IRequestHandler<UpdateTeamMemberLocalizationCommand, Result<TeamMemberLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateTeamMemberLocalizationCommand> _validator;
    private readonly ILocalizationService<TeamMember, TeamMemberLocalization> _localizationService;

    public UpdateTeamMemberLocalizationHandler(
        IMapper mapper,
        IValidator<UpdateTeamMemberLocalizationCommand> validator,
        ILocalizationService<TeamMember, TeamMemberLocalization> localizationService)
    {
        _mapper = mapper;
        _validator = validator;
        _localizationService = localizationService;
    }

    public async Task<Result<TeamMemberLocalizationDto>> Handle(UpdateTeamMemberLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var dto = request.UpdateTeamMemberLocalizationDto;
            TeamMemberLocalization entity = _mapper.Map<TeamMemberLocalization>(dto);
            entity.EntityId = request.EntityId;
            entity.LanguageId = request.LanguageId;
            var result = await _localizationService.UpdateEntityLocalizationAsync(entity);
            TeamMemberLocalizationDto responseDto = _mapper.Map<TeamMemberLocalizationDto>(result);
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<TeamMemberLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<TeamMemberLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamMemberLocalization)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<TeamMemberLocalizationDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<TeamMemberLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(TeamMemberLocalization)));
        }
    }
}
