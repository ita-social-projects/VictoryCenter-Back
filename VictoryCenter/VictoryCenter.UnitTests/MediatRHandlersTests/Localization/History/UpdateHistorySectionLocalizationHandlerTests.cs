using AutoMapper;

using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Update;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.HistorySections;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.History;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.History;

public class UpdateHistorySectionLocalizationHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IHistorySectionsRepository> _historySectionsRepositoryMock = new();
    private readonly Mock<IHistorySectionContentLocalizationsRepository> _localizationsRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILocalizationService<HistorySectionContent, HistorySectionContentLocalization>> _localizationServiceMock = new();
    private readonly Mock<IValidator<UpdateHistorySectionLocalizationCommand>> _validatorMock = new();

    private readonly UpdateHistorySectionLocalizationHandler _sut;

    public UpdateHistorySectionLocalizationHandlerTests()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository).Returns(_historySectionsRepositoryMock.Object);
        _repositoryWrapperMock.Setup(r => r.HistorySectionContentLocalizationsRepository).Returns(_localizationsRepositoryMock.Object);

        _sut = new UpdateHistorySectionLocalizationHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _localizationServiceMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ValidatorThrows_ReturnsFailResult()
    {
        var command = new UpdateHistorySectionLocalizationCommand(new UpdateHistorySectionLocalizationDto(), 1);

        var realValidator = new UpdateHistorySectionLocalizationCommandValidator(
            new VictoryCenter.BLL.Validators.Localization.History.BaseHistorySectionContentLocalizationValidator());

        var sutWithRealValidator = new UpdateHistorySectionLocalizationHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _localizationServiceMock.Object,
            realValidator);

        var result = await sutWithRealValidator.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors.Select(e => e.Message), m => m.Contains(nameof(UpdateHistorySectionLocalizationCommand.UpdateDto.Contents)));
    }

    [Fact]
    public async Task Handle_SectionNotFound_ReturnsFailResult()
    {
        var dto = new UpdateHistorySectionLocalizationDto { EntityId = 1, Contents = [] };
        var command = new UpdateHistorySectionLocalizationCommand(dto, 1);

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        _historySectionsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync((HistorySection?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == ErrorMessagesConstants.NotFound(1, typeof(HistorySection)));
    }

    [Fact]
    public async Task Handle_ExistingLocalizationsNotFound_CreatesLocalizationsAndReturnsOk()
    {
        var dto = new UpdateHistorySectionLocalizationDto
        {
            EntityId = 1,
            Contents = [new UpdateHistorySectionContentLocalizationDto { EntityId = 10, Title = "Test" }]
        };
        var command = new UpdateHistorySectionLocalizationCommand(dto, 1);

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var section = new HistorySection
        {
            Id = 1,
            Contents = [new TitleHistoryContent { Id = 10, ContentType = ContentType.Title }]
        };

        _historySectionsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(section);

        var contentLocalization = new HistorySectionContentLocalization { EntityId = 10 };
        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns([contentLocalization]);

        _localizationsRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HistorySectionContentLocalization>>()))
            .ReturnsAsync([]);

        _localizationServiceMock.Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<List<HistorySectionContentLocalization>>(), false))
            .Returns(Task.CompletedTask);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _localizationServiceMock.Verify(s => s.TrackEntityLocalizationAsync(It.IsAny<List<HistorySectionContentLocalization>>(), false), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesLocalizationsAndReturnsOk()
    {
        var dto = new UpdateHistorySectionLocalizationDto
        {
            EntityId = 1,
            Contents = [new UpdateHistorySectionContentLocalizationDto { EntityId = 10, Title = "Test" }]
        };
        var command = new UpdateHistorySectionLocalizationCommand(dto, 1);

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var section = new HistorySection
        {
            Id = 1,
            Contents = [new TitleHistoryContent { Id = 10, ContentType = ContentType.Title }]
        };

        _historySectionsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(section);

        var contentLocalization = new HistorySectionContentLocalization { EntityId = 10 };
        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns([contentLocalization]);

        var existingLocalization = new HistorySectionContentLocalization { EntityId = 10 };
        _localizationsRepositoryMock.SetupSequence(r => r.GetAllAsync(It.IsAny<QueryOptions<HistorySectionContentLocalization>>()))
            .ReturnsAsync([existingLocalization])
            .ReturnsAsync([existingLocalization]);

        _localizationServiceMock.Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<List<HistorySectionContentLocalization>>(), true))
            .Returns(Task.CompletedTask);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var returnedDto = new HistorySectionLocalizationDto { EntityId = 1, Contents = [] };
        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalizationDto>>(It.IsAny<List<HistorySectionContentLocalization>>()))
            .Returns([]);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.EntityId);

        Assert.Equal(TranslationStatus.Relevant, contentLocalization.TranslationStatus);
        Assert.Equal(1, contentLocalization.LanguageId);

        _localizationServiceMock.Verify(s => s.TrackEntityLocalizationAsync(It.IsAny<List<HistorySectionContentLocalization>>(), true), Times.Once);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ReturnsFailResult()
    {
        var dto = new UpdateHistorySectionLocalizationDto
        {
            EntityId = 1,
            Contents = [new UpdateHistorySectionContentLocalizationDto { EntityId = 10, Title = "Test" }]
        };
        var command = new UpdateHistorySectionLocalizationCommand(dto, 1);

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var section = new HistorySection
        {
            Id = 1,
            Contents = [new TitleHistoryContent { Id = 10, ContentType = ContentType.Title }]
        };

        _historySectionsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(section);

        var contentLocalization = new HistorySectionContentLocalization { EntityId = 10 };
        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns([contentLocalization]);

        var existingLocalization = new HistorySectionContentLocalization { EntityId = 10 };
        _localizationsRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HistorySectionContentLocalization>>()))
            .ReturnsAsync([existingLocalization]);

        _localizationServiceMock.Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<List<HistorySectionContentLocalization>>(), true))
            .Returns(Task.CompletedTask);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HistorySectionContentLocalization)));
    }

    [Fact]
    public async Task Handle_ThrowsKeyNotFoundException_ReturnsFailResult()
    {
        var command = new UpdateHistorySectionLocalizationCommand(new UpdateHistorySectionLocalizationDto(), 1);
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _historySectionsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>())).ThrowsAsync(new KeyNotFoundException("Key not found"));

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "Key not found");
    }

    [Fact]
    public async Task Handle_ThrowsInvalidOperationException_ReturnsFailResult()
    {
        var command = new UpdateHistorySectionLocalizationCommand(new UpdateHistorySectionLocalizationDto(), 1);
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _historySectionsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>())).ThrowsAsync(new InvalidOperationException());

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == ErrorMessagesConstants.FailedToUpdateEntity(typeof(HistorySectionContentLocalization)));
    }

    [Fact]
    public async Task Handle_ThrowsDbUpdateException_ReturnsFailResult()
    {
        var command = new UpdateHistorySectionLocalizationCommand(new UpdateHistorySectionLocalizationDto(), 1);
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _historySectionsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>())).ThrowsAsync(new DbUpdateException());

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HistorySectionContentLocalization)));
    }

    [Fact]
    public async Task Handle_ThrowsGenericException_ReturnsFailResult()
    {
        var command = new UpdateHistorySectionLocalizationCommand(new UpdateHistorySectionLocalizationDto(), 1);
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _historySectionsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>())).ThrowsAsync(new Exception("Generic exception"));

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == ErrorMessagesConstants.FailedToUpdateEntity(typeof(HistorySectionContentLocalization)));
    }

    [Fact]
    public async Task Handle_MixedRequest_CreatesAndUpdatesLocalizationsAndReturnsOk()
    {
        var dto = new UpdateHistorySectionLocalizationDto
        {
            EntityId = 1,
            Contents = [
                new UpdateHistorySectionContentLocalizationDto { EntityId = 10, Title = "To Update" },
                new UpdateHistorySectionContentLocalizationDto { EntityId = 20, Description = "To Create" }
            ]
        };
        var command = new UpdateHistorySectionLocalizationCommand(dto, 1);

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var section = new HistorySection
        {
            Id = 1,
            Contents = [
                new TitleHistoryContent { Id = 10, ContentType = ContentType.Title },
                new DescriptionHistoryContent { Id = 20, ContentType = ContentType.Description }
            ]
        };

        _historySectionsRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(section);

        var contentLocalization1 = new HistorySectionContentLocalization { EntityId = 10 };
        var contentLocalization2 = new HistorySectionContentLocalization { EntityId = 20 };
        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns([contentLocalization1, contentLocalization2]);

        var existingLocalization = new HistorySectionContentLocalization { EntityId = 10 };
        _localizationsRepositoryMock.SetupSequence(r => r.GetAllAsync(It.IsAny<QueryOptions<HistorySectionContentLocalization>>()))
            .ReturnsAsync([existingLocalization])
            .ReturnsAsync([existingLocalization, contentLocalization2]);

        _localizationServiceMock.Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<List<HistorySectionContentLocalization>>(), true))
            .Returns(Task.CompletedTask);
        _localizationServiceMock.Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<List<HistorySectionContentLocalization>>(), false))
            .Returns(Task.CompletedTask);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalizationDto>>(It.IsAny<List<HistorySectionContentLocalization>>()))
            .Returns([]);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess, result.IsFailed ? result.Errors.First().Message : "");
        _localizationServiceMock.Verify(s => s.TrackEntityLocalizationAsync(It.IsAny<List<HistorySectionContentLocalization>>(), true), Times.Once);
        _localizationServiceMock.Verify(s => s.TrackEntityLocalizationAsync(It.IsAny<List<HistorySectionContentLocalization>>(), false), Times.Once);
    }
}
