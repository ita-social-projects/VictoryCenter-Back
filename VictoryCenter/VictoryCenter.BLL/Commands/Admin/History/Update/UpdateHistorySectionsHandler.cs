using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Commands.Admin.History.Update;

public class UpdateHistorySectionsHandler : IRequestHandler<UpdateHistorySectionsCommand, Result<List<HistorySectionDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateHistorySectionsCommand> _validator;

    public UpdateHistorySectionsHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateHistorySectionsCommand> validator)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
    }

    public async Task<Result<List<HistorySectionDto>>> Handle(
        UpdateHistorySectionsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var existingSections = await _repositoryWrapper.HistorySectionsRepository.GetAllAsync();

            var incomingSections = request.UpdateSections.ToList();

            var imagesByIdResult = await ImageValidationHelper.ValidateAndGetSectionImagesAsync(
                _repositoryWrapper,
                incomingSections.Cast<CreateHistorySectionDto>().ToList());

            if (imagesByIdResult.IsFailed)
            {
                return Result.Fail<List<HistorySectionDto>>(imagesByIdResult.Errors);
            }

            var now = DateTimeOffset.UtcNow;

            using var transaction = _repositoryWrapper.BeginTransaction();

            var finalSections = await ReplaceSections(
                existingSections.ToList(),
                incomingSections,
                now,
                imagesByIdResult.Value);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<List<HistorySectionDto>>(
                    ErrorMessagesConstants.FailedToUpdateEntity(typeof(HistorySection)));
            }

            transaction.Complete();

            return Result.Ok(_mapper.Map<List<HistorySectionDto>>(finalSections));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<List<HistorySectionDto>>(vex.Errors.Select(e => e.ErrorMessage));
        }
    }

    private async Task<List<HistorySection>> ReplaceSections(
        List<HistorySection> oldSections,
        List<UpdateHistorySectionDto> newSections,
        DateTimeOffset createdAt,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        if (oldSections.Count > 0)
        {
            _repositoryWrapper.HistorySectionsRepository.DeleteRange(oldSections);
        }

        var rebuiltSections = HistorySectionsBuilder.Build(
            newSections.Cast<CreateHistorySectionDto>().ToList(),
            createdAt,
            imagesById);

        if (rebuiltSections.Count > 0)
        {
            await _repositoryWrapper.HistorySectionsRepository.CreateRangeAsync(rebuiltSections);
        }

        return rebuiltSections;
    }
}
