using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Create;

public class CreateHippotherapyProgramHandler : IRequestHandler<CreateHippotherapyProgramCommand, Result<HippotherapyProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateHippotherapyProgramCommand> _validator;

    public CreateHippotherapyProgramHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<CreateHippotherapyProgramCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<HippotherapyProgramDto>> Handle(CreateHippotherapyProgramCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            IEnumerable<HippotherapyProgramCategory> categories = await _repositoryWrapper
                .HippotherapyProgramCategoriesRepository.GetAllAsync(new QueryOptions<HippotherapyProgramCategory>
                {
                    Filter = category => request.CreateProgramDto.CategoryIds.Contains(category.Id!),
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

            await _repositoryWrapper.HippotherapyProgramsRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<HippotherapyProgramDto>(entity));
            }

            return Result.Fail<HippotherapyProgramDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgram)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<HippotherapyProgramDto>(ex.Message);
        }
        catch (BlobStorageException)
        {
            return Result.Fail<HippotherapyProgramDto>(HippotherapyProgramConstants.FailedRetrievingProgramPhoto);
        }
    }
}
