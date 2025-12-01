using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Create;

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

            var teamMember = await _repositoryWrapper.TeamMembersRepository
            .GetFirstOrDefaultAsync(
                new QueryOptions<TeamMember>
                {
                    Filter = entity => entity.Id == request.CreateTeamMemberLocalizationDto.EntityId,
                });

            if (teamMember is null)
            {
                return Result.Fail<TeamMemberLocalizationDto>(
                    ErrorMessagesConstants.NotFound(
                        request.CreateTeamMemberLocalizationDto.EntityId,
                        typeof(TeamMember)));
            }

            var localizationLanguage = await _repositoryWrapper.LocalizationLanguagesRepository
                .GetFirstOrDefaultAsync(
                new QueryOptions<LocalizationLanguage>
                {
                    Filter = entity => entity.Id == request.CreateTeamMemberLocalizationDto.LanguageId,
                });

            if (localizationLanguage is null)
            {
                return Result.Fail<TeamMemberLocalizationDto>(
                    ErrorMessagesConstants.NotFound(
                        request.CreateTeamMemberLocalizationDto.LanguageId,
                        typeof(LocalizationLanguage)));
            }

            TeamMemberLocalization entity = _mapper.Map<TeamMemberLocalization>(request.CreateTeamMemberLocalizationDto);
            entity.CreatedAt = DateTimeOffset.UtcNow;
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
        catch (DbUpdateException)
        {
            return Result.Fail<TeamMemberLocalizationDto>(ErrorMessagesConstants.
                FailedToCreateEntityInDatabase(typeof(TeamMemberLocalization)));
        }
    }
}
