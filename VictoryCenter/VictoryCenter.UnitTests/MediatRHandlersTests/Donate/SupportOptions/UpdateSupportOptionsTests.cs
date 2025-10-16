using AutoMapper;
using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.BLL.Validators.Donate.SupportOptions;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.SupportOptions;
public class UpdateSupportOptionsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<UpdateSupportOptionsCommand> _validator;

    private readonly UpdateSupportOptionsDto _updateDto = new()
    {
        Name = "UpdatedName",
        Value = "UpdatedValue"
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
        Name = "UpdatedName",
        Value = "UpdatedValue"
    };

    public UpdateSupportOptionsTests()
    {
        _mockMapper = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new UpdateSupportOptionsCommandValidator();
    }

    [Fact]
    public async Task Handle_ShouldFail_EntityNotFound()
    {
        SetupDependencies(entityExists: false);
        var handler = new UpdateSupportOptionsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        Result<SupportOptionsDto> result = await handler.Handle(
            new UpdateSupportOptionsCommand(_updateDto, 99),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(99, typeof(Entities.SupportOptions)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        SetupDependencies(saveResult: -1);
        var handler = new UpdateSupportOptionsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        Result<SupportOptionsDto> result = await handler.Handle(
            new UpdateSupportOptionsCommand(_updateDto, 1),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Entities.SupportOptions)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateSupportOptions()
    {
        SetupDependencies();
        var handler = new UpdateSupportOptionsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        Result<SupportOptionsDto> result = await handler.Handle(
            new UpdateSupportOptionsCommand(_updateDto, 1),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_supportOptionsDto.Name, result.Value.Name);
        Assert.Equal(_supportOptionsDto.Value, result.Value.Value);
    }

    private void SetupDependencies(int saveResult = 1, bool entityExists = true)
    {
        SetUpAutomapper(_supportOptionsEntity, _supportOptionsDto);
        SetUpRepositoryWrapper(saveResult, entityExists);
    }

    private void SetUpAutomapper(Entities.SupportOptions outputEntity, SupportOptionsDto outputDto)
    {
        _mockMapper.Setup(m => m.Map<SupportOptionsDto>(It.IsAny<Entities.SupportOptions>()))
            .Returns(outputDto);
        _mockMapper.Setup(m => m.Map(
                It.IsAny<UpdateSupportOptionsDto>(),
                It.IsAny<Entities.SupportOptions>()))
            .Returns(outputEntity);
    }

    private void SetUpRepositoryWrapper(int saveResult, bool entityExists)
    {
        _repositoryWrapperMock.Setup(repo => repo.SupportOptionsRepository
            .Update(It.IsAny<Entities.SupportOptions>()));
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
        _repositoryWrapperMock.Setup(repo => repo.SupportOptionsRepository
                .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Entities.SupportOptions>>()))
                .ReturnsAsync(entityExists ? _supportOptionsEntity : null);
    }
}
