using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Images.Create;

public class CreateImageHandler : IRequestHandler<CreateImageCommand, Result<ImageDTO>>
{
    private readonly IBlobService _blobService;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateImageCommand> _validator;

    public CreateImageHandler(IBlobService blobService, IRepositoryWrapper repositoryWrapper, IMapper mapper, IValidator<CreateImageCommand> validator)
    {
        _blobService = blobService;
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<ImageDTO>> Handle(CreateImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var fileName = Guid.NewGuid().ToString().Replace("-", "");
            try
            {
                var fileNewName = _blobService.SaveFileInStorage(request.CreateImageDto.Base64, fileName, request.CreateImageDto.MimeType);
            }
            catch (Exception e)
            {
                return Result.Fail<ImageDTO>("File creating Fail");
            }

            Image? image = _mapper.Map<Image>(request.CreateImageDto);
            image.BlobName = fileName;

            Image result = await _repositoryWrapper.ImageRepository.CreateAsync(image);
            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<ImageDTO>(result));
            }

            return Result.Fail<ImageDTO>("something go wrong");
        }
        catch (ValidationException vex)
        {
            return Result.Fail<ImageDTO>(vex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
