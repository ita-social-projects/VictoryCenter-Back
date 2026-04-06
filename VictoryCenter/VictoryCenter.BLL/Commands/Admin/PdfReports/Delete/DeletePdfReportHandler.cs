using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.PdfStorage;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.PdfReports.Delete;

public class DeletePdfReportHandler : IRequestHandler<DeletePdfReportCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IPdfService _pdfService;
    private readonly IReorderService _reorderService;

    public DeletePdfReportHandler(
        IRepositoryWrapper repositoryWrapper,
        IPdfService pdfService,
        IReorderService reorderService)
    {
        _repositoryWrapper = repositoryWrapper;
        _pdfService = pdfService;
        _reorderService = reorderService;
    }

    public async Task<Result<Unit>> Handle(
        DeletePdfReportCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var pdfReport = await _repositoryWrapper.PdfReportRepository.GetFirstOrDefaultAsync(
                new QueryOptions<PdfReport>
                {
                    Filter = pr => pr.Id == request.Id,
                    AsNoTracking = false
                });

            if (pdfReport == null)
            {
                return Result.Fail<Unit>(ErrorMessagesConstants.NotFound(request.Id, typeof(PdfReport)));
            }

            using var transaction = _repositoryWrapper.BeginTransaction();

            var blobName = pdfReport.BlobName;
            _repositoryWrapper.PdfReportRepository.Delete(pdfReport);

            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<Unit>(
                    ErrorMessagesConstants.FailedToDeleteEntity(typeof(PdfReport)));
            }

            _pdfService.DeletePdf(blobName);
            await _reorderService.RenumberPriorityAsync<PdfReport>();

            await _repositoryWrapper.SaveChangesAsync();

            transaction.Complete();

            return Result.Ok(Unit.Value);
        }
        catch (DbUpdateException)
        {
            return Result.Fail<Unit>(
                ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(PdfReport)));
        }
    }
}
