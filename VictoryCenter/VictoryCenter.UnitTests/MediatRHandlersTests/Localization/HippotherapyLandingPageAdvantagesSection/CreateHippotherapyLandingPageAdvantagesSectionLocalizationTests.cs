using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAdvantagesSection.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageAdvantagesSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageAdvantagesSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageAdvantagesSection;

public class CreateHippotherapyLandingPageAdvantagesSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageAdvantagesSectionEntity, HippotherapyLandingPageAdvantagesSectionLocalization>> _mockLocalizationService;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageAdvantageCard, HippotherapyLandingPageAdvantageCardLocalization>> _mockCardLocalizationService;
    private readonly IValidator<CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand> _validator;
    private readonly CreateHippotherapyLandingPageAdvantagesSectionLocalizationHandler _handler;

    private readonly HippotherapyLandingPageAdvantagesSectionEntity _section = new()
    {
        Id = 1,
        AdvantageCards =
        [
            new HippotherapyLandingPageAdvantageCard { Id = 10, AdvantagesSectionId = 1, Description = "Original card 1", Priority = 1 },
            new HippotherapyLandingPageAdvantageCard { Id = 11, AdvantagesSectionId = 1, Description = "Original card 2", Priority = 2 },
        ],
    };

    private readonly CreateHippotherapyLandingPageAdvantagesSectionLocalizationDto _createDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Why this approach",
        Cards =
        [
            new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 10, Description = "Translated card description 1" },
            new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 11, Description = "Translated card description 2" },
        ],
    };

    private readonly HippotherapyLandingPageAdvantagesSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Why this approach",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageAdvantagesSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Why this approach",
    };

    public CreateHippotherapyLandingPageAdvantagesSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageAdvantagesSectionEntity, HippotherapyLandingPageAdvantagesSectionLocalization>>();
        _mockCardLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageAdvantageCard, HippotherapyLandingPageAdvantageCardLocalization>>();
        _validator = new CreateHippotherapyLandingPageAdvantagesSectionLocalizationValidator(
            new BaseHippotherapyLandingPageAdvantagesSectionLocalizationValidator(
                new HippotherapyLandingPageAdvantageCardLocalizationItemValidator()));

        var cardsUpdater = new HippotherapyLandingPageAdvantageCardLocalizationUpdater(
            _mockMapper.Object, _mockRepositoryWrapper.Object, _mockCardLocalizationService.Object);

        _handler = new CreateHippotherapyLandingPageAdvantagesSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator, _mockRepositoryWrapper.Object, cardsUpdater);

        _mockRepositoryWrapper.Setup(r => r.BeginTransaction())
            .Returns(() => new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageAdvantagesSectionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantagesSectionEntity>>()))
            .ReturnsAsync(_section);
    }

    [Fact]
    public async Task Handle_ShouldCreateSectionAndCardLocalizations_Successfully()
    {
        SetupDependencies(existingCardLocalization: false);

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_responseDto.Title, result.Value.Title);
        Assert.Equal(_responseDto.EntityId, result.Value.EntityId);
        Assert.Equal(new[] { 10L, 11L }, result.Value.Cards.Select(c => c.CardId));
        _mockCardLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(
                It.Is<IEnumerable<HippotherapyLandingPageAdvantageCardLocalization>>(l => l.Count() == 2 && l.All(x => x.LanguageId == 2)),
                false),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateExistingCardLocalizations_WhenTheyAlreadyExist()
    {
        SetupDependencies(existingCardLocalization: true);

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _mockCardLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageAdvantageCardLocalization>>(), true),
            Times.Once);
        _mockCardLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageAdvantageCardLocalization>>(), false),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateOnlySectionLocalization_WhenCardsAreEmpty()
    {
        SetupDependencies(existingCardLocalization: false);
        var dto = new CreateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            EntityId = _createDto.EntityId,
            LanguageId = _createDto.LanguageId,
            Title = _createDto.Title,
            Cards = [],
        };
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAdvantagesSectionLocalization>(dto)).Returns(_entity);

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(dto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Cards);
        _mockCardLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageAdvantageCardLocalization>>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSectionNotFound()
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageAdvantagesSectionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantagesSectionEntity>>()))
            .ReturnsAsync((HippotherapyLandingPageAdvantagesSectionEntity?)null);

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        _mockLocalizationService.Verify(
            s => s.CreateEntityLocalizationAsync(It.IsAny<HippotherapyLandingPageAdvantagesSectionLocalization>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCardDoesNotBelongToSection()
    {
        SetupDependencies(existingCardLocalization: false);
        var dto = new CreateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            EntityId = _createDto.EntityId,
            LanguageId = _createDto.LanguageId,
            Title = _createDto.Title,
            Cards = [new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 999, Description = "Translated card description" }],
        };
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAdvantagesSectionLocalization>(dto)).Returns(_entity);

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(dto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        _mockCardLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageAdvantageCardLocalization>>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new CreateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            Title = "",
            Cards =
            [
                new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 10, Description = "Translated card description" },
                new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 10, Description = "Translated card description" },
            ],
        };

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(invalidDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("EntityId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("LanguageId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("Title is required", result.Errors.Select(e => e.Message));
        Assert.Contains(
            ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto.Cards)),
            result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAdvantagesSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Entity not found"));

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAdvantagesSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageAdvantagesSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAdvantagesSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageAdvantagesSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(bool existingCardLocalization)
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAdvantagesSectionLocalization>(_createDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAdvantagesSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);

        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageAdvantageCardLocalization>(It.IsAny<UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto>()))
            .Returns((object src) =>
            {
                var dto = (UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto)src;
                return new HippotherapyLandingPageAdvantageCardLocalization
                {
                    EntityId = dto.CardId,
                    Description = dto.Description,
                    TranslationStatus = TranslationStatus.Relevant,
                };
            });
        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageAdvantageCardLocalizationItemDto>(It.IsAny<HippotherapyLandingPageAdvantageCardLocalization>()))
            .Returns((object src) =>
            {
                var entity = (HippotherapyLandingPageAdvantageCardLocalization)src;
                return new HippotherapyLandingPageAdvantageCardLocalizationItemDto
                {
                    CardId = entity.EntityId,
                    Description = entity.Description,
                    TranslationStatus = entity.TranslationStatus,
                };
            });

        var existing = existingCardLocalization
            ? _createDto.Cards
                .Select(c => new HippotherapyLandingPageAdvantageCardLocalization
                {
                    EntityId = c.CardId,
                    LanguageId = _createDto.LanguageId,
                    Description = "Old translation",
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
                })
                .ToList()
            : [];
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageAdvantageCardLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantageCardLocalization>>()))
            .ReturnsAsync(existing);
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
    }
}
