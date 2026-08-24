using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.BLL.Hubs;

namespace VictoryCenter.BLL.Commands.Admin.PdfReports.Reorder;

public class ReorderPdfReportsHandler : IRequestHandler<ReorderPdfReportsCommand, Result<Unit>>
{
    private readonly IValidator<ReorderPdfReportsCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;
    private readonly IHubContext<PdfReportsHub> _hubContext;

    public ReorderPdfReportsHandler(
        IValidator<ReorderPdfReportsCommand> validator,
        IRepositoryWrapper repositoryWrapper,
        IReorderService reorderService,
        IHubContext<PdfReportsHub> hubContext)
    {
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _reorderService = reorderService;
        _hubContext = hubContext;
    }

    public async Task<Result<Unit>> Handle(ReorderPdfReportsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var orderedIds = request.ReorderPdfReportsDto.OrderedIds;
            var languageId = request.ReorderPdfReportsDto.LanguageId;

            var languageExists = await _repositoryWrapper.LocalizationLanguagesRepository.ExistsAsync(l => l.Id == languageId);
            if (!languageExists)
            {
                return Result.Fail<Unit>(ErrorMessagesConstants.NotFound(languageId, typeof(LocalizationLanguage)));
            }

            var reportsToReorderCount = await _repositoryWrapper.PdfReportRepository.CountAsync(
                new QueryOptions<PdfReport>
                {
                    Filter = e => e.LanguageId == languageId && orderedIds.Contains(e.Id)
                });

            if (reportsToReorderCount == 0)
            {
                return Result.Fail<Unit>(PdfReportConstants.PdfNotFound);
            }

            if (reportsToReorderCount != orderedIds.Count)
            {
                return Result.Fail<Unit>(ReorderConstants.NotAllEntitiesFoundForReorder(reportsToReorderCount, orderedIds.Count));
            }

            await _reorderService.SwapElementsAsync<PdfReport>(
                idsOrder: orderedIds,
                idSelector: e => e.Id,
                groupSelector: e => e.LanguageId == languageId);

            await _hubContext.Clients.All.SendAsync("PdfReportAction", languageId, cancellationToken: cancellationToken);
            return Result.Ok(Unit.Value);
        }
        catch (ValidationException ex)
        {
            return Result.Fail<Unit>(ex.Message);
        }
        catch (ReorderException ex)
        {
            return Result.Fail<Unit>(ReorderConstants.ErrorWithReordering(ex.Message));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<Unit>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PdfReport)));
        }
    }
}
