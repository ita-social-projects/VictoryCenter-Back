using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMemberLocalizations;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.TeamMemberLocalizations.Create;

public class CreateTeamMemberLocalizationHandler : IRequestHandler<CreateTeamMemberLocalizationCommand, Result<TeamMemberLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateTeamMemberLocalizationCommand> _validator;

    public CreateTeamMemberLocalizationHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IValidator<CreateTeamMemberLocalizationCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<TeamMemberLocalizationDto>> Handle(CreateTeamMemberLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            TeamMemberLocalization entity = _mapper.Map<TeamMemberLocalization>(request.CreateTeamMemberLocalizationDto);
            entity.CreatedAt = DateTime.UtcNow;
            await _repositoryWrapper.TeamMemberLocalizationsRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                TeamMemberLocalizationDto responseDto = _mapper.Map<TeamMemberLocalizationDto>(entity);
                return Result.Ok(responseDto);
            }

            return Result.Fail<TeamMemberLocalizationDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(TeamMemberLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<TeamMemberLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail<TeamMemberLocalizationDto>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(TeamMemberLocalization)) + ex.Message);
        }
    }
}
