using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageEthicsSection.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageEthicsSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageEthicsSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageEthicsSection;

public class CreateHippotherapyLandingPageEthicsSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageEthicsSectionEntity, HippotherapyLandingPageEthicsSectionLocalization>> _mockLocalizationService;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageEthicsPrinciple, HippotherapyLandingPageEthicsPrincipleLocalization>> _mockPrincipleLocalizationService;
    private readonly IValidator<CreateHippotherapyLandingPageEthicsSectionLocalizationCommand> _validator;
    private readonly CreateHippotherapyLandingPageEthicsSectionLocalizationHandler _handler;

    private readonly HippotherapyLandingPageEthicsSectionEntity _section = new()
    {
        Id = 1,
        EthicsPrinciples =
        [
            new HippotherapyLandingPageEthicsPrinciple { Id = 10, EthicsSectionId = 1, Text = "Original principle 1", Priority = 1 },
            new HippotherapyLandingPageEthicsPrinciple { Id = 11, EthicsSectionId = 1, Text = "Original principle 2", Priority = 2 },
        ],
    };

    private readonly CreateHippotherapyLandingPageEthicsSectionLocalizationDto _createDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Principles",
        Description = "Therapeutic horseback riding for children and veterans.",
        Principles =
        [
            new UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto { PrincipleId = 10, Text = "Translated principle text 1" },
            new UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto { PrincipleId = 11, Text = "Translated principle text 2" },
        ],
    };

    private readonly HippotherapyLandingPageEthicsSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Principles",
        Description = "Therapeutic horseback riding for children and veterans.",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageEthicsSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Principles",
        Description = "Therapeutic horseback riding for children and veterans.",
    };

    public CreateHippotherapyLandingPageEthicsSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageEthicsSectionEntity, HippotherapyLandingPageEthicsSectionLocalization>>();
        _mockPrincipleLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageEthicsPrinciple, HippotherapyLandingPageEthicsPrincipleLocalization>>();
        _validator = new CreateHippotherapyLandingPageEthicsSectionLocalizationValidator(
            new BaseHippotherapyLandingPageEthicsSectionLocalizationValidator(
                new HippotherapyLandingPageEthicsPrincipleLocalizationItemValidator()));

        var principlesUpdater = new HippotherapyLandingPageEthicsPrincipleLocalizationUpdater(
            _mockMapper.Object, _mockRepositoryWrapper.Object, _mockPrincipleLocalizationService.Object);

        _handler = new CreateHippotherapyLandingPageEthicsSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator, _mockRepositoryWrapper.Object, principlesUpdater);

        _mockRepositoryWrapper.Setup(r => r.BeginTransaction())
            .Returns(() => new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageEthicsSectionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageEthicsSectionEntity>>()))
            .ReturnsAsync(_section);
    }

    [Fact]
    public async Task Handle_ShouldCreateSectionAndPrincipleLocalizations_Successfully()
    {
        SetupDependencies(existingPrincipleLocalization: false);

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageEthicsSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_responseDto.Title, result.Value.Title);
        Assert.Equal(_responseDto.EntityId, result.Value.EntityId);
        Assert.Equal(new[] { 10L, 11L }, result.Value.Principles.Select(c => c.PrincipleId));
        _mockPrincipleLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(
                It.Is<IEnumerable<HippotherapyLandingPageEthicsPrincipleLocalization>>(l => l.Count() == 2 && l.All(x => x.LanguageId == 2)),
                false),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateExistingPrincipleLocalizations_WhenTheyAlreadyExist()
    {
        SetupDependencies(existingPrincipleLocalization: true);

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageEthicsSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _mockPrincipleLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageEthicsPrincipleLocalization>>(), true),
            Times.Once);
        _mockPrincipleLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageEthicsPrincipleLocalization>>(), false),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateOnlySectionLocalization_WhenPrinciplesAreEmpty()
    {
        SetupDependencies(existingPrincipleLocalization: false);
        var dto = new CreateHippotherapyLandingPageEthicsSectionLocalizationDto
        {
            EntityId = _createDto.EntityId,
            LanguageId = _createDto.LanguageId,
            Title = _createDto.Title,
            Description = _createDto.Description,
            Principles = [],
        };
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageEthicsSectionLocalization>(dto)).Returns(_entity);

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageEthicsSectionLocalizationCommand(dto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Principles);
        _mockPrincipleLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageEthicsPrincipleLocalization>>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSectionNotFound()
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageEthicsSectionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageEthicsSectionEntity>>()))
            .ReturnsAsync((HippotherapyLandingPageEthicsSectionEntity?)null);

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageEthicsSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        _mockLocalizationService.Verify(
            s => s.CreateEntityLocalizationAsync(It.IsAny<HippotherapyLandingPageEthicsSectionLocalization>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPrincipleDoesNotBelongToSection()
    {
        SetupDependencies(existingPrincipleLocalization: false);
        var dto = new CreateHippotherapyLandingPageEthicsSectionLocalizationDto
        {
            EntityId = _createDto.EntityId,
            LanguageId = _createDto.LanguageId,
            Title = _createDto.Title,
            Description = _createDto.Description,
            Principles = [new UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto { PrincipleId = 999, Text = "Translated principle text" }],
        };
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageEthicsSectionLocalization>(dto)).Returns(_entity);

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageEthicsSectionLocalizationCommand(dto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        _mockPrincipleLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageEthicsPrincipleLocalization>>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new CreateHippotherapyLandingPageEthicsSectionLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            Title = "",
            Description = "",
            Principles =
            [
                new UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto { PrincipleId = 10, Text = "Translated principle text" },
                new UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto { PrincipleId = 10, Text = "Translated principle text" },
            ],
        };

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageEthicsSectionLocalizationCommand(invalidDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("EntityId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("LanguageId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("Title is required", result.Errors.Select(e => e.Message));
        Assert.Contains("Description is required", result.Errors.Select(e => e.Message));
        Assert.Contains(
            ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdateHippotherapyLandingPageEthicsSectionLocalizationDto.Principles)),
            result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageEthicsSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Entity not found"));

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageEthicsSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageEthicsSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageEthicsSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyLandingPageEthicsSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageEthicsSectionLocalization>(_createDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var result = await _handler.Handle(
            new CreateHippotherapyLandingPageEthicsSectionLocalizationCommand(_createDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyLandingPageEthicsSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(bool existingPrincipleLocalization)
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageEthicsSectionLocalization>(_createDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageEthicsSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);

        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageEthicsPrincipleLocalization>(It.IsAny<UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto>()))
            .Returns((object src) =>
            {
                var dto = (UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto)src;
                return new HippotherapyLandingPageEthicsPrincipleLocalization
                {
                    EntityId = dto.PrincipleId,
                    Text = dto.Text,
                    TranslationStatus = TranslationStatus.Relevant,
                };
            });
        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageEthicsPrincipleLocalizationItemDto>(It.IsAny<HippotherapyLandingPageEthicsPrincipleLocalization>()))
            .Returns((object src) =>
            {
                var entity = (HippotherapyLandingPageEthicsPrincipleLocalization)src;
                return new HippotherapyLandingPageEthicsPrincipleLocalizationItemDto
                {
                    PrincipleId = entity.EntityId,
                    Text = entity.Text,
                    TranslationStatus = entity.TranslationStatus,
                };
            });

        var existing = existingPrincipleLocalization
            ? _createDto.Principles
                .Select(c => new HippotherapyLandingPageEthicsPrincipleLocalization
                {
                    EntityId = c.PrincipleId,
                    LanguageId = _createDto.LanguageId,
                    Text = "Old translation",
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
                })
                .ToList()
            : [];
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageEthicsPrincipleLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageEthicsPrincipleLocalization>>()))
            .ReturnsAsync(existing);
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
    }
}
