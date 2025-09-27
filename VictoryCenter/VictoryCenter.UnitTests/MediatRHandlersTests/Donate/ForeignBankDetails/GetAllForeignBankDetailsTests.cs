using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.BLL.Queries.Admin.Donate.ForeignBankDetails.GetAll;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.ForeignBankDetails;

public class GetAllForeignBankDetailsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<Entities.ForeignBankDetails> _testEntities =
    [
        new()
        {
            Id = 1,
            Name = "Foreign Bank 1",
            Receiver = "Receiver 1",
            Iban = "123456789012345678901234567",
            Swift = "12345678901",
            Address = "Address 1",
            CorrespondentBanks = []
        },
        new()
        {
            Id = 2,
            Name = "Foreign Bank 2",
            Receiver = "Receiver 2",
            Iban = "123456789012345678901234567",
            Swift = "12345678901",
            Address = "Address 2",
            CorrespondentBanks = []
        }

    ];

    private readonly IEnumerable<ForeignBankDetailsDto> _testDtos =
    [
        new()
        {
            Id = 1,
            Name = "Foreign Bank 1",
            Receiver = "Receiver 1",
            Iban = "123456789012345678901234567",
            Swift = "12345678901",
            Address = "Address 1",
            CorrespondentBanks = []
        },
        new()
        {
            Id = 2,
            Name = "Foreign Bank 2",
            Receiver = "Receiver 2",
            Iban = "123456789012345678901234567",
            Swift = "12345678901",
            Address = "Address 2",
            CorrespondentBanks = []
        }

    ];

    public GetAllForeignBankDetailsTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllForeignBankDetails()
    {
        SetupDependencies();
        var handler = new GetAllForeignBankDetailsHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        Result<List<ForeignBankDetailsDto>> result = await handler.Handle(new GetAllForeignBankDetailsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("Foreign Bank 1", result.Value[0].Name);
        Assert.Equal("Foreign Bank 2", result.Value[1].Name);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<IEnumerable<ForeignBankDetailsDto>>(It.IsAny<IEnumerable<Entities.ForeignBankDetails>>()))
            .Returns(_testDtos);

        _mockRepositoryWrapper.Setup(r => r.ForeignBankDetailsRepository
            .GetAllAsync(It.IsAny<QueryOptions<Entities.ForeignBankDetails>>()))
            .ReturnsAsync(_testEntities);
    }
}
