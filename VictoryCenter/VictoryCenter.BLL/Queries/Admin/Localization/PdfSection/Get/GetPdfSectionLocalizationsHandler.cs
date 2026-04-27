using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using PdfSectionEntity = VictoryCenter.DAL.Entities.PdfSection;

namespace VictoryCenter.BLL.Queries.Admin.Localization.PdfSection.Get;

public class GetPdfSectionLocalizationsHandler
    : IRequestHandler<GetPdfSectionLocalizationsQuery, Result<List<PdfSectionLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetPdfSectionLocalizationsHandler(IMapper mapper, IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<List<PdfSectionLocalizationDto>>> Handle(
        GetPdfSectionLocalizationsQuery request,
        CancellationToken cancellationToken)
    {
        var section = await _repository.PdfSectionRepository
            .GetFirstOrDefaultAsync(new QueryOptions<PdfSectionEntity> { AsNoTracking = true });

        if (section is null)
        {
            return Result.Fail<List<PdfSectionLocalizationDto>>(
                ErrorMessagesConstants.NotFound());
        }

        var queryOptions = new QueryOptions<PdfSectionLocalization>
        {
            Filter = l => l.EntityId == section.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };

        IEnumerable<PdfSectionLocalization> localizations =
            await _repository.PdfSectionLocalizationsRepository.GetAllAsync(queryOptions);

        List<PdfSectionLocalizationDto> localizationsDto =
            _mapper.Map<List<PdfSectionLocalizationDto>>(localizations);

        return Result.Ok(localizationsDto);
    }
}
