using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
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

        var dto = new PdfSectionWithReportsDto
        {
            Title = section.Title,
            Description = section.Description,
        };

        return Result.Ok(dto);
    }
}
