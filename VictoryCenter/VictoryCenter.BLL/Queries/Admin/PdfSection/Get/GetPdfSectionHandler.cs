using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;
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
            new QueryOptions<PdfSectionEntity>
            {
                AsNoTracking = true,
                Include = ps => ps.Include(ps => ps.Localizations).ThenInclude(l => l.Language)
            });

        if (section == null)
        {
            return Result.Fail<PdfSectionDto>(PdfSectionConstants.SectionNotFound);
        }

        var dto = new PdfSectionDto
        {
            Title = section.Title,
            Description = section.Description,
            Localizations = section.Localizations.Select(l => new PdfSectionLocalizationDto
            {
                LanguageId = l.LanguageId,
                Title = l.Title,
                Description = l.Description,
                TranslationStatus = l.TranslationStatus,
            }).ToList()
        };

        return Result.Ok(dto);
    }
}
