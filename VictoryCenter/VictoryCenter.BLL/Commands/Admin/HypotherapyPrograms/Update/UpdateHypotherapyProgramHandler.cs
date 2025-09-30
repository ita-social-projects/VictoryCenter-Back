using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HypotherapyPrograms.Update;

public class UpdateHypotherapyProgramHandler : IRequestHandler<UpdateHypotherapyProgramCommand, Result<HypotherapyProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateHypotherapyProgramCommand> _validator;

    public UpdateHypotherapyProgramHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<UpdateHypotherapyProgramCommand> validator, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<HypotherapyProgramDto>> Handle(UpdateHypotherapyProgramCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            HypotherapyProgram? programToUpdate = await _repositoryWrapper.HypotherapyProgramsRepository.GetFirstOrDefaultAsync(
                new QueryOptions<HypotherapyProgram>
                {
                    Filter = program => program.Id == request.Id,
                    Include = program => program.Include(p => p.Categories),
                    AsNoTracking = false
                });

            if (programToUpdate is null)
            {
                return Result.Fail<HypotherapyProgramDto>(ErrorMessagesConstants
                    .NotFound(request.Id, typeof(HypotherapyProgram)));
            }

            IEnumerable<ProgramCategory> newCategories = await _repositoryWrapper.ProgramCategoriesRepository.GetAllAsync(
                new QueryOptions<ProgramCategory>
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

            foreach (ProgramCategory category in newCategories)
            {
                programToUpdate.Categories.Add(category);
            }

            _repositoryWrapper.HypotherapyProgramsRepository.Update(programToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                HypotherapyProgramDto responseDto = _mapper.Map<HypotherapyProgramDto>(programToUpdate);
                return Result.Ok(responseDto);
            }

            return Result.Fail<HypotherapyProgramDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(HypotherapyProgram)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<HypotherapyProgramDto>(ex.Message);
        }
        catch (BlobStorageException)
        {
            return Result.Fail<HypotherapyProgramDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(HypotherapyProgram)));
        }
    }
}
