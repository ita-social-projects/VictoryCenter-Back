using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.BLL.Queries.Admin.Donate.SupportOptions.GetAll;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.SupportOptions;

public class GetAllSupportOptionsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<Entities.SupportOptions> _usdSupportOptions =
    [
        new()
        {
            Id = 1,
            Name = "USD Option 1",
            Value = "Value 1",
            Currency = BankCurrency.Usd
        },
        new()
        {
            Id = 2,
            Name = "USD Option 2",
            Value = "Value 2",
            Currency = BankCurrency.Usd
        },
    ];

    private readonly IEnumerable<Entities.SupportOptions> _eurSupportOptions =
    [
        new()
        {
            Id = 3,
            Name = "EUR Option 1",
            Value = "Value 3",
            Currency = BankCurrency.Eur
        },
    ];

    public GetAllSupportOptionsTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnUsdSupportOptions()
    {
        SetupDependencies(BankCurrency.Usd, _usdSupportOptions);
        var handler = new GetAllSupportOptionsHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        Result<List<SupportOptionsDto>> result = await handler.Handle(
            new GetAllSupportOptionsQuery(BankCurrency.Usd),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("USD Option 1", result.Value[0].Name);
        Assert.Equal("USD Option 2", result.Value[1].Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnEurSupportOptions()
    {
        SetupDependencies(BankCurrency.Eur, _eurSupportOptions);
        var handler = new GetAllSupportOptionsHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        Result<List<SupportOptionsDto>> result = await handler.Handle(
            new GetAllSupportOptionsQuery(BankCurrency.Eur),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.Equal("EUR Option 1", result.Value[0].Name);
    }

    [Theory]
    [InlineData(BankCurrency.Usd)]
    [InlineData(BankCurrency.Eur)]
    public async Task Handle_ShouldCallRepositoryWithCorrectCurrency(BankCurrency currency)
    {
        var supportOptions = currency == BankCurrency.Usd ? _usdSupportOptions : _eurSupportOptions;

        SetupDependencies(currency, supportOptions);
        var handler = new GetAllSupportOptionsHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        await handler.Handle(new GetAllSupportOptionsQuery(currency), CancellationToken.None);

        _mockRepositoryWrapper.Verify(
            r => r.SupportOptionsRepository.GetAllAsync(
                It.Is<QueryOptions<Entities.SupportOptions>>(
                    opts => opts.Filter != null)),
            Times.Once);
    }

    private void SetupDependencies(BankCurrency currency, IEnumerable<Entities.SupportOptions> supportOptions)
    {
        _mockRepositoryWrapper
            .Setup(r => r.SupportOptionsRepository.GetAllAsync(
                It.Is<QueryOptions<Entities.SupportOptions>>(
                    opts => opts.Filter != null)))
            .ReturnsAsync(supportOptions);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<SupportOptionsDto>>(It.IsAny<IEnumerable<Entities.SupportOptions>>()))
            .Returns<IEnumerable<Entities.SupportOptions>>(entities =>
                entities.Select(e => new SupportOptionsDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Value = e.Value
                }).ToList());
    }
}
