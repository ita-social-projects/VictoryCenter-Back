using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.PdfReports.GetByLanguageId;

public class GetPublicPdfReportsByLanguageIdHandler
    : IRequestHandler<GetPublicPdfReportsByLanguageIdQuery, Result<PaginationResult<PdfReportDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetPublicPdfReportsByLanguageIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<PaginationResult<PdfReportDto>>> Handle(
        GetPublicPdfReportsByLanguageIdQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<PdfReport>
        {
            Filter = p => p.LanguageId == request.LanguageId,
            Include = q => q.Include(p => p.Language),
            Offset = request.Filter.Offset ?? 0,
            Limit = request.Filter.Limit ?? 20,
            OrderByASC = p => p.Priority,
            AsNoTracking = true
        };

        var countOptions = new QueryOptions<PdfReport>
        {
            Filter = p => p.LanguageId == request.LanguageId,
            AsNoTracking = true
        };

        var pdfReports = await _repositoryWrapper.PdfReportRepository.GetAllAsync(queryOptions);
        var totalCount = await _repositoryWrapper.PdfReportRepository.CountAsync(countOptions);

        var result = _mapper.Map<List<PdfReportDto>>(pdfReports);
        return Result.Ok(new PaginationResult<PdfReportDto>([.. result], totalCount));
    }
}
