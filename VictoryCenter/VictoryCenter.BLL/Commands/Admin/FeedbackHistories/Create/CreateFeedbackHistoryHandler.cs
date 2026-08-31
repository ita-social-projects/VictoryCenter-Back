using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Create;

public class CreateFeedbackHistoryHandler : IRequestHandler<CreateFeedbackHistoryCommand, Result<FeedbackHistoryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<CreateFeedbackHistoryCommand> _validator;
    private readonly TimeProvider _timeProvider;

    public CreateFeedbackHistoryHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<CreateFeedbackHistoryCommand> validator,
        TimeProvider timeProvider)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Handles the creation of a new FeedbackHistory entity.
    /// </summary>
    /// <param name="request">The create command request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing the created FeedbackHistoryDto or errors.</returns>
    public async Task<Result<FeedbackHistoryDto>> Handle(CreateFeedbackHistoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var imageResult = await ImageValidationHelper.ValidateAndGetImageAsync(
                _repositoryWrapper,
                request.CreateFeedbackHistoryDto.ImageId);

            if (imageResult.IsFailed)
            {
                return Result.Fail<FeedbackHistoryDto>(imageResult.Errors);
            }

            var entity = _mapper.Map<FeedbackHistory>(request.CreateFeedbackHistoryDto);
            entity.CreatedAt = _timeProvider.GetUtcNow();
            entity.Image = imageResult.Value;

            await _repositoryWrapper.FeedbackHistoriesRepository.CreateAsync(entity);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<FeedbackHistoryDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(FeedbackHistory)));
            }

            var result = _mapper.Map<FeedbackHistoryDto>(entity);

            return Result.Ok(result);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<FeedbackHistoryDto>(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(FeedbackHistory)));
        }
        catch (ValidationException ex)
        {
            return Result.Fail<FeedbackHistoryDto>(ex.Errors.Select(e => e.ErrorMessage));
        }
    }
}
