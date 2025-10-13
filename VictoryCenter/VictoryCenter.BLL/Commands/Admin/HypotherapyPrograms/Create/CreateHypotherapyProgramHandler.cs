using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Create;

public class CreateHypotherapyProgramHandler : IRequestHandler<CreateHypotherapyProgramCommand, Result<HypotherapyProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateHypotherapyProgramCommand> _validator;

    public CreateHypotherapyProgramHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<CreateHypotherapyProgramCommand> validator, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<HypotherapyProgramDto>> Handle(CreateHypotherapyProgramCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            IEnumerable<HippotherapyProgramCategory> categories = await _repositoryWrapper
                .HypotherapyProgramCategoriesRepository.GetAllAsync(new QueryOptions<HippotherapyProgramCategory>
                {
                    Filter = category => request.CreateProgramDto.CategoryIds.Contains(category.Id),
                    AsNoTracking = false
                });

            HippotherapyProgram entity = _mapper.Map<HippotherapyProgram>(request.CreateProgramDto);

            if (entity.ImageId != null)
            {
                Image? newImage = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
                {
                    Filter = image => image.Id == request.CreateProgramDto.ImageId,
                    AsNoTracking = false
                });

                entity.Image = newImage;
            }

            entity.Categories = [.. categories];
            entity.CreatedAt = DateTimeOffset.UtcNow;

            await _repositoryWrapper.HypotherapyProgramsRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<HypotherapyProgramDto>(entity));
            }

            return Result.Fail<HypotherapyProgramDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgram)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<HypotherapyProgramDto>(ex.Message);
        }
        catch (BlobStorageException)
        {
            return Result.Fail<HypotherapyProgramDto>(HypotherapyProgramConstants.FailedRetrievingProgramPhoto);
        }
    }
}
