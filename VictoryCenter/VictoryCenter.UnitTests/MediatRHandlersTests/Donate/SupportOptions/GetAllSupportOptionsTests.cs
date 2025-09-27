using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.BLL.Queries.Admin.Donate.SupportOptions.GetAll;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.SupportOptions;
public class GetAllSupportOptionsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<Entities.SupportOptions> _testEntities =
    [
        new()
        {
            Id = 1,
            Name = "Option1",
            Value = "Value1"
        },
        new()
        {
            Id = 2,
            Name = "Option2",
            Value = "Value2"
        }

    ];

    private readonly IEnumerable<SupportOptionsDto> _testDtos =
    [
        new()
        {
            Id = 1,
            Name = "Option1",
            Value = "Value1"
        },
        new()
        {
            Id = 2,
            Name = "Option2",
            Value = "Value2"
        }

    ];

    public GetAllSupportOptionsTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllSupportOptions()
    {
        SetupDependencies();

        var handler = new GetAllSupportOptionsHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        Result<List<SupportOptionsDto>> result =
            await handler.Handle(new GetAllSupportOptionsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("Option1", result.Value[0].Name);
        Assert.Equal("Option2", result.Value[1].Name);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<IEnumerable<SupportOptionsDto>>(It.IsAny<IEnumerable<Entities.SupportOptions>>()))
            .Returns(_testDtos);

        _mockRepositoryWrapper.Setup(r => r.SupportOptionsRepository.GetAllAsync(null))
            .ReturnsAsync(_testEntities);
    }
}
