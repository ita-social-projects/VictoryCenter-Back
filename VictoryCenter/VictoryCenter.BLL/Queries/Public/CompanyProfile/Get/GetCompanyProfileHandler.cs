using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Public.CompanyProfile.Get;

public class GetCompanyProfileHandler : IRequestHandler<GetCompanyProfileQuery, Result<CompanyProfileDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetCompanyProfileHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<CompanyProfileDto>> Handle(GetCompanyProfileQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repositoryWrapper.CompanyProfileRepository.GetFirstOrDefaultAsync(
            new QueryOptions<DAL.Entities.CompanyProfile>
            {
                Include = q => q
                    .Include(p => p.Contact)
                        .ThenInclude(c => c.Localizations)
                        .ThenInclude(l => l.Language)
                    .Include(p => p.Requisite)
                        .ThenInclude(r => r.Localizations)
                        .ThenInclude(l => l.Language)
                    .Include(p => p.SocialLinks),
                AsNoTracking = true
            });

        if (entity is null)
        {
            return Result.Ok(new CompanyProfileDto
            {
                Contacts = new CompanyProfileContactsDto
                {
                    Phone = string.Empty,
                    Address = string.Empty,
                    Email = string.Empty,
                    CorrespondenceEmail = string.Empty,
                    Motto = string.Empty,
                    Localizations = []
                },
                Requisites = new CompanyProfileRequisiteDto
                {
                    Recipient = string.Empty,
                    Edrpou = string.Empty,
                    Address = string.Empty,
                    Localizations = []
                },
                SocialLinks = []
            });
        }

        return Result.Ok(_mapper.Map<CompanyProfileDto>(entity));
    }
}
