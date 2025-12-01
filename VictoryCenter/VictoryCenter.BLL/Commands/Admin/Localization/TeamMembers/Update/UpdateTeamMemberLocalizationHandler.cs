using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Update;

public class UpdateTeamMemberLocalizationHandler : IRequestHandler<UpdateTeamMemberLocalizationCommand, Result<TeamMemberLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateTeamMemberLocalizationCommand> _validator;

    public UpdateTeamMemberLocalizationHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<UpdateTeamMemberLocalizationCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<TeamMemberLocalizationDto>> Handle(UpdateTeamMemberLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var dto = request.UpdateTeamMemberLocalizationDto;

            TeamMemberLocalization? entity = await _repositoryWrapper.TeamMemberLocalizationsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<TeamMemberLocalization>
                {
                    Filter = localization => localization.EntityId == request.EntityId &&
                                           localization.LanguageId == request.LanguageId
                });

            if (entity is null)
            {
                return Result.Fail<TeamMemberLocalizationDto>(ErrorMessagesConstants
                    .NotFound(new { request.EntityId, request.LanguageId }, typeof(TeamMemberLocalization)));
            }

            TeamMemberLocalization entityToUpdate = _mapper.Map(dto, entity);
            entityToUpdate.TranslationStatus = TranslationStatus.Relevant;
            entityToUpdate.CreatedAt = entity.CreatedAt;

            _repositoryWrapper.TeamMemberLocalizationsRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                TeamMemberLocalizationDto responseDto = _mapper.Map<TeamMemberLocalizationDto>(entityToUpdate);
                return Result.Ok(responseDto);
            }

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
