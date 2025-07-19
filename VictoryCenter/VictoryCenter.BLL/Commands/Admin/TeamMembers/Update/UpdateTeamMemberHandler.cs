using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Update;

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

            TeamMember? teamMemberEntity =
                await _repositoryWrapper.TeamMembersRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamMember>
                {
                    Filter = entity => entity.Id == request.id
                });

            if (teamMemberEntity is null)
            {
                return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.NotFound(request.id, typeof(TeamMember)));
            }

            TeamMember? entityToUpdate = _mapper.Map<UpdateTeamMemberDto, TeamMember>(request.updateTeamMemberDto);
            entityToUpdate.Id = request.id;
            using TransactionScope scope = _repositoryWrapper.BeginTransaction();
            entityToUpdate.CreatedAt = teamMemberEntity.CreatedAt;

            Category? category = await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(
                new QueryOptions<Category>
                {
                    Filter = entity => entity.Id == request.updateTeamMemberDto.CategoryId
                });
            if (category is null)
            {
                return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.NotFound(request.updateTeamMemberDto.CategoryId, typeof(Category)));
            }

            if (entityToUpdate.CategoryId == teamMemberEntity.CategoryId)
            {
                entityToUpdate.Priority = teamMemberEntity.Priority;
            }
            else
            {
                var maxPriority = await _repositoryWrapper.TeamMembersRepository.MaxAsync(
                    u => u.Priority,
                    u => u.CategoryId == entityToUpdate.CategoryId);
                entityToUpdate.Priority = (maxPriority ?? 0) + 1;
            }

            _repositoryWrapper.TeamMembersRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                scope.Complete();
                TeamMemberDto? resultDto = _mapper.Map<TeamMember, TeamMemberDto>(entityToUpdate);
                return Result.Ok(resultDto);
            }

            return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamMember)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<TeamMemberDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
