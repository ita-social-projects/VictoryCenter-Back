using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.TeamMembers.CreateTeamMember;

public class CreateTeamMemberHandler : IRequestHandler<CreateTeamMemberCommand, Result<TeamMemberDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateTeamMemberCommand> _validator;

    public CreateTeamMemberHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IValidator<CreateTeamMemberCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<TeamMemberDto>> Handle(CreateTeamMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var category = await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(
                new QueryOptions<Category>()
                {
                    Filter = c => c.Id == request.createTeamMemberDto.CategoryId
                });

            if (category == null)
            {
                return Result.Fail<TeamMemberDto>("There are no categories with this id");
            }

            TeamMember? entity = _mapper.Map<TeamMember>(request.createTeamMemberDto);
            using TransactionScope scope = _repositoryWrapper.BeginTransaction();

            entity.CreatedAt = DateTime.UtcNow;
            var maxPriority = await _repositoryWrapper.TeamMembersRepository.MaxAsync(
                u => u.Priority,
                u => u.CategoryId == entity.CategoryId);
            entity.Priority = (maxPriority ?? 0) + 1;
            await _repositoryWrapper.TeamMembersRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                scope.Complete();
                TeamMemberDto? result = _mapper.Map<TeamMemberDto>(entity);
                result.CategoryId = category.Id;
                return Result.Ok(result);
            }

            return Result.Fail<TeamMemberDto>("Failed to create new TeamMember");
        }
        catch (ValidationException vex)
        {
            return Result.Fail<TeamMemberDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail<TeamMemberDto>("Fail to create new team member in database:" + ex.Message);
        }
    }
}
