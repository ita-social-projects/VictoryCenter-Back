using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Images.Update;

public class UpdateImageHandler : IRequestHandler<UpdateImageCommand, Result<ImageDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateImageCommand> _validator;

    public UpdateImageHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateImageCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<ImageDTO>> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var imageEntity =
                await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
                {
                    Filter = entity => entity.Id == request.id
                });

            if (imageEntity is null)
            {
                return Result.Fail<ImageDTO>("Not found");
            }

            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var entityToUpdate = _mapper.Map<Image>(request.updateImageDto);
            entityToUpdate.Id = imageEntity.Id; // Ensure the ID remains unchanged
            entityToUpdate.BlobName = imageEntity.BlobName;

            _repositoryWrapper.ImageRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var resultDto = _mapper.Map<Image, ImageDTO>(entityToUpdate);
                return Result.Ok(resultDto);
            }

            return Result.Fail<ImageDTO>("Failed to update image");
        }
        catch (ValidationException vex)
        {
            return Result.Fail<ImageDTO>(vex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
