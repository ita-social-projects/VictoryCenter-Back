using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpendituresCategories;

public class UpdateReportFundsExpendituresCategoryTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresCategoriesRepository> _categoriesRepositoryMock;
    private readonly IValidator<UpdateReportFundsExpendituresCategoryCommand> _validator;

    private readonly ReportFundsExpendituresCategory _existingCategory = new()
    {
        Id = 1,
        Name = "Old name",
        Type = ReportFundsExpendituresType.Income
    };

    private readonly UpdateReportFundsExpendituresCategoryDto _updateDto = new()
    {
        Name = "Updated name"
    };

    private readonly ReportFundsExpendituresCategoryDto _updatedDto = new()
    {
        Id = 1,
        Name = "Updated name",
        Type = ReportFundsExpendituresType.Income
    };

    public UpdateReportFundsExpendituresCategoryTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _categoriesRepositoryMock = new Mock<IReportFundsExpendituresCategoriesRepository>();
        _validator = new UpdateReportFundsExpendituresCategoryValidator();
    }

    [Fact]
    public async Task Handle_ShouldUpdateCategory()
    {
        // Arrange
        SetupDependencies([_existingCategory], saveResult: 1);
        var handler = new UpdateReportFundsExpendituresCategoryHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresCategoryCommand(_updateDto, _existingCategory.Id),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_updatedDto.Name, result.Value.Name);
        Assert.Equal(_updatedDto.Type, result.Value.Type);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_WhenNameIsInvalid(string? name)
    {
        // Arrange
        var invalidDto = new UpdateReportFundsExpendituresCategoryDto { Name = name! };
        var handler = new UpdateReportFundsExpendituresCategoryHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresCategoryCommand(invalidDto, _existingCategory.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryNotFound()
    {
        // Arrange
        SetupDependencies([], saveResult: 1);
        var handler = new UpdateReportFundsExpendituresCategoryHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresCategoryCommand(_updateDto, 999),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(999, typeof(ReportFundsExpendituresCategory)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDuplicateNameExistsForSameType()
    {
        // Arrange
        var duplicateCategory = new ReportFundsExpendituresCategory
        {
            Id = 2,
            Name = _updateDto.Name,
            Type = _existingCategory.Type
        };

        SetupDependencies([_existingCategory, duplicateCategory], saveResult: 1);
        var handler = new UpdateReportFundsExpendituresCategoryHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresCategoryCommand(_updateDto, _existingCategory.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ReportFundsExpendituresCategoryConstants.DuplicateCategoryName, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupDependencies([_existingCategory], saveResult: 0);
        var handler = new UpdateReportFundsExpendituresCategoryHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresCategoryCommand(_updateDto, _existingCategory.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(ReportFundsExpendituresCategory)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        SetupDependencies([_existingCategory], saveResult: 1);
        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = new UpdateReportFundsExpendituresCategoryHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresCategoryCommand(_updateDto, _existingCategory.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(ReportFundsExpendituresCategory)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(IEnumerable<ReportFundsExpendituresCategory> categories, int saveResult)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresCategoriesRepository)
            .Returns(_categoriesRepositoryMock.Object);

        _categoriesRepositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<ReportFundsExpendituresCategory>>()))
            .ReturnsAsync(categories);

        _categoriesRepositoryMock
            .Setup(repository => repository.Update(It.IsAny<ReportFundsExpendituresCategory>()));

        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveResult);

        _mapperMock
            .Setup(mapper => mapper.Map(
                It.IsAny<UpdateReportFundsExpendituresCategoryDto>(),
                It.IsAny<ReportFundsExpendituresCategory>()))
            .Callback<UpdateReportFundsExpendituresCategoryDto, ReportFundsExpendituresCategory>(
                (dto, category) => category.Name = dto.Name)
            .Returns((UpdateReportFundsExpendituresCategoryDto _, ReportFundsExpendituresCategory category) => category);

        _mapperMock.Setup(mapper => mapper.Map<ReportFundsExpendituresCategoryDto>(It.IsAny<ReportFundsExpendituresCategory>()))
            .Returns(_updatedDto);
    }
}
