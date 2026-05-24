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
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.History;

public class UpdateHistoryLocalizatonHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILocalizationService<HistorySectionContent, HistorySectionContentLocalization>> _localizationServiceMock;
    private readonly Mock<IValidator<UpdateHistoryLocalizationCommand>> _validatorMock;
    private readonly UpdateHistoryLocalizationHandler _handler;

    private static readonly List<UpdateHistorySectionContentLocalizationDto> _testContentDto = new()
    {
        new UpdateHistorySectionContentLocalizationDto { EntityId = 3, Title = "Updated Title" },
        new UpdateHistorySectionContentLocalizationDto { EntityId = 2, Description = "Updated Description" }
    };

    private readonly UpdateHistorySectionLocalizationDto _testSectionDto = new()
    {
        EntityId = 1,
        Contents = _testContentDto
    };

    private readonly HistorySection _section = new()
    {
        Id = 1,
        Template = HistorySectionTemplate.TextOnly,
        Order = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        Contents = new List<HistorySectionContent>
        {
            new TitleHistoryContent
            {
                Id = 3,
                SectionId = 1,
                ContentType = ContentType.Title,
                Order = 1,
                Title = "Original Title"
            },
            new DescriptionHistoryContent
            {
                Id = 2,
                SectionId = 1,
                ContentType = ContentType.Description,
                Order = 2,
                Description = "Original Description"
            }
        }
    };

    private readonly List<HistorySectionContentLocalization> _existingLocalizations = new()
    {
        new HistorySectionContentLocalization { EntityId = 3, LanguageId = 2 },
        new HistorySectionContentLocalization { EntityId = 2, LanguageId = 2 }
    };

    public UpdateHistoryLocalizatonHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _localizationServiceMock = new Mock<ILocalizationService<HistorySectionContent, HistorySectionContentLocalization>>();
        _validatorMock = new Mock<IValidator<UpdateHistoryLocalizationCommand>>();
        _handler = new UpdateHistoryLocalizationHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _localizationServiceMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalizationForHistory_Successfully()
    {
        SetupDependencies();

        var command = new UpdateHistoryLocalizationCommand(new List<UpdateHistorySectionLocalizationDto> { _testSectionDto }, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMissingSections()
    {
        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(new List<HistorySection> { _section, new() { Id = 999 } });

        var command = new UpdateHistoryLocalizationCommand(new List<UpdateHistorySectionLocalizationDto> { _testSectionDto }, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSectionNotFoundInDatabase()
    {
        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(new List<HistorySection> { _section });

        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync((HistorySection?)null);

        var command = new UpdateHistoryLocalizationCommand(new List<UpdateHistorySectionLocalizationDto> { _testSectionDto }, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMissingContents()
    {
        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(new List<HistorySection> { _section });

        var sectionWithExtraContent = new HistorySection
        {
            Id = 1,
            Template = HistorySectionTemplate.TextOnly,
            Order = 1,
            CreatedAt = DateTimeOffset.UtcNow,
            Contents = new List<HistorySectionContent>
            {
                new TitleHistoryContent { Id = 3, SectionId = 1, ContentType = ContentType.Title, Order = 1, Title = "T" },
                new DescriptionHistoryContent { Id = 2, SectionId = 1, ContentType = ContentType.Description, Order = 2, Description = "D" },
                new DescriptionHistoryContent { Id = 99, SectionId = 1, ContentType = ContentType.Description, Order = 3, Description = "Extra" }
            }
        };

        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(sectionWithExtraContent);

        var command = new UpdateHistoryLocalizationCommand(new List<UpdateHistorySectionLocalizationDto> { _testSectionDto }, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenLocalizationDoesNotExistInDatabase()
    {
        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(new List<HistorySection> { _section });

        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock
            .Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>
            {
                new() { EntityId = 3, LanguageId = 2 },
                new() { EntityId = 2, LanguageId = 2 }
            });

        _repositoryWrapperMock
            .Setup(r => r.HistorySectionContentLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<HistorySectionContentLocalization>>()))
            .ReturnsAsync(new List<HistorySectionContentLocalization>());

        var command = new UpdateHistoryLocalizationCommand(new List<UpdateHistorySectionLocalizationDto> { _testSectionDto }, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenKeyNotFoundExceptionThrown()
    {
        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ThrowsAsync(new KeyNotFoundException(ErrorMessagesConstants.NotFound(1, typeof(HistorySection))));

        var command = new UpdateHistoryLocalizationCommand(new List<UpdateHistorySectionLocalizationDto> { _testSectionDto }, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenValidationExceptionThrown()
    {
        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(new List<HistorySection> { _section });

        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock
            .Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>
            {
                new() { EntityId = 3, LanguageId = 2 },
                new() { EntityId = 2, LanguageId = 2 }
            });

        _repositoryWrapperMock
            .Setup(r => r.HistorySectionContentLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<HistorySectionContentLocalization>>()))
            .ReturnsAsync(_existingLocalizations);

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), true))
            .ThrowsAsync(new ValidationException(new[] { new ValidationFailure("test", "Validation failed.") }));

        var command = new UpdateHistoryLocalizationCommand(new List<UpdateHistorySectionLocalizationDto> { _testSectionDto }, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenInvalidOperationExceptionThrown()
    {
        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(new List<HistorySection> { _section });

        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock
            .Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>
            {
                new() { EntityId = 3, LanguageId = 2 },
                new() { EntityId = 2, LanguageId = 2 }
            });

        _repositoryWrapperMock
            .Setup(r => r.HistorySectionContentLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<HistorySectionContentLocalization>>()))
            .ReturnsAsync(_existingLocalizations);

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), true))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdateHistoryLocalizationCommand(new List<UpdateHistorySectionLocalizationDto> { _testSectionDto }, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenFailedToSaveChanges()
    {
        SetupDependencies();

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var command = new UpdateHistoryLocalizationCommand(new List<UpdateHistorySectionLocalizationDto> { _testSectionDto }, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenDbUpdateExceptionThrown()
    {
        SetupDependencies();

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var command = new UpdateHistoryLocalizationCommand(new List<UpdateHistorySectionLocalizationDto> { _testSectionDto }, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUnexpectedExceptionThrown()
    {
        SetupDependencies();

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new Exception("Unexpected error"));

        var command = new UpdateHistoryLocalizationCommand(new List<UpdateHistorySectionLocalizationDto> { _testSectionDto }, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    private void SetupDependencies()
    {
        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(new List<HistorySection> { _section });

        _repositoryWrapperMock
            .Setup(r => r.HistorySectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock
            .Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>
            {
                new() { EntityId = 3, LanguageId = 2 },
                new() { EntityId = 2, LanguageId = 2 }
            });

        _repositoryWrapperMock
            .Setup(r => r.HistorySectionContentLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<HistorySectionContentLocalization>>()))
            .ReturnsAsync(_existingLocalizations);

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), true))
            .Returns(Task.CompletedTask);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        _mapperMock
            .Setup(m => m.Map<List<HistorySectionContentLocalizationDto>>(It.IsAny<List<HistorySectionContentLocalization>>()))
            .Returns(new List<HistorySectionContentLocalizationDto>());
    }
}
