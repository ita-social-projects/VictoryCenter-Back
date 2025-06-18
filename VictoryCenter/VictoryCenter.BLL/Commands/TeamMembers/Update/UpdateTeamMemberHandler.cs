using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.TeamMembers.Update;

public class UpdateTeamMemberHandler : IRequestHandler<UpdateTeamMemberCommand, Result<TeamMemberDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateTeamMemberCommand> _validator;

    public UpdateTeamMemberHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateTeamMemberCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<TeamMemberDto>> Handle(UpdateTeamMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var teamMemberEntity =
                await _repositoryWrapper.TeamMemberRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamMember>
                {
                    FilterPredicate = entity => entity.Id == request.updateTeamMemberDto.Id
                });

            if (teamMemberEntity is null)
            {
                return Result.Fail<TeamMemberDto>("Not found");
            }

            var entityToUpdate = _mapper.Map<UpdateTeamMemberDto, TeamMember>(request.updateTeamMemberDto);
            entityToUpdate.CreatedAt = teamMemberEntity.CreatedAt;

            _repositoryWrapper.TeamMemberRepository.Update(teamMemberEntity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var resultDto = _mapper.Map<TeamMember, TeamMemberDto>(teamMemberEntity);
                return Result.Ok(resultDto);
            }

            return Result.Fail<TeamMemberDto>("Failed to update team member");
        }
        catch (ValidationException ex)
        {
            return Result.Fail<TeamMemberDto>(ex.Message);
        }
    }
}
