using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Update;

public class UpdateTeamMemberHandler : BaseHandler<UpdateTeamMemberCommand, TeamMemberDto>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateTeamMemberCommand> _validator;
    private readonly IReorderService _indexReorderService;

    public UpdateTeamMemberHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateTeamMemberCommand> validator,
        IReorderService indexReorderService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _indexReorderService = indexReorderService;
    }

    public override async Task<TeamMemberDto> HandleRequest(UpdateTeamMemberCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var entityToUpdate = await _repositoryWrapper.TeamMembersRepository.GetFirstOrDefaultAsync(
            new QueryOptions<TeamMember>
            {
                Filter = entity => entity.Id == request.Id,
                AsNoTracking = false
            });

        if (entityToUpdate is null)
        {
            throw new Exception(ErrorMessagesConstants.NotFound(request.Id, typeof(TeamMember)));
        }

        var oldCategoryId = entityToUpdate.CategoryId;
        var newCategoryId = request.UpdateTeamMemberDto.CategoryId;
        var categoryChanged = oldCategoryId != newCategoryId;

<<<<<<< HEAD
        if (categoryChanged)
        {
            var newCategory = await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(
                new QueryOptions<Category>
=======
            if (categoryChanged)
            {
                var newCategory = await _repositoryWrapper.TeamCategoriesRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamCategory>
>>>>>>> release/1.0.0
                {
                    Filter = c => c.Id == newCategoryId,
                    AsNoTracking = true
                });

<<<<<<< HEAD
            if (newCategory == null)
=======
                if (newCategory == null)
                {
                    return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.NotFound(newCategoryId, typeof(TeamCategory)));
                }
            }

            using (TransactionScope scope = _repositoryWrapper.BeginTransaction())
>>>>>>> release/1.0.0
            {
                throw new Exception(ErrorMessagesConstants.NotFound(newCategoryId, typeof(Category)));
            }
        }

        using (TransactionScope scope = _repositoryWrapper.BeginTransaction())
        {
            var rowsAffected = 0;

            _mapper.Map(request.UpdateTeamMemberDto, entityToUpdate);

            if (categoryChanged)
            {
                entityToUpdate.Priority = await _indexReorderService.GetNextDisplayOrderAsync<TeamMember>(
                    groupSelector: tm => tm.CategoryId == newCategoryId);
            }

            _repositoryWrapper.TeamMembersRepository.Update(entityToUpdate);
            rowsAffected += await _repositoryWrapper.SaveChangesAsync();

            if (categoryChanged)
            {
                await _indexReorderService.RenumberPriorityAsync<TeamMember>(
                    groupSelector: tm => tm.CategoryId == oldCategoryId);
            }

            rowsAffected += await _repositoryWrapper.SaveChangesAsync();

            if (rowsAffected > 0)
            {
                if (entityToUpdate.ImageId != null)
                {
                    Image? image = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(
                        new QueryOptions<Image>
                        {
                            Filter = i => i.Id == entityToUpdate.ImageId
                        });
                    entityToUpdate.Image = image;
                }

                TeamMemberDto? resultDto = _mapper.Map<TeamMember, TeamMemberDto>(entityToUpdate);

                scope.Complete();

                return resultDto;
            }

            throw new DbUpdateException(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamMember)));
        }
    }
}
