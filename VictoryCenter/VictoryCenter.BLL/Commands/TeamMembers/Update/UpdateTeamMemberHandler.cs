using System.Transactions;
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
                await _repositoryWrapper.TeamMembersRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamMember>
                {
                    Filter = entity => entity.Id == request.updateTeamMemberDto.Id
                });

            if (teamMemberEntity is null)
            {
                return Result.Fail<TeamMemberDto>("Not found");
            }

            var entityToUpdate = _mapper.Map<UpdateTeamMemberDto, TeamMember>(request.updateTeamMemberDto);
            using TransactionScope scope = _repositoryWrapper.BeginTransaction();
            entityToUpdate.CreatedAt = teamMemberEntity.CreatedAt;

            var category = await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(new QueryOptions<Category>
            {
                Filter = entity => entity.Id == request.updateTeamMemberDto.CategoryId
            });
            if (category is null)
            {
                return Result.Fail<TeamMemberDto>("Category not found");
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
                var resultDto = _mapper.Map<TeamMember, TeamMemberDto>(entityToUpdate);
                return Result.Ok(resultDto);
            }

            return Result.Fail<TeamMemberDto>("Failed to update team member");
        }
        catch (ValidationException vex)
        {
            return Result.Fail<TeamMemberDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
