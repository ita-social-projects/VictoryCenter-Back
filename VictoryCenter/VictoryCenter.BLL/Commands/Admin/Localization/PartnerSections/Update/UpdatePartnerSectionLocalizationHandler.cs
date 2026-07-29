using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Interfaces.Partners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Update;

public class UpdatePartnerSectionLocalizationHandler
    : IRequestHandler<UpdatePartnerSectionLocalizationCommand, Result<PartnerSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IValidator<UpdatePartnerSectionLocalizationCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILocalizationService<PartnerSection, PartnerSectionLocalization> _sectionLocalizationService;
    private readonly IPartnerSectionLocalizationUpdater _partnersUpdater;

    public UpdatePartnerSectionLocalizationHandler(
        IMapper mapper,
        IValidator<UpdatePartnerSectionLocalizationCommand> validator,
        IRepositoryWrapper repositoryWrapper,
        ILocalizationService<PartnerSection, PartnerSectionLocalization> sectionLocalizationService,
        IPartnerSectionLocalizationUpdater partnersUpdater)
    {
        _mapper = mapper;
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _sectionLocalizationService = sectionLocalizationService;
        _partnersUpdater = partnersUpdater;
    }

    public async Task<Result<PartnerSectionLocalizationDto>> Handle(UpdatePartnerSectionLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var section = await _repositoryWrapper.PartnerSectionsRepository
                .GetFirstOrDefaultAsync(new QueryOptions<PartnerSection>
                {
                    Filter = s => s.Id == request.EntityId,
                    Include = q => q.Include(s => s.Partners)
                });

            if (section is null)
            {
                return Result.Fail<PartnerSectionLocalizationDto>(ErrorMessagesConstants.NotFound(request.EntityId, typeof(PartnerSection)));
            }

            using var transaction = _repositoryWrapper.BeginTransaction();

            var dto = request.UpdatePartnerSectionLocalizationDto;
            var sectionEntity = _mapper.Map<PartnerSectionLocalization>(dto);
            sectionEntity.EntityId = request.EntityId;
            sectionEntity.LanguageId = request.LanguageId;
            var updatedSection = await _sectionLocalizationService.UpdateEntityLocalizationAsync(sectionEntity);

            var partnersResult = await _partnersUpdater.UpsertPartnersAsync(section, dto.Partners, request.LanguageId);
            if (partnersResult.IsFailed)
            {
                return Result.Fail<PartnerSectionLocalizationDto>(partnersResult.Errors);
            }

            transaction.Complete();

            var response = _mapper.Map<PartnerSectionLocalizationDto>(updatedSection) with
            {
                Partners = partnersResult.Value
            };

            return Result.Ok(response);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<PartnerSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<PartnerSectionLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(PartnerSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<PartnerSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<PartnerSectionLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PartnerSectionLocalization)));
        }
    }
}
