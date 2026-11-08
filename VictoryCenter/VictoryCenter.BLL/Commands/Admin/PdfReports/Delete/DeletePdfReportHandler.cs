using FluentResults;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Hubs;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.PdfReports.Delete;

public class DeletePdfReportHandler : IRequestHandler<DeletePdfReportCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IReorderService _reorderService;
    private readonly IHubContext<PdfReportsHub> _hubContext;

    public DeletePdfReportHandler(
        IRepositoryWrapper repositoryWrapper,
        IReorderService reorderService,
        IHubContext<PdfReportsHub> hubContext)
    {
        _repositoryWrapper = repositoryWrapper;
        _reorderService = reorderService;
        _hubContext = hubContext;
    }

    public async Task<Result<Unit>> Handle(
        DeletePdfReportCommand request,
        CancellationToken cancellationToken)
    {
        var pdfReport = await _repositoryWrapper.PdfReportRepository.GetFirstOrDefaultAsync(
            new QueryOptions<PdfReport>
            {
                Filter = pr => pr.Id == request.Id,
                AsNoTracking = false,
                Include = pr => pr.Include(p => p.Language)
            });

        if (pdfReport == null)
        {
            return Result.Fail<Unit>(ErrorMessagesConstants.NotFound(request.Id, typeof(PdfReport)));
        }

        try
        {
            using var transaction = _repositoryWrapper.BeginTransaction();

            _repositoryWrapper.PdfReportRepository.Delete(pdfReport);
            await _repositoryWrapper.SaveChangesAsync();
            await _reorderService.RenumberPriorityAsync<PdfReport>(
                p => p.LanguageId == pdfReport.LanguageId);
            transaction.Complete();
        }
        catch (DbUpdateException)
        {
            return Result.Fail<Unit>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(PdfReport)));
        }

        await _hubContext.Clients.All.SendAsync("PdfReportDeleted", request.Id, cancellationToken: cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
