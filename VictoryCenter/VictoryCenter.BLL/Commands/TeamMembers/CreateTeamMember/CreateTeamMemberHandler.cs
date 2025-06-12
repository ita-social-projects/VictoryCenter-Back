using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

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

    public async Task<Result<TeamMemberDto>> Handle(CreateTeamMemberCommand request , CancellationToken cancellationToken)
    {

        try
        {

            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            
            var entity = _mapper.Map<TeamMember>(request.createTeamMemberDto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.Priority = await _repositoryWrapper.TeamMembersRepository.MaxAsync<long>(u => u.Priority) + 1;
            
            //TODO
            // add check if category exists
            
            await _repositoryWrapper.TeamMembersRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                
                var result = _mapper.Map<TeamMemberDto>(entity);

                return Result.Ok(result);
            }
            else
            {
                return Result.Fail<TeamMemberDto>("Failed to create new TeamMember");
            }
            
        }
        catch (Exception ex)
        {
            return Result.Fail<TeamMemberDto>(ex.Message);
        }
        

    }
    
}
