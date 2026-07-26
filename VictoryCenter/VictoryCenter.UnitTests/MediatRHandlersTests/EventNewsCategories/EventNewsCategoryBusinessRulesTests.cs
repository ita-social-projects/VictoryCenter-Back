using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Create;
using VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Delete;
using VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;
using VictoryCenter.BLL.Queries.Admin.EventNewsCategories.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.EventNews;
using VictoryCenter.DAL.Repositories.Interfaces.EventNewsCategories;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.EventNewsCategories;

public class EventNewsCategoryBusinessRulesTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _wrapper = new();
    private readonly Mock<IEventNewsCategoryRepository> _categoryRepository = new();
    private readonly Mock<IEventNewsRepository> _eventNewsRepository = new();

    public EventNewsCategoryBusinessRulesTests()
    {
        _wrapper.SetupGet(item => item.EventNewsCategoryRepository).Returns(_categoryRepository.Object);
        _wrapper.SetupGet(item => item.EventNewsRepository).Returns(_eventNewsRepository.Object);
        _mapper.Setup(mapper => mapper.Map<AdminEventNewsCategoryDto>(It.IsAny<EventNewsCategory>()))
            .Returns((EventNewsCategory category) => new AdminEventNewsCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                CreatedAt = category.CreatedAt
            });
    }

    [Fact]
    public async Task CreateCategory_ShouldTrimNameAndReturnCreatedCategory()
    {
        EventNewsCategory? createdCategory = null;
        _categoryRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategory, bool>>>()))
            .ReturnsAsync(false);
        _categoryRepository
            .Setup(repository => repository.CreateAsync(It.IsAny<EventNewsCategory>()))
            .Callback<EventNewsCategory>(category => createdCategory = category)
            .ReturnsAsync((EventNewsCategory category) => category);
        _wrapper.Setup(repository => repository.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new CreateEventNewsCategoryHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new CreateEventNewsCategoryCommand(new CreateEventNewsCategoryDto { Name = "  News  " }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(createdCategory);
        Assert.Equal("News", createdCategory.Name);
        Assert.NotEqual(default, createdCategory.CreatedAt);
    }

    [Fact]
    public async Task CreateCategory_ShouldFail_WhenNameAlreadyExists()
    {
        _categoryRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategory, bool>>>()))
            .ReturnsAsync(true);
        var handler = new CreateEventNewsCategoryHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new CreateEventNewsCategoryCommand(new CreateEventNewsCategoryDto { Name = "News" }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(EventNewsCategoryConstants.DuplicateCategoryName, result.Errors[0].Message);
        _categoryRepository.Verify(repository => repository.CreateAsync(It.IsAny<EventNewsCategory>()), Times.Never);
    }

    [Fact]
    public async Task CreateCategory_ShouldPropagateUnexpectedDatabaseException()
    {
        _categoryRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategory, bool>>>()))
            .ReturnsAsync(false);
        _categoryRepository
            .Setup(repository => repository.CreateAsync(It.IsAny<EventNewsCategory>()))
            .ReturnsAsync((EventNewsCategory category) => category);
        _wrapper.Setup(repository => repository.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());
        var handler = new CreateEventNewsCategoryHandler(_mapper.Object, _wrapper.Object);

        await Assert.ThrowsAsync<DbUpdateException>(() => handler.Handle(
            new CreateEventNewsCategoryCommand(new CreateEventNewsCategoryDto { Name = "News" }),
            CancellationToken.None));
    }

    [Fact]
    public async Task UpdateCategory_ShouldFail_WhenCategoryDoesNotExist()
    {
        _categoryRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<EventNewsCategory>>()))
            .ReturnsAsync((EventNewsCategory?)null);
        var handler = new UpdateEventNewsCategoryHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateEventNewsCategoryCommand(10, new UpdateEventNewsCategoryDto { Name = "Updated" }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains("not found", result.Errors[0].Message, StringComparison.OrdinalIgnoreCase);
        _wrapper.Verify(repository => repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateCategory_ShouldFail_WhenNameAlreadyExists()
    {
        var category = new EventNewsCategory { Id = 10, Name = "News" };
        _categoryRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<EventNewsCategory>>()))
            .ReturnsAsync(category);
        _categoryRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategory, bool>>>()))
            .ReturnsAsync(true);
        var handler = new UpdateEventNewsCategoryHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateEventNewsCategoryCommand(10, new UpdateEventNewsCategoryDto { Name = "Existing" }),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(EventNewsCategoryConstants.DuplicateCategoryName, result.Errors[0].Message);
        Assert.Equal("News", category.Name);
        _wrapper.Verify(repository => repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateCategory_ShouldTrimAndPersistChangedName()
    {
        var category = new EventNewsCategory { Id = 10, Name = "News" };
        _categoryRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<EventNewsCategory>>()))
            .ReturnsAsync(category);
        _categoryRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsCategory, bool>>>()))
            .ReturnsAsync(false);
        _wrapper.Setup(repository => repository.SaveChangesAsync()).ReturnsAsync(1);
        var handler = new UpdateEventNewsCategoryHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(
            new UpdateEventNewsCategoryCommand(10, new UpdateEventNewsCategoryDto { Name = "  Updated  " }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated", category.Name);
        _wrapper.Verify(repository => repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCategory_ShouldFail_WhenCategoryDoesNotExist()
    {
        _categoryRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<EventNewsCategory>>()))
            .ReturnsAsync((EventNewsCategory?)null);
        var handler = new DeleteEventNewsCategoryHandler(_wrapper.Object);

        var result = await handler.Handle(new DeleteEventNewsCategoryCommand(10), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains("not found", result.Errors[0].Message, StringComparison.OrdinalIgnoreCase);
        _categoryRepository.Verify(repository => repository.Delete(It.IsAny<EventNewsCategory>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCategory_ShouldFail_WhenCategoryIsAssignedToEventNews()
    {
        _categoryRepository
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<EventNewsCategory>>()))
            .ReturnsAsync(new EventNewsCategory { Id = 10, Name = "News" });
        _eventNewsRepository
            .Setup(repository => repository.ExistsAsync(
                It.IsAny<Expression<Func<EventNewsEntity, bool>>>()))
            .ReturnsAsync(true);
        var handler = new DeleteEventNewsCategoryHandler(_wrapper.Object);

        var result = await handler.Handle(new DeleteEventNewsCategoryCommand(10), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(
            EventNewsCategoryConstants.CantDeleteCategoryWhileAssociatedWithEventNews,
            result.Errors[0].Message);
        _categoryRepository.Verify(repository => repository.Delete(It.IsAny<EventNewsCategory>()), Times.Never);
    }

    [Fact]
    public async Task GetAllCategories_ShouldUseReadOnlyOrderedQuery()
    {
        QueryOptions<EventNewsCategory>? capturedOptions = null;
        _categoryRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<EventNewsCategory>>()))
            .Callback<QueryOptions<EventNewsCategory>>(options => capturedOptions = options)
            .ReturnsAsync([]);
        _mapper.Setup(mapper => mapper.Map<List<AdminEventNewsCategoryDto>>(
                It.IsAny<IEnumerable<EventNewsCategory>>()))
            .Returns([]);
        var handler = new GetAllEventNewsCategoriesHandler(_mapper.Object, _wrapper.Object);

        var result = await handler.Handle(new GetAllEventNewsCategoriesQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedOptions);
        Assert.True(capturedOptions.AsNoTracking);
        Assert.NotNull(capturedOptions.OrderByASC);
    }
}
