using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageParticipantsSection.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageParticipantsSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageParticipantsSection;

public class UpdateHippotherapyLandingPageParticipantsSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization>> _mockLocalizationService;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageParticipantCard, HippotherapyLandingPageParticipantCardLocalization>> _mockCardLocalizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand> _validator;
    private readonly UpdateHippotherapyLandingPageParticipantsSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly HippotherapyLandingPageParticipantsSectionEntity _section = new()
    {
        Id = 1,
        ParticipantCards =
        [
            new HippotherapyLandingPageParticipantCard { Id = 10, ParticipantsSectionId = 1, Description = "Original card 1", Priority = 1 },
            new HippotherapyLandingPageParticipantCard { Id = 11, ParticipantsSectionId = 1, Description = "Original card 2", Priority = 2 },
        ],
    };

    private readonly UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto _updateDto = new()
    {
        Title = "Updated title",
        Cards =
        [
            new UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto { CardId = 10, Description = "Updated card description 1" },
            new UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto { CardId = 11, Description = "Updated card description 2" },
        ],
    };

    private readonly HippotherapyLandingPageParticipantsSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Updated title",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageParticipantsSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Updated title",
    };

    public UpdateHippotherapyLandingPageParticipantsSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageParticipantsSectionEntity, HippotherapyLandingPageParticipantsSectionLocalization>>();
        _mockCardLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageParticipantCard, HippotherapyLandingPageParticipantCardLocalization>>();
        _validator = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationValidator(
            new BaseHippotherapyLandingPageParticipantsSectionLocalizationValidator(
                new HippotherapyLandingPageParticipantCardLocalizationItemValidator()));

        var cardsUpdater = new HippotherapyLandingPageParticipantCardLocalizationUpdater(
            _mockMapper.Object, _mockRepositoryWrapper.Object, _mockCardLocalizationService.Object);

        _handler = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator, _mockRepositoryWrapper.Object, cardsUpdater);

        _mockRepositoryWrapper.Setup(r => r.BeginTransaction())
            .Returns(() => new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageParticipantsSectionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageParticipantsSectionEntity>>()))
            .ReturnsAsync(_section);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalization_Successfully()
    {
        SetupDependencies(existingCardLocalization: true);

        var command = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_responseDto.Title, result.Value.Title);
        Assert.Equal(_entityId, result.Value.EntityId);
        Assert.Equal(new[] { 10L, 11L }, result.Value.Cards.Select(c => c.CardId));
        Assert.All(result.Value.Cards, c => Assert.Equal(TranslationStatus.Relevant, c.TranslationStatus));
        _mockCardLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(
                It.Is<IEnumerable<HippotherapyLandingPageParticipantCardLocalization>>(l => l.Count() == 2 && l.All(x => x.LanguageId == _languageId)),
                true),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateMissingCardLocalizations_WhenTheyDoNotExist()
    {
        SetupDependencies(existingCardLocalization: false);

        var command = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _mockCardLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageParticipantCardLocalization>>(), false),
            Times.Once);
        _mockCardLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageParticipantCardLocalization>>(), true),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSectionNotFound()
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageParticipantsSectionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageParticipantsSectionEntity>>()))
            .ReturnsAsync((HippotherapyLandingPageParticipantsSectionEntity?)null);

        var command = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        _mockLocalizationService.Verify(
            s => s.UpdateEntityLocalizationAsync(It.IsAny<HippotherapyLandingPageParticipantsSectionLocalization>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCardDoesNotBelongToSection()
    {
        SetupDependencies(existingCardLocalization: false);
        var dto = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto
        {
            Title = _updateDto.Title,
            Cards = [new UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto { CardId = 999, Description = "Updated card description" }],
        };
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageParticipantsSectionLocalization>(dto)).Returns(_entity);

        var command = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand(dto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        _mockCardLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageParticipantCardLocalization>>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto
        {
            Title = "",
            Cards = [new UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto { CardId = 0, Description = "" }],
        };
        var command = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand(invalidDto, _entityId, _languageId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Title is required", result.Errors.Select(e => e.Message));
        Assert.Contains("CardId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("Description is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageParticipantsSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Localization not found"));

        var command = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Localization not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageParticipantsSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageParticipantsSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageParticipantsSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var command = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageParticipantsSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(bool existingCardLocalization)
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageParticipantsSectionLocalization>(_updateDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageParticipantsSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);

        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageParticipantCardLocalization>(It.IsAny<UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto>()))
            .Returns((object src) =>
            {
                var dto = (UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto)src;
                return new HippotherapyLandingPageParticipantCardLocalization
                {
                    EntityId = dto.CardId,
                    Description = dto.Description,
                    TranslationStatus = TranslationStatus.Relevant,
                };
            });
        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageParticipantCardLocalizationItemDto>(It.IsAny<HippotherapyLandingPageParticipantCardLocalization>()))
            .Returns((object src) =>
            {
                var entity = (HippotherapyLandingPageParticipantCardLocalization)src;
                return new HippotherapyLandingPageParticipantCardLocalizationItemDto
                {
                    CardId = entity.EntityId,
                    Description = entity.Description,
                    TranslationStatus = entity.TranslationStatus,
                };
            });

        var existing = existingCardLocalization
            ? _updateDto.Cards
                .Select(c => new HippotherapyLandingPageParticipantCardLocalization
                {
                    EntityId = c.CardId,
                    LanguageId = _languageId,
                    Description = "Old translation",
                    TranslationStatus = TranslationStatus.Outdated,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
                })
                .ToList()
            : [];
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageParticipantCardLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageParticipantCardLocalization>>()))
            .ReturnsAsync(existing);
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
    }
}
