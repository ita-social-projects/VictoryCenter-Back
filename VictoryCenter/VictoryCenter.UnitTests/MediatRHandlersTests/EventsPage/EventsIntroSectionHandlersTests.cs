using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Commands.Admin.EventsPage.Update;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;
using VictoryCenter.BLL.Queries.Admin.EventsPage.Get;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.EventsIntroSections;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.EventsPage;

public class EventsIntroSectionHandlersTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IEventsIntroSectionsRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    public EventsIntroSectionHandlersTests()
    {
        _repositoryWrapperMock
            .SetupGet(wrapper => wrapper.EventsIntroSectionsRepository)
            .Returns(_repositoryMock.Object);
        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync())
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task Get_Handle_WhenSectionExists_ReturnsMappedDto()
    {
        // Arrange
        var entity = CreateEntity();
        var expectedDto = CreateDto(entity);
        _repositoryMock.Setup(repository => repository.GetFirstOrDefaultAsync(null)).ReturnsAsync(entity);
        _mapperMock.Setup(mapper => mapper.Map<EventsIntroSectionDto>(entity)).Returns(expectedDto);
        var handler = new GetEventsIntroSectionHandler(_repositoryWrapperMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(new GetEventsIntroSectionQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Same(expectedDto, result.Value);
    }

    [Fact]
    public async Task Get_Handle_WhenSectionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _repositoryMock.Setup(repository => repository.GetFirstOrDefaultAsync(null)).ReturnsAsync((EventsIntroSection?)null);
        var handler = new GetEventsIntroSectionHandler(_repositoryWrapperMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(new GetEventsIntroSectionQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(), result.Errors.Single().Message);
        _mapperMock.Verify(mapper => mapper.Map<EventsIntroSectionDto>(It.IsAny<EventsIntroSection>()), Times.Never);
    }

    [Fact]
    public async Task UpdateDescription_Handle_ChangesOnlyDescription()
    {
        // Arrange
        var entity = CreateEntity();
        var originalTitle = entity.EventsBlockTitle;
        var dto = new UpdateEventsPageDescriptionDto { PageDescription = "<p>New description</p>" };
        SetupTrackedEntity(entity);
        _mapperMock.Setup(mapper => mapper.Map<EventsIntroSectionDto>(entity)).Returns(CreateDto(entity));
        var handler = new UpdateEventsPageDescriptionHandler(_repositoryWrapperMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(new UpdateEventsPageDescriptionCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(dto.PageDescription, entity.PageDescription);
        Assert.Equal(originalTitle, entity.EventsBlockTitle);
        VerifyTrackedUpdate(entity);
    }

    [Fact]
    public async Task UpdateTitle_Handle_ChangesOnlyTitle()
    {
        // Arrange
        var entity = CreateEntity();
        var originalDescription = entity.PageDescription;
        var dto = new UpdateEventsBlockTitleDto { EventsBlockTitle = "<p>New title</p>" };
        SetupTrackedEntity(entity);
        _mapperMock.Setup(mapper => mapper.Map<EventsIntroSectionDto>(entity)).Returns(CreateDto(entity));
        var handler = new UpdateEventsBlockTitleHandler(_repositoryWrapperMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(new UpdateEventsBlockTitleCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(dto.EventsBlockTitle, entity.EventsBlockTitle);
        Assert.Equal(originalDescription, entity.PageDescription);
        VerifyTrackedUpdate(entity);
    }

    private void SetupTrackedEntity(EventsIntroSection entity)
    {
        _repositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<EventsIntroSection>>()))
            .ReturnsAsync(entity);
    }

    private void VerifyTrackedUpdate(EventsIntroSection entity)
    {
        _repositoryMock.Verify(repository => repository.Update(entity), Times.Once);
        _repositoryWrapperMock.Verify(wrapper => wrapper.SaveChangesAsync(), Times.Once);
        _repositoryMock.Verify(
            repository => repository.GetFirstOrDefaultAsync(It.Is<QueryOptions<EventsIntroSection>>(options => !options.AsNoTracking)),
            Times.Once);
    }

    private static EventsIntroSection CreateEntity() => new()
    {
        Id = 1,
        EventsBlockTitle = "<p>Original title</p>",
        PageDescription = "<p>Original description</p>",
        CreatedAt = DateTimeOffset.UtcNow,
    };

    private static EventsIntroSectionDto CreateDto(EventsIntroSection entity) => new()
    {
        EventsBlockTitle = entity.EventsBlockTitle,
        PageDescription = entity.PageDescription,
    };
}
