using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Create;

public class CreateTeamMemberHandler : IRequestHandler<CreateTeamMemberCommand, Result<TeamMemberDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateTeamMemberCommand> _validator;
    private readonly IIndexReorderService _indexReorderService;

    public CreateTeamMemberHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreateTeamMemberCommand> validator,
        IIndexReorderService indexReorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
        _indexReorderService = indexReorderService;
    }

    public async Task<Result<TeamMemberDto>> Handle(CreateTeamMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            Category? category = await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(
                new QueryOptions<Category>
                {
                    Filter = c => c.Id == request.CreateTeamMemberDto.CategoryId
                });

            if (category == null)
            {
                return Result.Fail<TeamMemberDto>(
                    ErrorMessagesConstants.NotFound(request.CreateTeamMemberDto.CategoryId, typeof(Category)));
            }

            TeamMember? entity = _mapper.Map<TeamMember>(request.CreateTeamMemberDto);
            using (TransactionScope scope = _repositoryWrapper.BeginTransaction())
            {
                entity.CreatedAt = DateTime.UtcNow;

                entity.Priority = await _indexReorderService.GetNextPriority<TeamMember>(
                    u => u.CategoryId == entity.CategoryId);

                await _repositoryWrapper.TeamMembersRepository.CreateAsync(entity);

                if (await _repositoryWrapper.SaveChangesAsync() <= 0)
                {
                    return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(TeamMember)));
                }

                TeamMemberDto? result = _mapper.Map<TeamMemberDto>(entity);
                if (entity.ImageId != null)
                {
                    Image? imageResult = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(
                        new QueryOptions<Image>
                        {
                            Filter = i => i.Id == entity.ImageId
                        });

                    result.Image = _mapper.Map<ImageDto>(imageResult);
                }

                scope.Complete();
                return Result.Ok(result);
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
        catch (DbUpdateException)
        {
            return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(TeamMember)));
        }
    }
}
