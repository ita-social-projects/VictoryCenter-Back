using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfSection;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using PdfSectionEntity = VictoryCenter.DAL.Entities.PdfSection;

namespace VictoryCenter.BLL.Queries.Admin.PdfSection.Get;

public class GetPdfSectionHandler
    : IRequestHandler<GetPdfSectionQuery, Result<PdfSectionDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPdfSectionHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PdfSectionDto>> Handle(
        GetPdfSectionQuery request,
        CancellationToken cancellationToken)
    {
        var section = await _repositoryWrapper.PdfSectionRepository.GetFirstOrDefaultAsync(
            new QueryOptions<PdfSectionEntity> { AsNoTracking = true });

        if (section == null)
        {
            return Result.Fail<PdfSectionDto>(PdfSectionConstants.SectionNotFound);
        }

        var dto = new PdfSectionDto
        {
            Title = section.Title,
            Description = section.Description,
        };

        return Result.Ok(dto);
    }
}
