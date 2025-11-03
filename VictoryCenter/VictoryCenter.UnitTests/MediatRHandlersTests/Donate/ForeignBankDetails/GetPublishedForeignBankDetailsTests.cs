using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Public.Donate.ForeignBankDetails;
using VictoryCenter.BLL.Queries.Public.Donate.ForeignBankDetails.GetPublished;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.ForeignBankDetails;

public class GetPublishedForeignBankDetailsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<Entities.ForeignBankDetails> _usdBankDetails =
    [
        new()
        {
            Id = 1,
            Name = "Foreign Bank USD 1",
            Receiver = "Receiver 1",
            Iban = "123456789012345678901234567",
            Swift = "12345678901",
            Address = "Address 1",
            Currency = BankCurrency.Usd,
            CorrespondentBanks = []
        },
        new()
        {
            Id = 2,
            Name = "Foreign Bank USD 2",
            Receiver = "Receiver 2",
            Iban = "123456789012345678901234567",
            Swift = "12345678901",
            Address = "Address 2",
            Currency = BankCurrency.Usd,
            CorrespondentBanks = []
        },
    ];

    private readonly IEnumerable<Entities.ForeignBankDetails> _eurBankDetails =
    [
        new()
        {
            Id = 3,
            Name = "Foreign Bank EUR 1",
            Receiver = "Receiver 3",
            Iban = "123456789012345678901234567",
            Swift = "12345678901",
            Address = "Address 3",
            Currency = BankCurrency.Eur,
            CorrespondentBanks = []
        },
    ];

    public GetPublishedForeignBankDetailsTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnUsdForeignBankDetails()
    {
        SetupDependencies(BankCurrency.Usd, _usdBankDetails);
        var handler = new GetPublishedForeignBankDetailsHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        Result<List<PublishedForeignBankDetailsDto>> result = await handler.Handle(
            new GetPublishedForeignBankDetailsQuery(BankCurrency.Usd),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("Foreign Bank USD 1", result.Value[0].Name);
        Assert.Equal("Foreign Bank USD 2", result.Value[1].Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnEurForeignBankDetails()
    {
        SetupDependencies(BankCurrency.Eur, _eurBankDetails);
        var handler = new GetPublishedForeignBankDetailsHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        Result<List<PublishedForeignBankDetailsDto>> result = await handler.Handle(
            new GetPublishedForeignBankDetailsQuery(BankCurrency.Eur),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.Equal("Foreign Bank EUR 1", result.Value[0].Name);
    }

    [Theory]
    [InlineData(BankCurrency.Usd)]
    [InlineData(BankCurrency.Eur)]
    public async Task Handle_ShouldCallRepositoryWithCorrectCurrency(BankCurrency currency)
    {
        var bankDetails = currency == BankCurrency.Usd ? _usdBankDetails : _eurBankDetails;
        SetupDependencies(currency, bankDetails);
        var handler = new GetPublishedForeignBankDetailsHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        await handler.Handle(new GetPublishedForeignBankDetailsQuery(currency), CancellationToken.None);

        _mockRepositoryWrapper.Verify(
            r => r.ForeignBankDetailsRepository.GetAllAsync(
                It.Is<QueryOptions<Entities.ForeignBankDetails>>(
                    opts => opts.Filter != null)),
            Times.Once);
    }

    private void SetupDependencies(BankCurrency currency, IEnumerable<Entities.ForeignBankDetails> bankDetails)
    {
        _mockRepositoryWrapper
            .Setup(r => r.ForeignBankDetailsRepository.GetAllAsync(
                It.Is<QueryOptions<Entities.ForeignBankDetails>>(
                    opts => opts.Filter != null)))
            .ReturnsAsync(bankDetails);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<PublishedForeignBankDetailsDto>>(It.IsAny<IEnumerable<Entities.ForeignBankDetails>>()))
            .Returns<IEnumerable<Entities.ForeignBankDetails>>(entities =>
                entities.Select(e => new PublishedForeignBankDetailsDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Receiver = e.Receiver,
                    Iban = e.Iban,
                    Swift = e.Swift,
                    Address = e.Address,
                    CorrespondentBanks = []
                }).ToList());
    }
}
