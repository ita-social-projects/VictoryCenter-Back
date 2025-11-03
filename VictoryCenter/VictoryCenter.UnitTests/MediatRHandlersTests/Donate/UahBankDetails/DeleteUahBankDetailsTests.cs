using FluentResults;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.UahBankDetails;

public class DeleteUahBankDetailsTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Entities.UahBankDetails _uahBankDetails;

    public DeleteUahBankDetailsTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        _uahBankDetails = new Entities.UahBankDetails
        {
            Id = 1,
            Name = "Test Bank",
            Receiver = "Test Receiver",
            Edrpou = "12345678",
            Iban = "123456789012345678901234567",
            PaymentPurpose = "Test purpose"
        };
    }

    [Fact]
    public async Task Handle_ShouldDeleteUahBankDetails_WhenEntityExists()
    {
        SetupEntityRetrieval(_uahBankDetails);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var handler = new DeleteUahBankDetailsHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteUahBankDetailsCommand(_uahBankDetails.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_uahBankDetails.Id, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenEntityNotFound()
    {
        SetupEntityRetrieval(null);
        var handler = new DeleteUahBankDetailsHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteUahBankDetailsCommand(99), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(99, typeof(Entities.UahBankDetails)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        SetupEntityRetrieval(_uahBankDetails);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var handler = new DeleteUahBankDetailsHandler(_repositoryWrapperMock.Object);

        Result<long> result = await handler.Handle(new DeleteUahBankDetailsCommand(_uahBankDetails.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Entities.UahBankDetails)), result.Errors[0].Message);
    }

    private void SetupEntityRetrieval(Entities.UahBankDetails? entity)
    {
        _repositoryWrapperMock.Setup(r => r.UahBankDetailsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Entities.UahBankDetails>>()))
            .ReturnsAsync(entity);
    }
}
