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

    private static readonly List<UpdateHistorySectionContentLocalizationDto> _testContentLocalizationDto = new()
    {
        new UpdateHistorySectionContentLocalizationDto
        {
            EntityId = 3,
            Title = "Test Title Updated",
        },
        new UpdateHistorySectionContentLocalizationDto
        {
            EntityId = 2,
            Description = "Test Description Updated 2",
        }
    };

    private readonly UpdateHistorySectionLocalizationDto _testSectionLocalizationDto = new()
    {
        Contents = _testContentLocalizationDto
    };

    private readonly UpdateHistorySectionLocalizationDto _failedFortest = new()
    {
        Contents = new List<UpdateHistorySectionContentLocalizationDto>
        {
            new()
            {
                EntityId = 3,
                Title = "For failed Test Title",
            },
            new()
            {
                EntityId = 2,
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

    public UpdateHistoryLocalizatonHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _localizationServiceMock = new Mock<ILocalizationService<HistorySectionContent, HistorySectionContentLocalization>>();
        _validatorMock = new Mock<IValidator<UpdateHistoryLocalizationCommand>>();
        _handler = new UpdateHistoryLocalizationHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _localizationServiceMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalizationForHistory_Successfully()
    {
        SetupDependencies();

        var command = new UpdateHistoryLocalizationCommand(_testSectionLocalizationDto, 1, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailWhenSectionNotFoundInDatabase()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ThrowsAsync(new KeyNotFoundException(ErrorMessagesConstants.NotFound(1, typeof(HistorySection))));

        var command = new UpdateHistoryLocalizationCommand(_testSectionLocalizationDto, 1, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenKeyNotFoundExceptionThrown()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .ThrowsAsync(new KeyNotFoundException(ErrorMessagesConstants.NotFound(2, typeof(HistorySectionContentLocalization))));

        var command = new UpdateHistoryLocalizationCommand(_testSectionLocalizationDto, 1, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenValidationExceptionThrowsWhileTrackEntityLocalizationAsync()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .ThrowsAsync(new ValidationException(new[] { new ValidationFailure("test", "Validation failed during update.") }));

        var command = new UpdateHistoryLocalizationCommand(_failedFortest, 1, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenInvalidOperationExceptionThrown()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdateHistoryLocalizationCommand(_testSectionLocalizationDto, 1, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenFailedToUpdateEntityInDatabase()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var command = new UpdateHistoryLocalizationCommand(_testSectionLocalizationDto, 1, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenDbUpdateExceptionThrown()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var command = new UpdateHistoryLocalizationCommand(_testSectionLocalizationDto, 1, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUnexpectedExceptionThrown()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new Exception("Unexpected error"));

        var command = new UpdateHistoryLocalizationCommand(_testSectionLocalizationDto, 1, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    private void SetupDependencies()
    {
        _repositoryWrapperMock.Setup(r => r.HistorySectionsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(_section);

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalization>>(It.IsAny<List<UpdateHistorySectionContentLocalizationDto>>()))
            .Returns(new List<HistorySectionContentLocalization>());

        _localizationServiceMock
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HistorySectionContentLocalization>>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        _repositoryWrapperMock.Setup(r => r.HistorySectionContentLocalizationsRepository
            .GetAllAsync(It.IsAny<QueryOptions<HistorySectionContentLocalization>>()))
            .ReturnsAsync(new List<HistorySectionContentLocalization>());

        _mapperMock.Setup(m => m.Map<List<HistorySectionContentLocalizationDto>>(It.IsAny<List<HistorySectionContentLocalization>>()))
            .Returns(new List<HistorySectionContentLocalizationDto>());
    }
}
