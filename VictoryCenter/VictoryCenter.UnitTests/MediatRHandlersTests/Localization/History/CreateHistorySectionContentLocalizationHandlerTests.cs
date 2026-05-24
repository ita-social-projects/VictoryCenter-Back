using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.History;

public class CreateHistorySectionContentLocalizationHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILocalizationService<HistorySectionContent, HistorySectionContentLocalization>> _localizationServiceMock;
    private readonly Mock<IValidator<CreateHistoryLocalizationCommand>> _validatorMock;
    private readonly CreateHistoryLocalizationHandler _handler;

    private static readonly List<CreateHistorySectionContentLocalizationDto> _testContentLocalizationDto = new()
    {
        new CreateHistorySectionContentLocalizationDto
        {
            EntityId = 3,
            LanguageId = 2,
            Title = "Test Title",
        },
        new CreateHistorySectionContentLocalizationDto
        {
            EntityId = 2,
            LanguageId = 2,
            Description = "Test Description 2",
        }
    };
    private readonly CreateHistorySectionLocalizationDto _testSectionLocalizationDto = new()
    {
        EntityId = 1,
        Contents = _testContentLocalizationDto
    };
    private readonly CreateHistorySectionLocalizationDto _failedFortest = new()
    {
        EntityId = 3,
        Contents = new List<CreateHistorySectionContentLocalizationDto>
        {
            new ()
            {
                EntityId = 3,
                LanguageId = 2,
                Title = "For failed Test Title",
            },
            new ()
            {
                EntityId = 2,
                LanguageId = 3,
                Description = "For failed Test Description 2",
            }
        }
    };

    private readonly HistorySection _section = new()
    {
        Id = 1,
        Template = HistorySectionTemplate.TextOnly,
        Order = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        Contents = new List<HistorySectionContent>()
        {
            new TitleHistoryContent
            {
                Id = 3,
                SectionId = 1,
                ContentType = ContentType.Title,
                Order = 1,
                Title = "Оригінальний текст",
            },
            new DescriptionHistoryContent
            {
                Id = 2,
                SectionId = 1,
                ContentType = ContentType.Description,
                Order = 1,
                Description = "Оригінальний опис",
            }
        }
    };
    public CreateHistorySectionContentLocalizationHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _localizationServiceMock = new Mock<ILocalizationService<HistorySectionContent, HistorySectionContentLocalization>>();
        _validatorMock = new Mock<IValidator<CreateHistoryLocalizationCommand>>();
        _handler = new CreateHistoryLocalizationHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _localizationServiceMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateLocalizationForHistory_Successfully()
    {
        SetupDependencies();

        var command = new CreateHistoryLocalizationCommand(new List<CreateHistorySectionLocalizationDto> { _testSectionLocalizationDto });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailWhenSectionNotFoundInDatabase()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ThrowsAsync(new KeyNotFoundException(ErrorMessagesConstants.NotFound(_testSectionLocalizationDto.EntityId, typeof(HistorySection))));

        var command = new CreateHistoryLocalizationCommand(new List<CreateHistorySectionLocalizationDto> { _testSectionLocalizationDto });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenKeyNotFoundExceptionThrown()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<CreateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .ThrowsAsync(new KeyNotFoundException(ErrorMessagesConstants.NotFound(2, typeof(HistorySectionContentLocalization))));

        var command = new CreateHistoryLocalizationCommand(new List<CreateHistorySectionLocalizationDto> { _testSectionLocalizationDto });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenValidationExceptionThrowsWhileTrackEntityLocalizationAsync()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<CreateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .ThrowsAsync(new ValidationException(new[] { new ValidationFailure("test", "Bulk localization supports only one LanguageId.") }));

        var command = new CreateHistoryLocalizationCommand(new List<CreateHistorySectionLocalizationDto> { _failedFortest });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenInvalidOperationExceptionThrown()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<CreateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .ThrowsAsync(new InvalidOperationException());

        var command = new CreateHistoryLocalizationCommand(new List<CreateHistorySectionLocalizationDto> { _testSectionLocalizationDto });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenFailedToCreateEntityInDatabase()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<CreateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var command = new CreateHistoryLocalizationCommand(new List<CreateHistorySectionLocalizationDto> { _testSectionLocalizationDto });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenDbUpdateExceptionThrown()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<CreateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var command = new CreateHistoryLocalizationCommand(new List<CreateHistorySectionLocalizationDto> { _testSectionLocalizationDto });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUnexpectedExceptionThrown()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);
        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<CreateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());
        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new Exception("Unexpected error"));
        var command = new CreateHistoryLocalizationCommand(new List<CreateHistorySectionLocalizationDto> { _testSectionLocalizationDto });
        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsFailed);
    }

    private void SetupDependencies()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<CreateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        _repositoryWrapperMock.Setup(r => r.HistorySectionContentLocalizationsRepository
        .GetAllAsync(It.IsAny<QueryOptions<HistorySectionContentLocalization>>()))
        .ReturnsAsync(new List<HistorySectionContentLocalization>());

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<CreateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());
    }
}
