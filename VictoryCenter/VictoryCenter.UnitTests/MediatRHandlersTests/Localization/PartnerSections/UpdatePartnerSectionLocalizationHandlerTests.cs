using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Services.Partners;
using VictoryCenter.BLL.Validators.Localization.PartnerSections;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.PartnerSections;

public class UpdatePartnerSectionLocalizationHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILocalizationService<PartnerSection, PartnerSectionLocalization>> _mockSectionLocalizationService;
    private readonly Mock<ILocalizationService<Partner, PartnerLocalization>> _mockPartnerLocalizationService;
    private readonly IValidator<UpdatePartnerSectionLocalizationCommand> _validator;
    private readonly UpdatePartnerSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly PartnerSection _section = new()
    {
        Id = 1,
        Partners =
        [
            new Partner { Id = 10, PartnersSectionId = 1, Description = "Original description 1" },
            new Partner { Id = 11, PartnersSectionId = 1, Description = "Original description 2" }
        ]
    };

    private readonly PartnerSectionLocalization _sectionLocalization = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Upd Localized section title",
        Description = "Upd Localized section description"
    };

    private readonly PartnerSectionLocalizationDto _sectionLocalizationDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new() { Id = 2, Code = "en" },
        Title = "Upd Localized section title",
        Description = "Upd Localized section description"
    };

    public UpdatePartnerSectionLocalizationHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockSectionLocalizationService = new Mock<ILocalizationService<PartnerSection, PartnerSectionLocalization>>();
        _mockPartnerLocalizationService = new Mock<ILocalizationService<Partner, PartnerLocalization>>();
        _validator = new UpdatePartnerSectionLocalizationValidator(
            new BasePartnerSectionLocalizationValidator(new PartnerLocalizationItemValidator()));

        var partnersUpdater = new PartnerSectionLocalizationUpdater(
            _mockMapper.Object, _mockRepositoryWrapper.Object, _mockPartnerLocalizationService.Object);

        _handler = new UpdatePartnerSectionLocalizationHandler(
            _mockMapper.Object, _validator, _mockRepositoryWrapper.Object, _mockSectionLocalizationService.Object, partnersUpdater);

        _mockRepositoryWrapper.Setup(r => r.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
        _mockRepositoryWrapper.Setup(r => r.PartnerSectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnerSection>>()))
            .ReturnsAsync(_section);
    }

    [Fact]
    public async Task Handle_ShouldUpdateSectionAndExistingPartnerLocalizations_Successfully()
    {
        SetupDependencies(existingPartnerLocalization: true);

        var dto = GetValidDto();
        var command = new UpdatePartnerSectionLocalizationCommand(dto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_sectionLocalizationDto.Title, result.Value.Title);
        Assert.Equal(2, result.Value.Partners.Count);

        _mockPartnerLocalizationService.Verify(s => s.UpdateEntityLocalizationAsync(It.IsAny<PartnerLocalization>()), Times.Exactly(2));
        _mockPartnerLocalizationService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<PartnerLocalization>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateMissingPartnerLocalizations_WhenTheyDoNotExistYet()
    {
        SetupDependencies(existingPartnerLocalization: false);

        var dto = GetValidDto();
        var command = new UpdatePartnerSectionLocalizationCommand(dto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _mockPartnerLocalizationService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<PartnerLocalization>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSectionNotFound()
    {
        _mockRepositoryWrapper.Setup(r => r.PartnerSectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnerSection>>()))
            .ReturnsAsync((PartnerSection?)null);

        var command = new UpdatePartnerSectionLocalizationCommand(GetValidDto(), _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPartnerDoesNotBelongToSection()
    {
        SetupDependencies(existingPartnerLocalization: true);

        var dto = GetValidDto() with
        {
            Partners = [new UpdatePartnerLocalizationItemDto { PartnerId = 999, Description = "Valid description" }]
        };

        var command = new UpdatePartnerSectionLocalizationCommand(dto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        _mockPartnerLocalizationService.Verify(s => s.UpdateEntityLocalizationAsync(It.IsAny<PartnerLocalization>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        var dto = GetValidDto();
        _mockMapper.Setup(m => m.Map<PartnerSectionLocalization>(dto)).Returns(_sectionLocalization);
        _mockSectionLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_sectionLocalization))
            .ThrowsAsync(new KeyNotFoundException("Not found"));

        var command = new UpdatePartnerSectionLocalizationCommand(dto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        var dto = GetValidDto();
        _mockMapper.Setup(m => m.Map<PartnerSectionLocalization>(dto)).Returns(_sectionLocalization);
        _mockSectionLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_sectionLocalization))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdatePartnerSectionLocalizationCommand(dto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(PartnerSectionLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        var dto = GetValidDto();
        _mockMapper.Setup(m => m.Map<PartnerSectionLocalization>(dto)).Returns(_sectionLocalization);
        _mockSectionLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_sectionLocalization))
            .ThrowsAsync(new DbUpdateException());

        var command = new UpdatePartnerSectionLocalizationCommand(dto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PartnerSectionLocalization)), result.Errors[0].Message);
    }

    private void SetupDependencies(bool existingPartnerLocalization)
    {
        _mockMapper.Setup(m => m.Map<PartnerSectionLocalization>(It.IsAny<UpdatePartnerSectionLocalizationDto>())).Returns(_sectionLocalization);
        _mockSectionLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_sectionLocalization))
            .ReturnsAsync(_sectionLocalization);
        _mockMapper.Setup(m => m.Map<PartnerSectionLocalizationDto>(_sectionLocalization)).Returns(_sectionLocalizationDto);

        _mockMapper.Setup(m => m.Map<PartnerLocalization>(It.IsAny<UpdatePartnerLocalizationItemDto>()))
            .Returns((UpdatePartnerLocalizationItemDto item) => new PartnerLocalization { EntityId = item.PartnerId, Description = item.Description });
        _mockMapper.Setup(m => m.Map<PartnerLocalizationItemDto>(It.IsAny<PartnerLocalization>()))
            .Returns((PartnerLocalization entity) => new PartnerLocalizationItemDto { PartnerId = entity.EntityId, Description = entity.Description });

        var mockPartnerLocalizationRepo = new Mock<IRepositoryBase<PartnerLocalization>>();
        mockPartnerLocalizationRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnerLocalization>>()))
            .ReturnsAsync(existingPartnerLocalization ? new PartnerLocalization { EntityId = 10, LanguageId = 2 } : null);
        _mockRepositoryWrapper.Setup(r => r.GetRepository<PartnerLocalization>()).Returns(mockPartnerLocalizationRepo.Object);

        _mockPartnerLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<PartnerLocalization>()))
            .ReturnsAsync((PartnerLocalization entity) => entity);
        _mockPartnerLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(It.IsAny<PartnerLocalization>()))
            .ReturnsAsync((PartnerLocalization entity) => entity);
    }

    private static UpdatePartnerSectionLocalizationDto GetValidDto() => new()
    {
        Title = "Upd Localized section title",
        Description = "Upd Localized section description",
        Partners =
        [
            new UpdatePartnerLocalizationItemDto { PartnerId = 10, Description = "Upd Localized partner description 1" },
            new UpdatePartnerLocalizationItemDto { PartnerId = 11, Description = "Upd Localized partner description 2" }
        ]
    };
}
