using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageScientificReferencesSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageScientificReferencesSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageScientificReferencesSection;

public class UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization>> _mockLocalizationService;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageScientificReference, HippotherapyLandingPageScientificReferenceLocalization>> _mockReferenceLocalizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand> _validator;
    private readonly UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly HippotherapyLandingPageScientificReferencesSectionEntity _section = new()
    {
        Id = 1,
        ScientificReferences =
        [
            new HippotherapyLandingPageScientificReference { Id = 10, ScientificReferencesSectionId = 1, Name = "Original reference 1", Priority = 1 },
            new HippotherapyLandingPageScientificReference { Id = 11, ScientificReferencesSectionId = 1, Name = "Original reference 2", Priority = 2 },
        ],
    };

    private readonly UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto _updateDto = new()
    {
        Title = "Updated title",
        Description = "Updated description of the section",
        References =
        [
            new UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto { ReferenceId = 10, Name = "Updated reference name 1" },
            new UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto { ReferenceId = 11, Name = "Updated reference name 2" },
        ],
    };

    private readonly HippotherapyLandingPageScientificReferencesSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Updated title",
        Description = "Updated description of the section",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageScientificReferencesSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Updated title",
        Description = "Updated description of the section",
    };

    public UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageScientificReferencesSectionEntity, HippotherapyLandingPageScientificReferencesSectionLocalization>>();
        _mockReferenceLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageScientificReference, HippotherapyLandingPageScientificReferenceLocalization>>();
        _validator = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationValidator(
            new BaseHippotherapyLandingPageScientificReferencesSectionLocalizationValidator(
                new HippotherapyLandingPageScientificReferenceLocalizationItemValidator()));

        var referencesUpdater = new HippotherapyLandingPageScientificReferenceLocalizationUpdater(
            _mockMapper.Object, _mockRepositoryWrapper.Object, _mockReferenceLocalizationService.Object);

        _handler = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator, _mockRepositoryWrapper.Object, referencesUpdater);

        _mockRepositoryWrapper.Setup(r => r.BeginTransaction())
            .Returns(() => new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageScientificReferencesSectionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageScientificReferencesSectionEntity>>()))
            .ReturnsAsync(_section);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalization_Successfully()
    {
        SetupDependencies(existingReferenceLocalization: true);

        var command = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_responseDto.Title, result.Value.Title);
        Assert.Equal(_entityId, result.Value.EntityId);
        Assert.Equal(new[] { 10L, 11L }, result.Value.References.Select(c => c.ReferenceId));
        Assert.All(result.Value.References, c => Assert.Equal(TranslationStatus.Relevant, c.TranslationStatus));
        _mockReferenceLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(
                It.Is<IEnumerable<HippotherapyLandingPageScientificReferenceLocalization>>(l => l.Count() == 2 && l.All(x => x.LanguageId == _languageId)),
                true),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateMissingReferenceLocalizations_WhenTheyDoNotExist()
    {
        SetupDependencies(existingReferenceLocalization: false);

        var command = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _mockReferenceLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageScientificReferenceLocalization>>(), false),
            Times.Once);
        _mockReferenceLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageScientificReferenceLocalization>>(), true),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSectionNotFound()
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageScientificReferencesSectionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageScientificReferencesSectionEntity>>()))
            .ReturnsAsync((HippotherapyLandingPageScientificReferencesSectionEntity?)null);

        var command = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        _mockLocalizationService.Verify(
            s => s.UpdateEntityLocalizationAsync(It.IsAny<HippotherapyLandingPageScientificReferencesSectionLocalization>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenReferenceDoesNotBelongToSection()
    {
        SetupDependencies(existingReferenceLocalization: false);
        var dto = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = _updateDto.Title,
            Description = _updateDto.Description,
            References = [new UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto { ReferenceId = 999, Name = "Updated reference name" }],
        };
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageScientificReferencesSectionLocalization>(dto)).Returns(_entity);

        var command = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(dto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        _mockReferenceLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageScientificReferenceLocalization>>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = "",
            Description = "",
            References = [new UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto { ReferenceId = 0, Name = "" }],
        };
        var command = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(invalidDto, _entityId, _languageId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Title is required", result.Errors.Select(e => e.Message));
        Assert.Contains("Description is required", result.Errors.Select(e => e.Message));
        Assert.Contains("ReferenceId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("Name is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageScientificReferencesSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Localization not found"));

        var command = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Localization not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageScientificReferencesSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageScientificReferencesSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var command = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageScientificReferencesSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(bool existingReferenceLocalization)
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageScientificReferencesSectionLocalization>(_updateDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);

        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageScientificReferenceLocalization>(It.IsAny<UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto>()))
            .Returns((object src) =>
            {
                var dto = (UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto)src;
                return new HippotherapyLandingPageScientificReferenceLocalization
                {
                    EntityId = dto.ReferenceId,
                    Name = dto.Name,
                    TranslationStatus = TranslationStatus.Relevant,
                };
            });
        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageScientificReferenceLocalizationItemDto>(It.IsAny<HippotherapyLandingPageScientificReferenceLocalization>()))
            .Returns((object src) =>
            {
                var entity = (HippotherapyLandingPageScientificReferenceLocalization)src;
                return new HippotherapyLandingPageScientificReferenceLocalizationItemDto
                {
                    ReferenceId = entity.EntityId,
                    Name = entity.Name,
                    TranslationStatus = entity.TranslationStatus,
                };
            });

        var existing = existingReferenceLocalization
            ? _updateDto.References
                .Select(c => new HippotherapyLandingPageScientificReferenceLocalization
                {
                    EntityId = c.ReferenceId,
                    LanguageId = _languageId,
                    Name = "Old translation",
                    TranslationStatus = TranslationStatus.Outdated,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
                })
                .ToList()
            : [];
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageScientificReferenceLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageScientificReferenceLocalization>>()))
            .ReturnsAsync(existing);
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
    }
}
