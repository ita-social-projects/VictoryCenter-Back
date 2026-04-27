using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.PdfSection.GetByEntityId;

public class GetPdfSectionLocalizationByEntityIdHandler
    : IRequestHandler<GetPdfSectionLocalizationByEntityIdQuery, Result<List<PdfSectionLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetPdfSectionLocalizationByEntityIdHandler(IMapper mapper, IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<List<PdfSectionLocalizationDto>>> Handle(
        GetPdfSectionLocalizationByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<PdfSectionLocalization>
        {
            Filter = l => l.EntityId == request.Id,
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
