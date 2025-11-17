using FluentResults;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.CorrespondentBankDetails;

public class DeleteCorrespondentBankDetailsTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Entities.CorrespondentBankDetails _correspondentBankDetails;

    public DeleteCorrespondentBankDetailsTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _correspondentBankDetails = new Entities.CorrespondentBankDetails
        {
            Id = 1,
            Name = "Test Correspondent Bank",
            Swift = "12345678901",
            Account = "TESTACC12345",
            Iban = "123456789012345678901234567",
            ForeignBankDetailsId = 1
        };
    }

    [Fact]
    public async Task Handle_ShouldDeleteCorrespondentBankDetails_WhenEntityExists()
    {
        // Arrange
        SetupEntityRetrieval(_correspondentBankDetails);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var handler = new DeleteCorrespondentBankDetailsHandler(_repositoryWrapperMock.Object);

        // Act
        Result<long> result = await handler.Handle(new DeleteCorrespondentBankDetailsCommand(_correspondentBankDetails.Id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_correspondentBankDetails.Id, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenEntityNotFound()
    {
        // Arrange
        SetupEntityRetrieval(null);

        var handler = new DeleteCorrespondentBankDetailsHandler(_repositoryWrapperMock.Object);

        // Act
        Result<long> result = await handler.Handle(new DeleteCorrespondentBankDetailsCommand(99), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(99, typeof(Entities.CorrespondentBankDetails)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupEntityRetrieval(_correspondentBankDetails);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var handler = new DeleteCorrespondentBankDetailsHandler(_repositoryWrapperMock.Object);

        // Act
        Result<long> result = await handler.Handle(new DeleteCorrespondentBankDetailsCommand(_correspondentBankDetails.Id), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Entities.CorrespondentBankDetails)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        SetupEntityRetrieval(_correspondentBankDetails);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = new DeleteCorrespondentBankDetailsHandler(_repositoryWrapperMock.Object);

        // Act
        Result<long> result = await handler.Handle(new DeleteCorrespondentBankDetailsCommand(_correspondentBankDetails.Id), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(Entities.CorrespondentBankDetails)), result.Errors[0].Message);
    }

    private void SetupEntityRetrieval(Entities.CorrespondentBankDetails? entity)
    {
        _repositoryWrapperMock.Setup(r => r.CorrespondentBankDetailsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Entities.CorrespondentBankDetails>>()))
            .ReturnsAsync(entity);
    }
}
