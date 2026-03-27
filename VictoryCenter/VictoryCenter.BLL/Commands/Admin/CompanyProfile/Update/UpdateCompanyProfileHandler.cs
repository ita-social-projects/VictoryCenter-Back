using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.CompanyProfile.Update;

public class UpdateCompanyProfileHandler : IRequestHandler<UpdateCompanyProfileCommand, Result<CompanyProfileDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILocalizationService<CompanyProfileContact, CompanyProfileContactLocalization> _localizationContactService;
    private readonly ILocalizationService<CompanyProfileRequisite, CompanyProfileRequisiteLocalization> _localizationRequisiteService;
    private readonly IValidator<UpdateCompanyProfileCommand> _validator;

    public UpdateCompanyProfileHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        ILocalizationService<CompanyProfileContact, CompanyProfileContactLocalization> localizationContactService,
        ILocalizationService<CompanyProfileRequisite, CompanyProfileRequisiteLocalization> localizationRequisiteService,
        IValidator<UpdateCompanyProfileCommand> validator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _localizationContactService = localizationContactService;
        _localizationRequisiteService = localizationRequisiteService;
        _validator = validator;
    }

    public async Task<Result<CompanyProfileDto>> Handle(UpdateCompanyProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var entity = await _repositoryWrapper.CompanyProfileRepository.GetFirstOrDefaultAsync(
                new QueryOptions<DAL.Entities.CompanyProfile>
                {
                    AsNoTracking = false,
                    Include = q => q
                        .Include(p => p.Contact)
                            .ThenInclude(c => c.Localizations)
                            .ThenInclude(l => l.Language)
                        .Include(p => p.Requisite)
                            .ThenInclude(r => r.Localizations)
                            .ThenInclude(l => l.Language)
                        .Include(p => p.SocialLinks)
                });

            if (entity is null)
            {
                return Result.Fail<CompanyProfileDto>(
                    ErrorMessagesConstants.NotFound());
            }

            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                _mapper.Map(request.UpdateCompanyProfileDto.Contacts, entity.Contact);
                _mapper.Map(request.UpdateCompanyProfileDto.Requisites, entity.Requisite);

                var linksByPlatform = entity.SocialLinks.ToDictionary(sl => sl.SocialPlatform);
                var contactLocalizationByLanguage = entity.Contact.Localizations.ToDictionary(l => l.LanguageId);
                var requisiteLocalizationByLanguage = entity.Requisite.Localizations.ToDictionary(l => l.LanguageId);

                var requestPlatforms = request.UpdateCompanyProfileDto.SocialLinks
                    .Select(sl => sl.SocialPlatform)
                    .ToHashSet();

                foreach (var (platform, linkToRemove) in linksByPlatform)
                {
                    if (!requestPlatforms.Contains(platform))
                    {
                        entity.SocialLinks.Remove(linkToRemove);
                    }
                }

                foreach (var dto in request.UpdateCompanyProfileDto.SocialLinks)
                {
                    if (linksByPlatform.TryGetValue(dto.SocialPlatform, out var existingLink))
                    {
                        _mapper.Map(dto, existingLink);
                        continue;
                    }

                    var link = _mapper.Map<CompanyProfileSocialLink>(dto);
                    link.ProfileId = entity.Id;
                    entity.SocialLinks.Add(link);
                }

                foreach (var localizationDto in request.UpdateCompanyProfileDto.Contacts.Localizations)
                {
                    if(contactLocalizationByLanguage.TryGetValue(localizationDto.LanguageId, out var existingLocalization))
                    {
                        _mapper.Map(localizationDto, existingLocalization);
                        continue;
                    }

                    var localization = _mapper.Map<CompanyProfileContactLocalization>(localizationDto);
                    localization.EntityId = entity.Contact.Id;
                    await _localizationContactService.TrackEntityLocalizationForUpdateAsync(localization);
                }

                foreach (var localizationDto in request.UpdateCompanyProfileDto.Requisites.Localizations)
                {
                    if(requisiteLocalizationByLanguage.TryGetValue(localizationDto.LanguageId, out var existingLocalization))
                    {
                        _mapper.Map(localizationDto, existingLocalization);
                        continue;
                    }

                    var localization = _mapper.Map<CompanyProfileRequisiteLocalization>(localizationDto);
                    localization.EntityId = entity.Requisite.Id;
                    await _localizationRequisiteService.TrackEntityLocalizationForUpdateAsync(localization);
                }

                await _repositoryWrapper.SaveChangesAsync();

                scope.Complete();
            }

            var created = await _repositoryWrapper.CompanyProfileRepository.GetFirstOrDefaultAsync(
                new QueryOptions<DAL.Entities.CompanyProfile>
                {
                    Filter = p => p.Id == entity.Id,
                    Include = q => q
                        .Include(p => p.Contact)
                            .ThenInclude(c => c.Localizations)
                            .ThenInclude(l => l.Language)
                        .Include(p => p.Requisite)
                            .ThenInclude(r => r.Localizations)
                            .ThenInclude(l => l.Language)
                        .Include(p => p.SocialLinks)
                });

            return Result.Ok(_mapper.Map<CompanyProfileDto>(created));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<CompanyProfileDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<CompanyProfileDto>(
                ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(DAL.Entities.CompanyProfile)));
        }
    }
}
