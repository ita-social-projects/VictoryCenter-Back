using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.Exceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.TeamMembers.Update;

public class UpdateTeamMemberHandler : IRequestHandler<UpdateTeamMemberCommand, Result<TeamMemberDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateTeamMemberCommand> _validator;
    private readonly IBlobService _blobService;

    public UpdateTeamMemberHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateTeamMemberCommand> validator,
        IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _blobService = blobService;
    }

    public async Task<Result<TeamMemberDto>> Handle(UpdateTeamMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            TeamMember? teamMemberEntity =
                await _repositoryWrapper.TeamMembersRepository.GetFirstOrDefaultAsync(new QueryOptions<TeamMember>
                {
                    Filter = entity => entity.Id == request.Id
                });

            if (teamMemberEntity is null)
            {
                return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(TeamMember)));
            }

            TeamMember? entityToUpdate = _mapper.Map<UpdateTeamMemberDto, TeamMember>(request.UpdateTeamMemberDto);
            entityToUpdate.Id = request.Id;
            using TransactionScope scope = _repositoryWrapper.BeginTransaction();
            entityToUpdate.CreatedAt = teamMemberEntity.CreatedAt;

            Category? category = await _repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(
                new QueryOptions<Category>
                {
                    Filter = entity => entity.Id == request.UpdateTeamMemberDto.CategoryId
                });
            if (category is null)
            {
                return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.NotFound(request.UpdateTeamMemberDto.CategoryId, typeof(Category)));
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
                var resultDto = _mapper.Map<TeamMember, TeamMemberDto>(entityToUpdate);
                if (entityToUpdate.ImageId != null)
                {
                    Image? image = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(
                        new QueryOptions<Image>()
                        {
                            Filter = i => i.Id == entityToUpdate.ImageId
                        });
                    if (image != null)
                    {
                        var imageDto = _mapper.Map<ImageDto>(image);
                        imageDto.Base64 = await _blobService.FindFileInStorageAsBase64Async(image.BlobName, image.MimeType);
                        resultDto.Image = imageDto;
                    }
                    else
                    {
                        return Result.Fail<TeamMemberDto>(TeamMemberConstants.FailedRetrievingMemberPhoto);
                    }
                }

                scope.Complete();
                return Result.Ok(resultDto);
            }

            return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamMember)));
        }
        catch (BlobStorageException e)
        {
            return Result.Fail<TeamMemberDto>($"Error with user image: {e.Message}");
        }
        catch (ValidationException vex)
        {
            return Result.Fail<TeamMemberDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
