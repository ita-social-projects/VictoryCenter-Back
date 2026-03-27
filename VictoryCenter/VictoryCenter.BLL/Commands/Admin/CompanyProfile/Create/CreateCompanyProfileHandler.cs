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

namespace VictoryCenter.BLL.Commands.Admin.CompanyProfile.Create;

public class CreateCompanyProfileHandler : IRequestHandler<CreateCompanyProfileCommand, Result<CompanyProfileDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCompanyProfileCommand> _validator;
    private readonly ILocalizationService<CompanyProfileContact, CompanyProfileContactLocalization> _localizationContactService;
    private readonly ILocalizationService<CompanyProfileRequisite, CompanyProfileRequisiteLocalization> _localizationRequisiteService;

    public CreateCompanyProfileHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IValidator<CreateCompanyProfileCommand> validator,
        ILocalizationService<CompanyProfileContact, CompanyProfileContactLocalization> localizationContactService,
        ILocalizationService<CompanyProfileRequisite, CompanyProfileRequisiteLocalization> localizationRequisite)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _validator = validator;
        _localizationContactService = localizationContactService;
        _localizationRequisiteService = localizationRequisite;
    }

    public async Task<Result<CompanyProfileDto>> Handle(CreateCompanyProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            if ((await _repositoryWrapper.CompanyProfileRepository.GetFirstOrDefaultAsync()) is not null)
            {
                return Result.Fail<CompanyProfileDto>(errorMessage: ErrorMessagesConstants.OnlyOneEntityOfTypeIsAllowed(nameof(DAL.Entities.CompanyProfile)));
            }

            var entity = _mapper.Map<DAL.Entities.CompanyProfile>(request.CreateCompanyProfileDto);

            using (var scope = _repositoryWrapper.BeginTransaction())
            {
                await _repositoryWrapper.CompanyProfileRepository.CreateAsync(entity);

                if (await _repositoryWrapper.SaveChangesAsync() <= 0)
                {
                    return Result.Fail<CompanyProfileDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(DAL.Entities.CompanyProfile)));
                }

                foreach (var localizationDto in request.CreateCompanyProfileDto.Contacts.Localizations)
                {
                    var localization = _mapper.Map<CompanyProfileContactLocalization>(localizationDto);
                    localization.EntityId = entity.Contact.Id;
                    await _localizationContactService.TrackEntityLocalizationAsync(localization);
                }

                foreach (var localizationDto in request.CreateCompanyProfileDto.Requisites.Localizations)
                {
                    var localization = _mapper.Map<CompanyProfileRequisiteLocalization>(localizationDto);
                    localization.EntityId = entity.Requisite.Id;
                    await _localizationRequisiteService.TrackEntityLocalizationAsync(localization);
                }

                await _repositoryWrapper.SaveChangesAsync();

                scope.Complete();
            }

            var created = await _repositoryWrapper.CompanyProfileRepository.GetFirstOrDefaultAsync(new QueryOptions<DAL.Entities.CompanyProfile>
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

            var resultDto = _mapper.Map<CompanyProfileDto>(created);
            return Result.Ok(resultDto);
        }
        catch (ValidationException vex)
        {
            return Result.Fail<CompanyProfileDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<CompanyProfileDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(DAL.Entities.CompanyProfile)));
        }
    }
}
