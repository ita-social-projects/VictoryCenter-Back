using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Update;

public class UpdateHippotherapyProgramHandler : IRequestHandler<UpdateHippotherapyProgramCommand, Result<HippotherapyProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateHippotherapyProgramCommand> _validator;

    public UpdateHippotherapyProgramHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<UpdateHippotherapyProgramCommand> validator, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<HippotherapyProgramDto>> Handle(UpdateHippotherapyProgramCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            HippotherapyProgram? programToUpdate = await _repositoryWrapper.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(
                new QueryOptions<HippotherapyProgram>
                {
                    Filter = program => program.Id == request.Id,
                    Include = program => program.Include(p => p.Categories),
                    AsNoTracking = false
                });

            if (programToUpdate is null)
            {
                return Result.Fail<HippotherapyProgramDto>(ErrorMessagesConstants
                    .NotFound(request.Id, typeof(HippotherapyProgram)));
            }

            IEnumerable<HippotherapyProgramCategory> newCategories = await _repositoryWrapper.HippotherapyProgramCategoriesRepository.GetAllAsync(
                new QueryOptions<HippotherapyProgramCategory>
                {
                    Filter = category => request.UpdateProgramDto.CategoryIds.Contains(category.Id),
                    AsNoTracking = false
                });

            _mapper.Map(request.UpdateProgramDto, programToUpdate);

            if (programToUpdate.ImageId != null)
            {
                Image? newImage = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
                {
                    Filter = image => image.Id == request.UpdateProgramDto.ImageId,
                    AsNoTracking = false
                });

                programToUpdate.Image = newImage;
            }

            programToUpdate.Categories.Clear();

            foreach (HippotherapyProgramCategory category in newCategories)
            {
                programToUpdate.Categories.Add(category);
            }

            _repositoryWrapper.HippotherapyProgramsRepository.Update(programToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                HippotherapyProgramDto responseDto = _mapper.Map<HippotherapyProgramDto>(programToUpdate);
                return Result.Ok(responseDto);
            }

            return Result.Fail<HippotherapyProgramDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgram)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<HippotherapyProgramDto>(ex.Message);
        }
        catch (BlobStorageException)
        {
            return Result.Fail<HippotherapyProgramDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgram)));
        }
    }
}
