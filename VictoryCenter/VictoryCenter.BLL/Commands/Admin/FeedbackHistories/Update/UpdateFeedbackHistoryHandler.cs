using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Update;

public class UpdateFeedbackHistoryHandler : IRequestHandler<UpdateFeedbackHistoryCommand, Result<FeedbackHistoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateFeedbackHistoryCommand> _validator;

    public UpdateFeedbackHistoryHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateFeedbackHistoryCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<FeedbackHistoryDto>> Handle(UpdateFeedbackHistoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var imageResult = await ImageValidationHelper.ValidateAndGetImageAsync(
                _repositoryWrapper,
                request.UpdateFeedbackHistoryDto.ImageId);

            if (imageResult.IsFailed)
            {
                return Result.Fail<FeedbackHistoryDto>(imageResult.Errors);
            }

            var entity = await _repositoryWrapper.FeedbackHistoriesRepository
                .GetFirstOrDefaultAsync(new QueryOptions<FeedbackHistory>
                {
                    Filter = x => x.Id == request.Id
                });

            if (entity == null)
            {
                return Result.Fail<FeedbackHistoryDto>(ErrorMessagesConstants
                    .NotFound(request.Id, typeof(FeedbackHistory)));
            }

            var entityToUpdate = _mapper.Map(request.UpdateFeedbackHistoryDto, entity);
            entityToUpdate.Image = imageResult.Value;

            _repositoryWrapper.FeedbackHistoriesRepository.Update(entityToUpdate);

            if (await _repositoryWrapper.SaveChangesAsync() > 0)
            {
                var updatedEntityDto = _mapper.Map<FeedbackHistoryDto>(entityToUpdate);
                return Result.Ok(updatedEntityDto);
            }

            return Result.Fail<FeedbackHistoryDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(FeedbackHistory)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<FeedbackHistoryDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
    }
}