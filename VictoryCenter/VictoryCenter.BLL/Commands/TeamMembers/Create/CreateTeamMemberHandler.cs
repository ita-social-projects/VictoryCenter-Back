using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.TeamMembers.Create;

public class CreateTeamMemberHandler : IRequestHandler<CreateTeamMemberCommand, Result<TeamMemberDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateTeamMemberCommand> _validator;
    private readonly IBlobService _blobService;

    public CreateTeamMemberHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IValidator<CreateTeamMemberCommand> validator, IBlobService blobService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
        _blobService = blobService;
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
                return Result.Fail<TeamMemberDto>(ErrorMessagesConstants.NotFound(request.createTeamMemberDto.CategoryId, typeof(Category)));
            }

            TeamMember? entity = _mapper.Map<TeamMember>(request.createTeamMemberDto);
            using (TransactionScope scope = _repositoryWrapper.BeginTransaction())
            {
                entity.CreatedAt = DateTime.UtcNow;
                var maxPriority = await _repositoryWrapper.TeamMembersRepository.MaxAsync(
                    u => u.Priority,
                    u => u.CategoryId == entity.CategoryId);
                entity.Priority = (maxPriority ?? 0) + 1;
                await _repositoryWrapper.TeamMembersRepository.CreateAsync(entity);

                if (await _repositoryWrapper.SaveChangesAsync() > 0)
                {
                    scope.Complete();
                }
                else
                {
                    return Result.Fail<TeamMemberDto>(TeamMemberConstants.FailedToCreateNewTeamMember);
                }
            }

            TeamMemberDto? result = _mapper.Map<TeamMemberDto>(entity);
            if (entity.ImageId != null)
            {
                Image? imageResult = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(
                    new QueryOptions<Image>()
                    {
                        Filter = i => i.Id == entity.ImageId
                    });
                if (imageResult is not null)
                {
                    try
                    {
                        imageResult.Base64 = await _blobService.FindFileInStorageAsBase64Async(imageResult.BlobName, imageResult.MimeType);
                    }
                    catch(Exception)
                    {
                        return Result.Fail<TeamMemberDto>(TeamMemberConstants.FailedRetrievingMemberPhoto);
                    }
                }

                result.Image = _mapper.Map<ImageDTO>(imageResult);
            }

            return Result.Ok(result);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<TeamMemberDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail<TeamMemberDto>(TeamMemberConstants.FailedToCreateNewTeamMemberInTheDatabase + ex.Message);
        }
    }
}
