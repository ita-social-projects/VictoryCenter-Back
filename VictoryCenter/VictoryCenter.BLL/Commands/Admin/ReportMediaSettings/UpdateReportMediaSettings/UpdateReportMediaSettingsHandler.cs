using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Image = VictoryCenter.DAL.Entities.Image;

namespace VictoryCenter.BLL.Commands.Admin.ReportMediaSettings.UpdateReportMediaSettings;

public class UpdateReportMediaSettingsHandler : IRequestHandler<UpdateReportMediaSettingsCommand, Result<ReportMediaSettingsDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateReportMediaSettingsCommand> _validator;

    public UpdateReportMediaSettingsHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IValidator<UpdateReportMediaSettingsCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<ReportMediaSettingsDto>> Handle(UpdateReportMediaSettingsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            Image? collectedFundsImageExists = null;
            if (request.Dto.CollectedFundsBlock.ImageId.HasValue)
            {
                collectedFundsImageExists = await _repositoryWrapper.ImageRepository
                    .GetFirstOrDefaultAsync(new QueryOptions<Image>
                    {
                        Filter = i => i.Id == request.Dto.CollectedFundsBlock.ImageId,
                        AsNoTracking = true
                    });

                if (collectedFundsImageExists == null)
                {
                    return Result.Fail<ReportMediaSettingsDto>(
                        ErrorMessagesConstants.NotFound(request.Dto.CollectedFundsBlock.ImageId.Value, typeof(Image)));
                }
            }

            Image? changedLivesImageExists = null;
            if (request.Dto.ChangedLivesBlock.ImageId.HasValue)
            {
                changedLivesImageExists = await _repositoryWrapper.ImageRepository
                    .GetFirstOrDefaultAsync(new QueryOptions<Image>
                    {
                        Filter = i => i.Id == request.Dto.ChangedLivesBlock.ImageId,
                        AsNoTracking = true
                    });

                if (changedLivesImageExists == null)
                {
                    return Result.Fail<ReportMediaSettingsDto>(
                        ErrorMessagesConstants.NotFound(request.Dto.ChangedLivesBlock.ImageId.Value, typeof(Image)));
                }
            }

            var collectedFundsRepository = _repositoryWrapper.GetRepository<CollectedFundsBlock>();
            var collectedFundsEntity = await collectedFundsRepository.GetFirstOrDefaultAsync();

            if (collectedFundsEntity == null)
            {
                collectedFundsEntity = new CollectedFundsBlock
                {
                    Title = request.Dto.CollectedFundsBlock.Title,
                    TitleEn = request.Dto.CollectedFundsBlock.TitleEn,
                    ImageId = request.Dto.CollectedFundsBlock.ImageId,
                };
                await collectedFundsRepository.CreateAsync(collectedFundsEntity);
            }
            else
            {
                collectedFundsEntity.Title = request.Dto.CollectedFundsBlock.Title;
                collectedFundsEntity.TitleEn = request.Dto.CollectedFundsBlock.TitleEn;
                collectedFundsEntity.ImageId = request.Dto.CollectedFundsBlock.ImageId;
                collectedFundsRepository.Update(collectedFundsEntity);
            }

            var changedLivesRepository = _repositoryWrapper.GetRepository<ChangedLivesBlock>();
            var changedLivesEntity = await changedLivesRepository.GetFirstOrDefaultAsync();

            if (changedLivesEntity == null)
            {
                changedLivesEntity = new ChangedLivesBlock
                {
                    Title = request.Dto.ChangedLivesBlock.Title,
                    TitleEn = request.Dto.ChangedLivesBlock.TitleEn,
                    ChangedLivesCount = request.Dto.ChangedLivesBlock.ChangedLives,
                    ImageId = request.Dto.ChangedLivesBlock.ImageId,
                };
                await changedLivesRepository.CreateAsync(changedLivesEntity);
            }
            else
            {
                changedLivesEntity.Title = request.Dto.ChangedLivesBlock.Title;
                changedLivesEntity.TitleEn = request.Dto.ChangedLivesBlock.TitleEn;
                changedLivesEntity.ChangedLivesCount = request.Dto.ChangedLivesBlock.ChangedLives;
                changedLivesEntity.ImageId = request.Dto.ChangedLivesBlock.ImageId;
                changedLivesRepository.Update(changedLivesEntity);
            }

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<ReportMediaSettingsDto>(
                    ErrorMessagesConstants.FailedToUpdateEntity(typeof(CollectedFundsBlock)));
            }

            if (collectedFundsEntity != null)
            {
                collectedFundsEntity.Image = collectedFundsImageExists;
            }

            if (changedLivesEntity != null)
            {
                changedLivesEntity.Image = changedLivesImageExists;
            }

            var resultDto = new ReportMediaSettingsDto
            {
                CollectedFundsBlock = _mapper.Map<CollectedFundsBlockDto>(collectedFundsEntity),
                ChangedLivesBlock = _mapper.Map<ChangedLivesBlockDto>(changedLivesEntity)
            };

            return Result.Ok(resultDto);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<ReportMediaSettingsDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportMediaSettingsDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(CollectedFundsBlock)));
        }
    }
}
