using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Interfaces.MainPage;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Update;

public class UpdateMainPageLocalizationHandler : IRequestHandler<UpdateMainPageLocalizationCommand, Result<MainPageLocalizationDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<UpdateMainPageLocalizationCommand> _validator;
    private readonly ILocalizationService<MainPageEntity, MainPageLocalization> _mainPageService;
    private readonly IMainPageBlocksLocalizationUpdater _blocksUpdater;

    public UpdateMainPageLocalizationHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IValidator<UpdateMainPageLocalizationCommand> validator,
        ILocalizationService<MainPageEntity, MainPageLocalization> mainPageService,
        IMainPageBlocksLocalizationUpdater blocksUpdater)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _validator = validator;
        _mainPageService = mainPageService;
        _blocksUpdater = blocksUpdater;
    }

    public async Task<Result<MainPageLocalizationDto>> Handle(UpdateMainPageLocalizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            using var transaction = _repositoryWrapper.BeginTransaction();

            var mainPage = await _repositoryWrapper.MainPageRepository
                .GetFirstOrDefaultAsync(new QueryOptions<MainPageEntity>
                {
                    Filter = e => e.Id == request.EntityId,
                    Include = query => query
                        .Include(e => e.MainAboutUs)
                        .Include(e => e.MainPartners)
                        .Include(e => e.MainDonations)
                });

            if (mainPage is null)
            {
                return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.NotFound());
            }

            var dto = request.Dto;

            var blocksUpdateResult = await _blocksUpdater.UpdateBlocksAsync(dto, mainPage, request.LanguageId);
            if (blocksUpdateResult.IsFailed)
            {
                return Result.Fail<MainPageLocalizationDto>(blocksUpdateResult.Errors);
            }

            MainPageLocalization mainPageLocalizationEntity = _mapper.Map<MainPageLocalization>(dto);
            mainPageLocalizationEntity.EntityId = request.EntityId;
            mainPageLocalizationEntity.LanguageId = request.LanguageId;
            var updatedMainPageLocalization = await _mainPageService.UpdateEntityLocalizationAsync(mainPageLocalizationEntity);

            transaction.Complete();

            var blocks = blocksUpdateResult.Value;
            var response = _mapper.Map<MainPageLocalizationDto>(updatedMainPageLocalization) with
            {
                MainAboutUs = blocks.MainAboutUs,
                MainPartners = blocks.MainPartners,
                MainDonations = blocks.MainDonations
            };

            return Result.Ok(response);
        }
        catch (KeyNotFoundException knfex)
        {
            return Result.Fail<MainPageLocalizationDto>(knfex.Message);
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntity(typeof(MainPageLocalization)));
        }
        catch (ValidationException vex)
        {
            return Result.Fail<MainPageLocalizationDto>(vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DbUpdateException)
        {
            return Result.Fail<MainPageLocalizationDto>(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(MainPageLocalization)));
        }
    }
}
