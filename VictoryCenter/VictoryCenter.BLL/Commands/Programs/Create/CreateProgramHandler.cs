using MediatR;
using AutoMapper;
using FluentResults;
using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Programs.Create;

public class CreateProgramHandler : IRequestHandler<CreateProgramCommand, Result<ProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateProgramCommand> _validator;
    private readonly IBlobService _blobService;

    public CreateProgramHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<CreateProgramCommand> validator, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _blobService = blobService;
    }

    public async Task<Result<ProgramDto>> Handle(CreateProgramCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var categoryOptions = new QueryOptions<ProgramCategory>
            {
                Filter = category => request.createProgramDto.CategoriesId.Contains(category.Id),
                AsNoTracking = false
            };

            var categories = await _repositoryWrapper
                .ProgramCategoriesRepository.GetAllAsync(categoryOptions);

            var entity = _mapper.Map<Program>(request.createProgramDto);

            if (entity.ImageId != null)
            {
                var newImage = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
                {
                    Filter = image => image.Id == request.createProgramDto.ImageId,
                    AsNoTracking = false
                });
                if (newImage is not null)
                {
                    try
                    {
                        newImage.Base64 = await _blobService.FindFileInStorageAsBase64Async(newImage.BlobName, newImage.MimeType);
                    }
                    catch(Exception)
                    {
                        return Result.Fail<ProgramDto>(ProgramConstants.FailedRetrievingProgramPhoto);
                    }
                }

                entity.Image = newImage;
            }

            entity.Categories = categories.ToList();
            entity.CreatedAt = DateTime.UtcNow;

            await _repositoryWrapper.ProgramsRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<ProgramDto>(entity));
            }

            return Result.Fail<ProgramDto>(ProgramConstants.FailedToCreateProgram);
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ProgramDto>(ex.Message);
        }
    }
}
