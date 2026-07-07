using System.Linq.Expressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Validators.PdfReports;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfReports;

public class ReorderPdfReportsTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly IValidator<ReorderPdfReportsCommand> _validator;

    public ReorderPdfReportsTests()
    {
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
        _mockReorderService = new Mock<IReorderService>();
        _validator = new ReorderPdfReportsCommandValidator();
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
        SetupRepositoryWrapper(pdfIds.Length);
        SetupReorderService();

        var handler = new ReorderPdfReportsHandler(_validator, _mockRepoWrapper.Object, _mockReorderService.Object);

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
        SetupRepositoryWrapper(0);
        SetupReorderService();

        var handler = new ReorderPdfReportsHandler(_validator, _mockRepoWrapper.Object, _mockReorderService.Object);

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
    public async Task Handle_PdfNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = new ReorderPdfReportsCommand(new() { LanguageId = 1, OrderedIds = [2, 1] });
        SetupRepositoryWrapper(0);
        SetupReorderService();
        var handler = new ReorderPdfReportsHandler(_validator, _mockRepoWrapper.Object, _mockReorderService.Object);

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

        SetupRepositoryWrapper(2);

        _mockReorderService.Setup(service => service.SwapElementsAsync<PdfReport>(
            It.IsAny<List<long>>(),
            It.IsAny<Expression<Func<PdfReport, long>>>(),
            It.IsAny<Expression<Func<PdfReport, bool>>>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new ReorderPdfReportsHandler(_validator, _mockRepoWrapper.Object, _mockReorderService.Object);

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

        SetupRepositoryWrapper(2);

        _mockReorderService.Setup(service => service.SwapElementsAsync(
            It.IsAny<List<long>>(),
            It.IsAny<Expression<Func<PdfReport, long>>>(),
            It.IsAny<Expression<Func<PdfReport, bool>>>()))
            .ThrowsAsync(new ReorderException(reorderErrorMessage));

        var handler = new ReorderPdfReportsHandler(_validator, _mockRepoWrapper.Object, _mockReorderService.Object);

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

    private void SetupRepositoryWrapper(int countResult)
    {
        _mockRepoWrapper.Setup(
            repositoryWrapper => repositoryWrapper.PdfReportRepository.CountAsync(
                It.IsAny<QueryOptions<PdfReport>>())).ReturnsAsync(countResult);
    }
}
