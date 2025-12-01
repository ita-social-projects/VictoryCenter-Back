using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Public.Donate.UahBankDetails;
using VictoryCenter.BLL.Queries.Public.Donate.UahBankDetails.GetPublished;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.UahBankDetails;

public class GetPublishedUahBankDetailsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<Entities.UahBankDetails> _testEntities =
    [
        new()
        {
            Id = 1,
            Name = "Bank 1",
            Receiver = "Receiver 1",
            Edrpou = "11111111",
            UkrainianIban = "UA123456789012345678901234567",
            PaymentPurpose = "Purpose 1"
        },
        new()
        {
            Id = 2,
            Name = "Bank 2",
            Receiver = "Receiver 2",
            Edrpou = "22222222",
            UkrainianIban = "UA123456789012345678901234567",
            PaymentPurpose = "Purpose 2"
        }

    ];

    private readonly IEnumerable<PublishedUahBankDetailsDto> _testDtos =
    [
        new()
        {
            Id = 1,
            Name = "Bank 1",
            Receiver = "Receiver 1",
            Edrpou = "11111111",
            UkrainianIban = "UA123456789012345678901234567",
            PaymentPurpose = "Purpose 1"
        },
        new()
        {
            Id = 2,
            Name = "Bank 2",
            Receiver = "Receiver 2",
            Edrpou = "22222222",
            UkrainianIban = "UA123456789012345678901234567",
            PaymentPurpose = "Purpose 2"
        }

    ];

    public GetPublishedUahBankDetailsTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllUahBankDetails()
    {
        SetupDependencies();
        var handler = new GetPublishedUahBankDetailsHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        Result<List<PublishedUahBankDetailsDto>> result = await handler.Handle(new GetPublishedUahBankDetailsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("Bank 1", result.Value[0].Name);
        Assert.Equal("Bank 2", result.Value[1].Name);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<IEnumerable<PublishedUahBankDetailsDto>>(It.IsAny<IEnumerable<Entities.UahBankDetails>>()))
            .Returns(_testDtos.ToList());

        _mockRepositoryWrapper.Setup(r => r.UahBankDetailsRepository.GetAllAsync(null))
            .ReturnsAsync(_testEntities);
    }
}
