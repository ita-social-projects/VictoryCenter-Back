using Moq;
using FluentResults;
using VictoryCenter.BLL.Commands.ProgramCategories.Delete;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ProgramCategories;

public class DeleteProgramCategoryTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly ProgramCategory _programCategoryWithNoPrograms;
    private readonly ProgramCategory _programCategoryWithPrograms;

    public DeleteProgramCategoryTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        _programCategoryWithNoPrograms = new ProgramCategory
        {
            Id = 1,
            Name = "Without Programs",
            Programs = new List<DAL.Entities.Program>()
        };

        _programCategoryWithPrograms = new ProgramCategory
        {
            Id = 2,
            Name = "With Programs",
            Programs = new List<DAL.Entities.Program> { new() }
        };
    }

    [Fact]
    public async Task Handle_ShouldDeleteCategory_WhenNoProgramsAssociated()
    {
        SetupCategoryRetrieval(_programCategoryWithNoPrograms);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var handler = new DeleteProgramCategoryHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteProgramCategoryCommand(_programCategoryWithNoPrograms.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_programCategoryWithNoPrograms.Id, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryHasPrograms()
    {
        SetupCategoryRetrieval(_programCategoryWithPrograms);

        var handler = new DeleteProgramCategoryHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteProgramCategoryCommand(_programCategoryWithPrograms.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ProgramCategoryConstants.CantDeleteProgramCategoryWhileAssociatedWithAnyProgram, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryNotFound()
    {
        SetupCategoryRetrieval(null);
        var handler = new DeleteProgramCategoryHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteProgramCategoryCommand(99), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("was not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        SetupCategoryRetrieval(_programCategoryWithNoPrograms);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var handler = new DeleteProgramCategoryHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteProgramCategoryCommand(_programCategoryWithNoPrograms.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ProgramCategoryConstants.FailedToDeleteCategory, result.Errors[0].Message);
    }

    private void SetupCategoryRetrieval(ProgramCategory? category)
    {
        _repositoryWrapperMock.Setup(r => r.ProgramCategoriesRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ProgramCategory>>()))
            .ReturnsAsync(category);
    }
}
