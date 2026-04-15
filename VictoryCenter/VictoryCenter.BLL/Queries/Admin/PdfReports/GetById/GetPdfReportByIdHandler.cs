using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.BLL.Interfaces.PdfStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.GetById;

public class GetPdfReportByIdHandler : IRequestHandler<GetPdfReportByIdQuery, Result<Stream>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IPdfService _pdfService;

    public GetPdfReportByIdHandler(
        IRepositoryWrapper repositoryWrapper,
        IPdfService pdfService)
    {
        _repositoryWrapper = repositoryWrapper;
        _pdfService = pdfService;
    }

    public async Task<Result<Stream>> Handle(GetPdfReportByIdQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<PdfReport>
        {
            AsNoTracking = true,
            Filter = p => p.Id == request.Id
        };
        var pdfReport = await _repositoryWrapper.PdfReportRepository
            .GetFirstOrDefaultAsync(queryOptions);

        if (pdfReport == null)
        {
            return Result.Fail<Stream>(ErrorMessagesConstants.NotFound(request.Id, typeof(PdfReport)));
        }

        try
        {
            var pdfStream = await _pdfService.GetPdfAsync(pdfReport.BlobName);
            return Result.Ok<Stream>(pdfStream);
        }
        catch (BlobNotFoundException)
        {
            return Result.Fail<Stream>(ErrorMessagesConstants.NotFound(request.Id, typeof(PdfReport)));
        }
        catch (BlobFileSystemException)
        {
            return Result.Fail<Stream>(ErrorMessagesConstants.FailedToRetrievePdf());
        }
    }
}
