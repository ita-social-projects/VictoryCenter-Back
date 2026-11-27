using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.PdfSection;
using VictoryCenter.BLL.Errors;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using PdfSectionEntity = VictoryCenter.DAL.Entities.PdfSection;

namespace VictoryCenter.BLL.Queries.Public.PdfSection.Get;

public class GetPublicPdfSectionHandler
    : IRequestHandler<GetPublicPdfSectionQuery, Result<PublicPdfSectionDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetPublicPdfSectionHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<PublicPdfSectionDto>> Handle(
        GetPublicPdfSectionQuery request,
        CancellationToken cancellationToken)
    {
        var section = await _repositoryWrapper.PdfSectionRepository.GetFirstOrDefaultAsync(
            new QueryOptions<PdfSectionEntity>
            {
                AsNoTracking = true,
                Include = ps => ps.Include(s => s.Localizations).ThenInclude(l => l.Language),
            });

        if (section is null)
        {
            return Result.Fail<PublicPdfSectionDto>(new NotFoundError(PdfSectionConstants.SectionNotFound));
        }

        return Result.Ok(_mapper.Map<PublicPdfSectionDto>(section));
    }
}
