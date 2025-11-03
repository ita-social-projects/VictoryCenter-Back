using FluentResults;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.ForeignBankDetails;

public class DeleteForeignBankDetailsTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Entities.ForeignBankDetails _foreignBankDetails;

    public DeleteForeignBankDetailsTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        _foreignBankDetails = new Entities.ForeignBankDetails
        {
            Id = 1,
            Name = "Test Foreign Bank",
            Receiver = "Test Receiver",
            Iban = "123456789012345678901234567",
            Swift = "12345678901",
            Address = "Test Address",
            CorrespondentBanks = []
        };
    }

    [Fact]
    public async Task Handle_ShouldDeleteForeignBankDetails_WhenEntityExists()
    {
        SetupEntityRetrieval(_foreignBankDetails);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var handler = new DeleteForeignBankDetailsHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteForeignBankDetailsCommand(_foreignBankDetails.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_foreignBankDetails.Id, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenEntityNotFound()
    {
        SetupEntityRetrieval(null);
        var handler = new DeleteForeignBankDetailsHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteForeignBankDetailsCommand(99), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(99, typeof(Entities.ForeignBankDetails)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        SetupEntityRetrieval(_foreignBankDetails);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var handler = new DeleteForeignBankDetailsHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteForeignBankDetailsCommand(_foreignBankDetails.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Entities.ForeignBankDetails)), result.Errors[0].Message);
    }

    private void SetupEntityRetrieval(Entities.ForeignBankDetails? entity)
    {
        _repositoryWrapperMock.Setup(r => r.ForeignBankDetailsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Entities.ForeignBankDetails>>()))
            .ReturnsAsync(entity);
    }
}
