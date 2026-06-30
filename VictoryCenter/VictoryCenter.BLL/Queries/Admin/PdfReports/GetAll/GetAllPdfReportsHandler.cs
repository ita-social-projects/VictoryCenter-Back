using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.GetAll;

public class GetAllPdfReportsHandler : IRequestHandler<GetAllPdfReportsQuery, Result<PaginationResult<PdfReportDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetAllPdfReportsHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<PaginationResult<PdfReportDto>>> Handle(GetAllPdfReportsQuery request, CancellationToken cancellationToken)
    {
        var languageId = request.FilterDto.LanguageId;

        var queryOptions = new QueryOptions<PdfReport>
        {
            Filter = languageId.HasValue ? p => p.LanguageId == languageId.Value : null,
            Include = q => q.Include(p => p.Language),
            Offset = request.FilterDto.Offset ?? 0,
            Limit = request.FilterDto.Limit ?? 20,
            OrderByASC = p => p.Priority
        };

        var countOptions = new QueryOptions<PdfReport>
        {
            Filter = languageId.HasValue ? p => p.LanguageId == languageId.Value : null
        };

        var pdfReports = await _repositoryWrapper.PdfReportRepository.GetAllAsync(queryOptions);
        var totalCount = await _repositoryWrapper.PdfReportRepository.CountAsync(countOptions);

        var result = _mapper.Map<List<PdfReportDto>>(pdfReports);
        return Result.Ok(new PaginationResult<PdfReportDto>([.. result], totalCount));
    }
}
