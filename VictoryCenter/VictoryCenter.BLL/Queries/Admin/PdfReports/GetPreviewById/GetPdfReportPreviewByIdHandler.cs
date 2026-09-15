using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.PdfStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.GetPreviewById;

public class GetPdfReportPreviewByIdHandler : IRequestHandler<GetPdfReportPreviewByIdQuery, Result<PdfReportFileDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IPdfService _pdfService;

    public GetPdfReportPreviewByIdHandler(
        IRepositoryWrapper repositoryWrapper,
        IPdfService pdfService)
    {
        _repositoryWrapper = repositoryWrapper;
        _pdfService = pdfService;
    }

    public async Task<Result<PdfReportFileDto>> Handle(GetPdfReportPreviewByIdQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<PdfReport>
        {
            AsNoTracking = true,
            Filter = p => p.Id == request.Id,
            Include = pr => pr.Include(p => p.Language)
        };

        var pdfReport = await _repositoryWrapper.PdfReportRepository.GetFirstOrDefaultAsync(queryOptions);

        if (pdfReport == null)
        {
            return Result.Fail<PdfReportFileDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(PdfReport)));
        }

        try
        {
            var pdfStream = await _pdfService.GetPdfAsync(pdfReport.BlobName);

            var fileName = pdfReport.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                ? pdfReport.Name
                : $"{pdfReport.Name}.pdf";

            return Result.Ok(new PdfReportFileDto
            {
                FileStream = pdfStream,
                FileName = fileName
            });
        }
        catch (BlobNotFoundException)
        {
            return Result.Fail<PdfReportFileDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(PdfReport)));
        }
        catch (BlobFileSystemException)
        {
            return Result.Fail<PdfReportFileDto>(ErrorMessagesConstants.FailedToRetrievePdf());
        }
    }
}
