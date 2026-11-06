using System.Transactions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.PdfReports.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Hubs;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.UnitTests.Utils.SignalR;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.PdfReports;

public class DeletePdfReportHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly Mock<IHubContext<PdfReportsHub>> _mockHubContext;
    private readonly PdfReport _existingReport;

    public DeletePdfReportHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockReorderService = new Mock<IReorderService>();
        _mockHubContext = HubContextMockFactory.Create<PdfReportsHub>();
        _existingReport = new PdfReport
        {
            Id = 1,
            Name = "Report 2024",
            BlobName = "blob-name-123",
            FileSizeBytes = 1024000,
            Priority = 1,
            LanguageId = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _mockRepo.Setup(r => r.BeginTransaction())
                 .Returns(() => new TransactionScope(
                     TransactionScopeOption.Suppress,
                     TransactionScopeAsyncFlowOption.Enabled));
    }

    [Fact]
    public async Task Handle_ValidRequest_DeletesReportAndReorders()
    {
        // Arrange
        var command = new DeletePdfReportCommand(1);

        _mockRepo.Setup(r => r.PdfReportRepository.GetFirstOrDefaultAsync(
                     It.IsAny<QueryOptions<PdfReport>>()))
                 .ReturnsAsync(_existingReport);

        _mockRepo.Setup(r => r.PdfReportRepository.Delete(It.IsAny<PdfReport>()));

        _mockRepo.SetupSequence(r => r.SaveChangesAsync())
                 .ReturnsAsync(1)
                 .ReturnsAsync(1);

        _mockReorderService.Setup(r => r.RenumberPriorityAsync<PdfReport>(
                                It.IsAny<System.Linq.Expressions.Expression<Func<PdfReport, bool>>>()))
                           .Returns(Task.CompletedTask);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRepo.Verify(r => r.PdfReportRepository.Delete(_existingReport), Times.Once);
        _mockReorderService.Verify(
            r => r.RenumberPriorityAsync<PdfReport>(
            It.IsAny<System.Linq.Expressions.Expression<Func<PdfReport, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReportNotFound_ReturnsFailResult()
    {
        // Arrange
        var command = new DeletePdfReportCommand(999);

        _mockRepo.Setup(r => r.PdfReportRepository.GetFirstOrDefaultAsync(
                     It.IsAny<QueryOptions<PdfReport>>()))
                 .ReturnsAsync((PdfReport)null!);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.NotFound(999, typeof(PdfReport)),
            result.Errors.Select(e => e.Message));
        _mockRepo.Verify(r => r.PdfReportRepository.Delete(It.IsAny<PdfReport>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DbUpdateException_ReturnsFailResult()
    {
        // Arrange
        var command = new DeletePdfReportCommand(1);

        _mockRepo.Setup(r => r.PdfReportRepository.GetFirstOrDefaultAsync(
                     It.IsAny<QueryOptions<PdfReport>>()))
                 .ReturnsAsync(_existingReport);

        _mockRepo.Setup(r => r.PdfReportRepository.Delete(It.IsAny<PdfReport>()));

        _mockRepo.Setup(r => r.SaveChangesAsync())
                 .ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ValidRequest_NotifiesClientsAfterSavingMetadata()
    {
        // Arrange
        var command = new DeletePdfReportCommand(1);

        _mockRepo.Setup(r => r.PdfReportRepository.GetFirstOrDefaultAsync(
                     It.IsAny<QueryOptions<PdfReport>>()))
                 .ReturnsAsync(_existingReport);

        _mockRepo.Setup(r => r.PdfReportRepository.Delete(It.IsAny<PdfReport>()));

        _mockRepo.SetupSequence(r => r.SaveChangesAsync())
                 .ReturnsAsync(1);

        _mockReorderService.Setup(r => r.RenumberPriorityAsync<PdfReport>(
                                It.IsAny<System.Linq.Expressions.Expression<Func<PdfReport, bool>>>()))
                           .Returns(Task.CompletedTask);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockHubContext.Verify(hub => hub.Clients.All.SendCoreAsync("PdfReportDeleted", It.Is<object[]>(arguments => (long)arguments[0] == 1), It.IsAny<CancellationToken>()), Times.Once);
    }

    private DeletePdfReportHandler CreateHandler() =>
        new(
            _mockRepo.Object,
            _mockReorderService.Object,
            _mockHubContext.Object);
}
