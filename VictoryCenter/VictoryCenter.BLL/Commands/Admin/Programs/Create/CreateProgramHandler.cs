using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Programs;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Programs.Create;

public class CreateProgramHandler : IRequestHandler<CreateProgramCommand, Result<ProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateProgramCommand> _validator;

    public CreateProgramHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<CreateProgramCommand> validator, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<ProgramDto>> Handle(CreateProgramCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            IEnumerable<ProgramCategory> categories = await _repositoryWrapper
                .ProgramCategoriesRepository.GetAllAsync(new QueryOptions<ProgramCategory>
                {
                    Filter = category => request.CreateProgramDto.CategoryIds.Contains(category.Id),
                    AsNoTracking = false
                });

            Program entity = _mapper.Map<Program>(request.CreateProgramDto);

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

            await _repositoryWrapper.ProgramsRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                return Result.Ok(_mapper.Map<ProgramDto>(entity));
            }

            return Result.Fail<ProgramDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(Program)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ProgramDto>(ex.Message);
        }
        catch (BlobStorageException)
        {
            return Result.Fail<ProgramDto>(ProgramConstants.FailedRetrievingProgramPhoto);
        }
    }
}
