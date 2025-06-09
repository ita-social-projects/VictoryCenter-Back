using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.TeamMembers.CreateTeamMember;

public class CreateTeamMemberHandler : IRequestHandler<CreateTeamMemberCommand, Result<TeamMemberDto>>
{

    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CreateTeamMemberHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<TeamMemberDto>> Handle(CreateTeamMemberCommand request , CancellationToken cancellationToken)
    {

        try
        {
            var entity = _mapper.Map<TeamMember>(request.createTeamMemberDto);
            entity.CreatedAt = DateTime.Now;
            entity.Priority = await _repositoryWrapper.TeamMembersRepository.CountAsync(x => x.CategoryId == entity.CategoryId) + 1;

            await _repositoryWrapper.TeamMembersRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                
                var result = _mapper.Map<TeamMemberDto>(entity);

                return Result.Ok(result);
            }
            else
            {
                return Result.Fail("Failed to create new TeamMember");
            }
            
        }
        catch (Exception ex)
        {
            return Result.Fail<TeamMemberDto>(ex.Message);
        }
        

    }
    
}