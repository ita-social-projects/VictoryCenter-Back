using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Update;

public class UpdateTeamMemberHandler : IRequestHandler<UpdateTeamMemberCommand, Result<TeamMemberDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateTeamMemberCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;
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

    public async Task<Result<TeamMemberDto>> Handle(UpdateTeamMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var entityToUpdate = await _repositoryWrapper.TeamMembersRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamMember>
            {
                Filter = entity => entity.Id == request.Id,
                AsNoTracking = false
            });

            if (entityToUpdate is null)
            {
                return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(TeamMember)));
            }

            var oldCategoryId = entityToUpdate.CategoryId;
            var newCategoryId = request.UpdateTeamMemberDto.CategoryId;
            var categoryChanged = oldCategoryId != newCategoryId;

            if (categoryChanged)
            {
                var newCategory = await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(new QueryOptions<Category>
                {
                    Filter = c => c.Id == newCategoryId,
                    AsNoTracking = true
                });

                if (newCategory == null)
                {
                    return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.NotFound(newCategoryId, typeof(Category)));
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

                    return Result.Ok(resultDto);
                }

                return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamMember)));
            }
        }
        catch (ValidationException vex)
        {
            return Result.Fail<TeamMemberDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (BlobStorageException e)
        {
            return Result.Fail<TeamMemberDto>(ImageConstants.ErrorWithUserImage(e.Message));
        }
        catch (ReorderException e)
        {
            return Result.Fail<TeamMemberDto>(ReorderConstants.ErrorWithReordering(e.Message));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamMember)));
        }
    }
}
