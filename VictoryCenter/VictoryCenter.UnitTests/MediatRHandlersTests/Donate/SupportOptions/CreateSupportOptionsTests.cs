using AutoMapper;
using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.BLL.Validators.Donate.SupportOptions;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.SupportOptions;
public class CreateSupportOptionsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<CreateSupportOptionsCommand> _validator;

    private readonly CreateSupportOptionsDto _createDto = new()
    {
        Name = "Option1",
        Value = "Value1"
    };

    private readonly Entities.SupportOptions _supportOptionsEntity = new()
    {
        Id = 1,
        Name = "Option1",
        Value = "Value1"
    };

    private readonly SupportOptionsDto _supportOptionsDto = new()
    {
        Id = 1,
        Name = "Option1",
        Value = "Value1"
    };

    public CreateSupportOptionsTests()
    {
        _mockMapper = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new CreateSupportOptionsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        var dto = new CreateSupportOptionsDto { Name = name!, Value = "Test" };
        var handler = new CreateSupportOptionsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        Result<SupportOptionsDto> result = await handler.Handle(new CreateSupportOptionsCommand(dto), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        SetupDependencies(0);
        var handler = new CreateSupportOptionsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        Result<SupportOptionsDto> result = await handler.Handle(new CreateSupportOptionsCommand(_createDto), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(Entities.SupportOptions)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldCreateSupportOption()
    {
        SetupDependencies();
        var handler = new CreateSupportOptionsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        Result<SupportOptionsDto> result = await handler.Handle(new CreateSupportOptionsCommand(_createDto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_supportOptionsDto.Name, result.Value.Name);
    }

    private void SetupDependencies(int saveResult = 1)
    {
        _mockMapper.Setup(m => m.Map<Entities.SupportOptions>(It.IsAny<CreateSupportOptionsDto>()))
            .Returns(_supportOptionsEntity);
        _mockMapper.Setup(m => m.Map<SupportOptionsDto>(It.IsAny<Entities.SupportOptions>()))
            .Returns(_supportOptionsDto);

        _repositoryWrapperMock.Setup(repo => repo.SupportOptionsRepository
            .CreateAsync(It.IsAny<Entities.SupportOptions>(), new CancellationToken()));
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
