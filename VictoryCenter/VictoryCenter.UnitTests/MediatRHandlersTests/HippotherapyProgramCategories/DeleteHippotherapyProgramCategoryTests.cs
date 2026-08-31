using FluentResults;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.HippotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyProgramCategories;

public class DeleteHippotherapyProgramCategoryTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly HippotherapyProgramCategory _programCategoryWithNoPrograms;
    private readonly HippotherapyProgramCategory _programCategoryWithPrograms;
    private readonly HippotherapyProgramCategory _programCategoryWithLocalizations;

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

        _programCategoryWithLocalizations = new HippotherapyProgramCategory
        {
            Id = 3,
            Name = "With Localizations",
            Programs = [],
            Localizations =
            [
                new HippotherapyProgramCategoryLocalization { EntityId = 3, LanguageId = 1, Name = "Localized name" }
            ]
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
    public async Task Handle_ShouldDeleteLocalizations_WhenCategoryHasAttachedLocalizations()
    {
        SetupCategoryRetrieval(_programCategoryWithLocalizations);

        // Setup the HippotherapyProgramCategoryLocalizationsRepository mock
        _repositoryWrapperMock
            .Setup(r => r.HippotherapyProgramCategoryLocalizationsRepository)
            .Returns(Mock.Of<IHippotherapyProgramCategoryLocalizationsRepository>());

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var handler = new DeleteHippotherapyProgramCategoryHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteHippotherapyProgramCategoryCommand(_programCategoryWithLocalizations.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);

        _repositoryWrapperMock.Verify(
            r => r.HippotherapyProgramCategoryLocalizationsRepository.DeleteRange(
                It.Is<IEnumerable<HippotherapyProgramCategoryLocalization>>(list =>
                    list.Any(l => l.EntityId == _programCategoryWithLocalizations.Id && l.LanguageId == 1))),
            Times.Once);
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
