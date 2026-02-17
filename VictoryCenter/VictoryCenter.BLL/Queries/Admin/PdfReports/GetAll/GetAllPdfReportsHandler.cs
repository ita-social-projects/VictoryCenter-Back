using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.PdfStorage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.GetAll;

public class GetAllPdfReportsHandler : IRequestHandler<GetAllPdfReportsQuery, Result<PaginationResult<PdfReportDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IPdfService _pdfService;
    private readonly IMapper _mapper;

    public GetAllPdfReportsHandler(
        IRepositoryWrapper repositoryWrapper,
        IPdfService pdfService,
        IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _pdfService = pdfService;
        _mapper = mapper;
    }

    public async Task<Result<PaginationResult<PdfReportDto>>> Handle(GetAllPdfReportsQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<PdfReport>
        {
            Offset = request.FilterDto.Offset ?? 0,
            Limit = request.FilterDto.Limit ?? 20,
            OrderByASC = p => p.Priority
        };

        var pdfReports = await _repositoryWrapper.PdfReportRepository
        .GetAllAsync(queryOptions);

        var totalCount = await _repositoryWrapper.PdfReportRepository
            .CountAsync();

        var result = _mapper.Map<List<PdfReportDto>>(pdfReports);
        return Result.Ok(new PaginationResult<PdfReportDto>([.. result], totalCount));
    }
}
