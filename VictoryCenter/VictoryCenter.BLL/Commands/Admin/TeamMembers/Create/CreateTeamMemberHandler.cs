using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Create;

public class CreateTeamMemberHandler : BaseHandler<CreateTeamMemberCommand, TeamMemberDto>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateTeamMemberCommand> _validator;
    private readonly IReorderService _reorderService;

    public CreateTeamMemberHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreateTeamMemberCommand> validator,
        IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
        _reorderService = reorderService;
    }

    public override async Task<TeamMemberDto> HandleRequest(CreateTeamMemberCommand request, CancellationToken cancellationToken)
    {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var category = await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(
                new QueryOptions<Category>
                {
                    Filter = c => c.Id == request.CreateTeamMemberDto.CategoryId
                });

            if (category == null)
            {
                throw new Exception(ErrorMessagesConstants.NotFound(request.CreateTeamMemberDto.CategoryId, typeof(Category)));
            }

            var entity = _mapper.Map<TeamMember>(request.CreateTeamMemberDto);

            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                entity.CreatedAt = DateTimeOffset.UtcNow;
                var maxPriority = await _repositoryWrapper.TeamMembersRepository.MaxAsync(
                    u => u.Priority,
                    u => u.CategoryId == entity.CategoryId);
                entity.Priority = (maxPriority ?? 0) + 1;

                await _repositoryWrapper.TeamMembersRepository.CreateAsync(entity);

                if (await _repositoryWrapper.SaveChangesAsync() <= 0)
                {
                    throw new DbUpdateException(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(TeamMember)));
                }

                var result = _mapper.Map<TeamMemberDto>(entity);
                if (entity.ImageId != null)
                {
                    var imageResult = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(
                        new QueryOptions<Image>
                        {
                            Filter = i => i.Id == entity.ImageId
                        });

                    result.Image = _mapper.Map<ImageDto>(imageResult);
                }

                return result;
            }
    }
}
