using AutoMapper;
using MediatR;
using FluentResults;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Programs.Update;

public class UpdateProgramHandler : IRequestHandler<UpdateProgramCommand, Result<ProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateProgramCommand> _validator;
    private readonly IBlobService _blobService;

    public UpdateProgramHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IValidator<UpdateProgramCommand> validator, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _blobService = blobService;
    }

    public async Task<Result<ProgramDto>> Handle(UpdateProgramCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            Program? programToUpdate = await _repositoryWrapper.ProgramsRepository.GetFirstOrDefaultAsync(
                new QueryOptions<Program>()
                {
                    Filter = program => program.Id == request.id,
                    Include = program => program.Include(p => p.Categories),
                    AsNoTracking = false
                });

            if (programToUpdate is null)
            {
                return Result.Fail<ProgramDto>(ErrorMessagesConstants
                    .NotFound(request.id, typeof(Program)));
            }

            IEnumerable<ProgramCategory> newCategories = await _repositoryWrapper.ProgramCategoriesRepository.GetAllAsync(
                new QueryOptions<ProgramCategory>
                {
                    Filter = category => request.updateProgramDto.CategoriesId.Contains(category.Id),
                    AsNoTracking = false
                });

            _mapper.Map(request.updateProgramDto, programToUpdate);

            if (programToUpdate.ImageId != null)
            {
                Image? newImage = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(new QueryOptions<Image>
                {
                    Filter = image => image.Id == request.updateProgramDto.ImageId,
                    AsNoTracking = false
                });
                if (newImage is not null)
                {
                    try
                    {
                        newImage.Base64 =
                            await _blobService.FindFileInStorageAsBase64Async(newImage.BlobName, newImage.MimeType);
                    }
                    catch (Exception)
                    {
                        return Result.Fail<ProgramDto>(ProgramConstants.FailedRetrievingProgramPhoto);
                    }
                }

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

            return Result.Fail<ProgramDto>(ProgramConstants.FailedToUpdateProgram);
        }
        catch (ValidationException ex)
        {
            return Result.Fail<ProgramDto>(ex.Message);
        }
    }
}
