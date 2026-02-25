using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Admin.PdfSection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.PdfSectionWithReport;

public class GetPdfSectionWithReportsHandler
    : IRequestHandler<GetPdfSectionWithReportsQuery, Result<PdfSectionWithReportsDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPdfSectionWithReportsHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PdfSectionWithReportsDto>> Handle(
        GetPdfSectionWithReportsQuery request,
        CancellationToken cancellationToken)
    {
        var section = await _repositoryWrapper.PdfSectionRepository.GetFirstOrDefaultAsync(
            new QueryOptions<PdfSection> { AsNoTracking = true });

        if (section == null)
        {
            return Result.Fail<PdfSectionWithReportsDto>(PdfSectionConstants.SectionNotFound);
        }

        var pdfFiles = await _repositoryWrapper.PdfReportRepository.GetAllAsync(
            new QueryOptions<PdfReport>
            {
                OrderByASC = r => r.Priority,
                AsNoTracking = true
            });

        var dto = new PdfSectionWithReportsDto
        {
            Title = section.Title,
            Description = section.Description,
            PdfFiles = [.. pdfFiles.Select(r => new PdfReportDto
            {
                Id = r.Id,
                Name = r.Name,
                BlobName = r.BlobName,
                FileSizeBytes = r.FileSizeBytes,
                CreatedAt = r.CreatedAt,
                Priority = r.Priority
            })]
        };

        return Result.Ok(dto);
    }
}
