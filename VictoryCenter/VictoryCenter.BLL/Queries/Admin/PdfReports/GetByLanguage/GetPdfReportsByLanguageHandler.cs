using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.PdfReports.GetByLanguage;

public class GetPdfReportsByLanguageHandler : IRequestHandler<GetPdfReportsByLanguageQuery, Result<List<PdfReportDto>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetPdfReportsByLanguageHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<List<PdfReportDto>>> Handle(
        GetPdfReportsByLanguageQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<PdfReport>
        {
            Filter = p => p.LanguageId == request.LanguageId,
            OrderByASC = p => p.Priority,
            Include = pr => pr.Include(p => p.Language)
        };

        var pdfReports = await _repositoryWrapper.PdfReportRepository
            .GetAllAsync(queryOptions);

        var result = _mapper.Map<List<PdfReportDto>>(pdfReports);
        return Result.Ok(result);
    }
}
