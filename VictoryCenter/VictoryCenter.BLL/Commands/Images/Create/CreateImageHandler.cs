using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Images.Create;

public class CreateImageHandler : IRequestHandler<CreateImageCommand, Result<ImageDTO>>
{
    private IBlobService _blobService;
    private IRepositoryWrapper _repositoryWrapper;
    private IMapper _mapper;

    public CreateImageHandler(IBlobService blobService, IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _blobService = blobService;
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<ImageDTO>> Handle(CreateImageCommand request, CancellationToken cancellationToken)
    {
        var fileName = Guid.NewGuid().ToString().Replace("-", "");
        var fileNewName = _blobService.SaveFileInStorage(request.CreateImageDto.Base64, fileName, request.CreateImageDto.MimeType);
        var image = _mapper.Map<Image>(request.CreateImageDto);
        image.BlobName = fileName;

        var result = await _repositoryWrapper.ImageRepository.CreateAsync(image);
        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(_mapper.Map<ImageDTO>(result));
        }

        return Result.Fail("something go wrong");
    }
}
