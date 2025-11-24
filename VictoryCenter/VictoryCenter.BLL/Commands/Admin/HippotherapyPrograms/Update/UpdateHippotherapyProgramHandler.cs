using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Update;

public class UpdateHippotherapyProgramHandler : IRequestHandler<UpdateHippotherapyProgramCommand, Result<HippotherapyProgramDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateHippotherapyProgramCommand> _validator;

    public UpdateHippotherapyProgramHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateHippotherapyProgramCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<HippotherapyProgramDto>> Handle(
        UpdateHippotherapyProgramCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            HippotherapyProgram? programToUpdate = await _repositoryWrapper
                .HippotherapyProgramsRepository.GetFirstOrDefaultAsync(
                    new QueryOptions<HippotherapyProgram>
                    {
                        Filter = program => program.Id == request.Id,
                        Include = program => program.Include(p => p.Categories),
                        AsNoTracking = false
                    });

            if (programToUpdate is null)
            {
                return Result.Fail<HippotherapyProgramDto>(
                    ErrorMessagesConstants.NotFound(request.Id, typeof(HippotherapyProgram)));
            }

            IEnumerable<HippotherapyProgramCategory> newCategories = await _repositoryWrapper
                .HippotherapyProgramCategoriesRepository.GetAllAsync(
                    new QueryOptions<HippotherapyProgramCategory>
                    {
                        Filter = category => request.UpdateProgramDto.CategoryIds.Contains(category.Id),
                        AsNoTracking = false
                    });

            if (newCategories.Count() != request.UpdateProgramDto.CategoryIds.Count)
            {
                var existingIds = newCategories.Select(c => c.Id).ToList();
                var missingIds = request.UpdateProgramDto.CategoryIds.Except(existingIds).ToList();
                return Result.Fail<HippotherapyProgramDto>(
                    ErrorMessagesConstants.NotFound(string.Join(", ", missingIds), typeof(HippotherapyProgramCategory)));
            }

            _mapper.Map(request.UpdateProgramDto, programToUpdate);

            if (programToUpdate.BackgroundImageId.HasValue)
            {
                Image? backgroundImage = await _repositoryWrapper.ImageRepository
                    .GetFirstOrDefaultAsync(new QueryOptions<Image>
                    {
                        Filter = image => image.Id == programToUpdate.BackgroundImageId.Value,
                        AsNoTracking = false
                    });

                if (backgroundImage == null)
                {
                    return Result.Fail<HippotherapyProgramDto>(
                        ErrorMessagesConstants.NotFound(programToUpdate.BackgroundImageId.Value, typeof(Image)));
                }

                programToUpdate.BackgroundImage = backgroundImage;
            }
            else
            {
                programToUpdate.BackgroundImage = null;
            }

            if (programToUpdate.PreviewImageId.HasValue)
            {
                Image? previewImage = await _repositoryWrapper.ImageRepository
                    .GetFirstOrDefaultAsync(new QueryOptions<Image>
                    {
                        Filter = image => image.Id == programToUpdate.PreviewImageId.Value,
                        AsNoTracking = false
                    });

                if (previewImage == null)
                {
                    return Result.Fail<HippotherapyProgramDto>(
                        ErrorMessagesConstants.NotFound(programToUpdate.PreviewImageId.Value, typeof(Image)));
                }

                programToUpdate.PreviewImage = previewImage;
            }
            else
            {
                programToUpdate.PreviewImage = null;
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

            return Result.Fail<HippotherapyProgramDto>(
                ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgram)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<HippotherapyProgramDto>(ex.Message);
        }
        catch (BlobStorageException)
        {
            return Result.Fail<HippotherapyProgramDto>(
                HippotherapyProgramConstants.FailedRetrievingProgramPhoto);
        }
    }
}
