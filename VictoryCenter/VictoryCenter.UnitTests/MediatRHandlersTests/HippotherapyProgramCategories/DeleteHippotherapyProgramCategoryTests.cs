using FluentResults;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyProgramCategories;

public class DeleteHippotherapyProgramCategoryTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly HippotherapyProgramCategory _programCategoryWithNoPrograms;
    private readonly HippotherapyProgramCategory _programCategoryWithPrograms;

    public DeleteHippotherapyProgramCategoryTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        _programCategoryWithNoPrograms = new HippotherapyProgramCategory
        {
            Id = 1,
            Name = "Without Programs",
            Programs = []
        };

        _programCategoryWithPrograms = new HippotherapyProgramCategory
        {
            Id = 2,
            Name = "With Programs",
            Programs = [new()]
        };
    }

    [Fact]
    public async Task Handle_ShouldDeleteCategory_WhenNoProgramsAssociated()
    {
        SetupCategoryRetrieval(_programCategoryWithNoPrograms);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var handler = new DeleteHippotherapyProgramCategoryHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteHippotherapyProgramCategoryCommand(_programCategoryWithNoPrograms.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_programCategoryWithNoPrograms.Id, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryHasPrograms()
    {
        SetupCategoryRetrieval(_programCategoryWithPrograms);

        var handler = new DeleteHippotherapyProgramCategoryHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteHippotherapyProgramCategoryCommand(_programCategoryWithPrograms.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(HippotherapyProgramCategoryConstants.CantDeleteProgramCategoryWhileAssociatedWithAnyProgram, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryNotFound()
    {
        SetupCategoryRetrieval(null);
        var handler = new DeleteHippotherapyProgramCategoryHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteHippotherapyProgramCategoryCommand(99), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("was not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        SetupCategoryRetrieval(_programCategoryWithNoPrograms);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var handler = new DeleteHippotherapyProgramCategoryHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteHippotherapyProgramCategoryCommand(_programCategoryWithNoPrograms.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(HippotherapyProgramCategory)), result.Errors[0].Message);
    }

    private void SetupCategoryRetrieval(HippotherapyProgramCategory? category)
    {
        _repositoryWrapperMock.Setup(r => r.HippotherapyProgramCategoriesRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync(category);
    }
}
