using System.Linq.Expressions;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Hubs;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Validators.PdfReports;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.UnitTests.Utils.SignalR;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfReports;

public class ReorderPdfReportsTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly Mock<IHubContext<PdfReportsHub>> _mockHubContext;
    private readonly IValidator<ReorderPdfReportsCommand> _validator;

    public ReorderPdfReportsTests()
    {
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
        _mockReorderService = new Mock<IReorderService>();
        _mockHubContext = HubContextMockFactory.Create<PdfReportsHub>();
        _validator = new ReorderPdfReportsCommandValidator();

        // Mock language exist check to return true by default
        _mockRepoWrapper.Setup(
            repositoryWrapper => repositoryWrapper.LocalizationLanguagesRepository.ExistsAsync(
                It.IsAny<Expression<Func<LocalizationLanguage, bool>>>()))
            .ReturnsAsync(true);
    }

    [Theory]
    [InlineData(2L, 1L)]
    [InlineData(3L, 2L, 1L)]
    [InlineData(1L, 2L, 4L, 3L)]
    [InlineData(4L, 1L, 2L, 3L, 5L)]
    public async Task Handle_DtoIsValid_ShouldReturnOk(params long[] pdfIds)
    {
        // Arrange
        var command = new ReorderPdfReportsCommand(new() { LanguageId = 1, OrderedIds = [.. pdfIds] });
        SetupRepositoryWrapper(pdfIds.Length, 1, [.. pdfIds]);
        SetupReorderService();

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        _mockReorderService.Verify(
            service => service.SwapElementsAsync(
                It.Is<List<long>>(ids => ids.SequenceEqual(pdfIds)),
                It.IsAny<Expression<Func<PdfReport, long>>>(),
                It.IsAny<Expression<Func<PdfReport, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DtoIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        var command = new ReorderPdfReportsCommand(new() { LanguageId = -10, OrderedIds = [2, 1] });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Contains(
            ErrorMessagesConstants.PropertyMustBePositive(nameof(ReorderPdfReportsDto.LanguageId)),
            result.Errors[0].Message,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_LanguageDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = new ReorderPdfReportsCommand(new() { LanguageId = 999, OrderedIds = [2, 1] });

        _mockRepoWrapper.Setup(
            repositoryWrapper => repositoryWrapper.LocalizationLanguagesRepository.ExistsAsync(
                It.Is<Expression<Func<LocalizationLanguage, bool>>>(expr => expr.Compile()(new LocalizationLanguage { Id = 999 }))))
            .ReturnsAsync(false);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(999, typeof(LocalizationLanguage)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_NotAllPdfReportsFound_ShouldReturnFailure()
    {
        // Arrange
        var command = new ReorderPdfReportsCommand(new() { LanguageId = 1, OrderedIds = [2, 1] });
        SetupRepositoryWrapper(1, 1, [2, 1]);
        SetupReorderService();
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ReorderConstants.NotAllEntitiesFoundForReorder(1, 2), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_OrderedIdsIsNull_ShouldReturnFailure()
    {
        // Arrange
        var command = new ReorderPdfReportsCommand(new() { LanguageId = 1, OrderedIds = null! });
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Contains(
            ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(ReorderPdfReportsDto.OrderedIds)),
            result.Errors[0].Message,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_PdfNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = new ReorderPdfReportsCommand(new() { LanguageId = 1, OrderedIds = [2, 1] });
        SetupRepositoryWrapper(0, 1, [2, 1]);
        SetupReorderService();
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(PdfReportConstants.PdfNotFound, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_DbExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        var command = new ReorderPdfReportsCommand(new() { LanguageId = 1, OrderedIds = [2, 1] });

        SetupRepositoryWrapper(2, 1, [2, 1]);

        _mockReorderService.Setup(service => service.SwapElementsAsync<PdfReport>(
            It.IsAny<List<long>>(),
            It.IsAny<Expression<Func<PdfReport, long>>>(),
            It.IsAny<Expression<Func<PdfReport, bool>>>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PdfReport)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ReorderExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        var command = new ReorderPdfReportsCommand(new() { LanguageId = 1, OrderedIds = [2, 1] });
        var reorderErrorMessage = "Test reorder error";

        SetupRepositoryWrapper(2, 1, [2, 1]);

        _mockReorderService.Setup(service => service.SwapElementsAsync(
            It.IsAny<List<long>>(),
            It.IsAny<Expression<Func<PdfReport, long>>>(),
            It.IsAny<Expression<Func<PdfReport, bool>>>()))
            .ThrowsAsync(new ReorderException(reorderErrorMessage));

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ReorderConstants.ErrorWithReordering(reorderErrorMessage), result.Errors[0].Message);
    }

    private void SetupReorderService()
    {
        _mockReorderService
            .Setup(service => service.SwapElementsAsync<PdfReport>(
                It.IsAny<List<long>>(),
                It.IsAny<Expression<Func<PdfReport, long>>>(),
                It.IsAny<Expression<Func<PdfReport, bool>>>()))
            .Returns(Task.CompletedTask);
    }

    private ReorderPdfReportsHandler CreateHandler() =>
        new(_validator, _mockRepoWrapper.Object, _mockReorderService.Object, _mockHubContext.Object);

    private void SetupRepositoryWrapper(int countResult, long expectedLanguageId, List<long> expectedOrderedIds)
    {
        _mockRepoWrapper.Setup(
            repositoryWrapper => repositoryWrapper.PdfReportRepository.CountAsync(
                It.Is<QueryOptions<PdfReport>>(options =>
                    options != null &&
                    options.Filter != null &&
                    VerifyFilter(options.Filter, expectedLanguageId, expectedOrderedIds))))
            .ReturnsAsync(countResult);
    }

    private static bool VerifyFilter(Expression<Func<PdfReport, bool>> filter, long expectedLanguageId, List<long> expectedOrderedIds)
    {
        var compiled = filter.Compile();

        foreach (var id in expectedOrderedIds)
        {
            var report = new PdfReport { Id = id, LanguageId = expectedLanguageId };
            if (!compiled(report))
            {
                return false;
            }
        }

        foreach (var id in expectedOrderedIds)
        {
            var report = new PdfReport { Id = id, LanguageId = expectedLanguageId + 1 };
            if (compiled(report))
            {
                return false;
            }
        }

        var mismatchingIdReport = new PdfReport { Id = -999, LanguageId = expectedLanguageId };
        if (compiled(mismatchingIdReport))
        {
            return false;
        }

        return true;
    }
}
