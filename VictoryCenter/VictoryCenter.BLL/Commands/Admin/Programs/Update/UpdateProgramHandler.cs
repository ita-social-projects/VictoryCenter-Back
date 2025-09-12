using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Programs;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Programs.Update;

public class UpdateProgramHandler : IRequestHandler<UpdateProgramCommand, Result<ProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateProgramCommand> _validator;

    public UpdateProgramHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<UpdateProgramCommand> validator, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<ProgramDto>> Handle(UpdateProgramCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            Program? programToUpdate = await _repositoryWrapper.ProgramsRepository.GetFirstOrDefaultAsync(
                new QueryOptions<Program>
                {
                    Filter = program => program.Id == request.Id,
                    Include = program => program.Include(p => p.Categories),
                    AsNoTracking = false
                });

            if (programToUpdate is null)
            {
                return Result.Fail<ProgramDto>(ErrorMessagesConstants
                    .NotFound(request.Id, typeof(Program)));
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

            _repositoryWrapper.ProgramsRepository.Update(programToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                ProgramDto responseDto = _mapper.Map<ProgramDto>(programToUpdate);
                return Result.Ok(responseDto);
            }

            return Result.Fail<ProgramDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Program)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ProgramDto>(ex.Message);
        }
        catch (BlobStorageException)
        {
            return Result.Fail<ProgramDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Program)));
        }
    }
}
