using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.TeamMembers.Update;

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
            entityToUpdate.CreatedAt = teamMemberEntity.CreatedAt;

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
                        image.Base64 = _blobService.FindFileInStorageAsBase64(image.BlobName, image.MimeType);
                    }
                    else
                    {
                        return Result.Fail<TeamMemberDto>("Fail to find TeamMember photo");
                    }

                    resultDto.Image = _mapper.Map<ImageDTO>(image);
                }

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
