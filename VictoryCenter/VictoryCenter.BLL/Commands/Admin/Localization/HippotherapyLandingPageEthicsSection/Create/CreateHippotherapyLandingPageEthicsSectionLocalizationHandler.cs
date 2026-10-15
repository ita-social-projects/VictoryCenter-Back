using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.Interfaces.HippotherapyLandingPages;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageEthicsSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageEthicsSection;

namespace VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageEthicsSection.Create;

public class CreateHippotherapyLandingPageEthicsSectionLocalizationHandler
    : IRequestHandler<CreateHippotherapyLandingPageEthicsSectionLocalizationCommand, Result<HippotherapyLandingPageEthicsSectionLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly ILocalizationService<HippotherapyLandingPageEthicsSectionEntity, HippotherapyLandingPageEthicsSectionLocalization> _localizationService;
    private readonly IValidator<CreateHippotherapyLandingPageEthicsSectionLocalizationCommand> _validator;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IHippotherapyLandingPageEthicsPrincipleLocalizationUpdater _principlesUpdater;

    public CreateHippotherapyLandingPageEthicsSectionLocalizationHandler(
        IMapper mapper,
        ILocalizationService<HippotherapyLandingPageEthicsSectionEntity, HippotherapyLandingPageEthicsSectionLocalization> localizationService,
        IValidator<CreateHippotherapyLandingPageEthicsSectionLocalizationCommand> validator,
        IRepositoryWrapper repositoryWrapper,
        IHippotherapyLandingPageEthicsPrincipleLocalizationUpdater principlesUpdater)
    {
        _mapper = mapper;
        _localizationService = localizationService;
        _validator = validator;
        _repositoryWrapper = repositoryWrapper;
        _principlesUpdater = principlesUpdater;
    }

    public async Task<Result<HippotherapyLandingPageEthicsSectionLocalizationDto>> Handle(
        CreateHippotherapyLandingPageEthicsSectionLocalizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var dto = request.CreateHippotherapyLandingPageEthicsSectionLocalizationDto;

            var section = await GetSectionWithPrinciplesAsync(dto.EntityId);
            if (section is null)
            {
                return Result.Fail<HippotherapyLandingPageEthicsSectionLocalizationDto>(
                    ErrorMessagesConstants.NotFound(dto.EntityId, typeof(HippotherapyLandingPageEthicsSectionEntity)));
            }

            using var transaction = _repositoryWrapper.BeginTransaction();

            var entity = _mapper.Map<HippotherapyLandingPageEthicsSectionLocalization>(dto);
            var result = await _localizationService.CreateEntityLocalizationAsync(entity);

            var principlesResult = await _principlesUpdater.UpsertPrinciplesAsync(section, dto.Principles, dto.LanguageId);
            if (principlesResult.IsFailed)
            {
                return Result.Fail<HippotherapyLandingPageEthicsSectionLocalizationDto>(principlesResult.Errors);
            }

            transaction.Complete();

            var responseDto = _mapper.Map<HippotherapyLandingPageEthicsSectionLocalizationDto>(result);
            responseDto.Principles = principlesResult.Value;
            return Result.Ok(responseDto);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<HippotherapyLandingPageEthicsSectionLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<HippotherapyLandingPageEthicsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageEthicsSectionLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<HippotherapyLandingPageEthicsSectionLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<HippotherapyLandingPageEthicsSectionLocalizationDto>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageEthicsSectionLocalization)));
        }
    }

    private Task<HippotherapyLandingPageEthicsSectionEntity?> GetSectionWithPrinciplesAsync(long sectionId)
    {
        return _repositoryWrapper.HippotherapyLandingPageEthicsSectionsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<HippotherapyLandingPageEthicsSectionEntity>
            {
                Filter = s => s.Id == sectionId,
                Include = q => q.Include(s => s.EthicsPrinciples)
            });
    }
}
