using AutoMapper;
using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.BLL.Validators.Donate;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.UahBankDetails;
public class UpdateUahBankDetailsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<UpdateUahBankDetailsCommand> _validator;

    private readonly UpdateUahBankDetailsDto _updateDto = new()
    {
        Name = "Updated Bank",
        Receiver = "Updated Receiver",
        Edrpou = "87654321",
        Iban = "123456789012345678901234567",
        PaymentPurpose = "Updated purpose"
    };

    private readonly Entities.UahBankDetails _uahBankDetailsEntity = new()
    {
        Id = 1,
        Name = "Bank Name",
        Receiver = "Receiver Name",
        Edrpou = "12345678",
        Iban = "123456789012345678901234567",
        PaymentPurpose = "Old purpose"
    };

    private readonly UahBankDetailsDto _uahBankDetailsDto = new()
    {
        Id = 1,
        Name = "Updated Bank",
        Receiver = "Updated Receiver",
        Edrpou = "87654321",
        Iban = "123456789012345678901234567",
        PaymentPurpose = "Updated purpose"
    };

    public UpdateUahBankDetailsTests()
    {
        _mockMapper = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new UpdateUahBankDetailsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        _uahBankDetailsEntity.Name = name!;
        _uahBankDetailsDto.Name = name!;
        SetupDependencies();

        var handler = new UpdateUahBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        Result<UahBankDetailsDto> result = await handler.Handle(
            new UpdateUahBankDetailsCommand(new UpdateUahBankDetailsDto { Name = name! }, 1),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldFail_EntityNotFound()
    {
        SetupDependencies(entityExists: false);
        var handler = new UpdateUahBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        Result<UahBankDetailsDto> result = await handler.Handle(
            new UpdateUahBankDetailsCommand(_updateDto, 99),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(99, typeof(Entities.UahBankDetails)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        SetupDependencies(saveResult: -1);
        var handler = new UpdateUahBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        Result<UahBankDetailsDto> result = await handler.Handle(
            new UpdateUahBankDetailsCommand(_updateDto, 1),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Entities.UahBankDetails)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUahBankDetails()
    {
        SetupDependencies();
        var handler = new UpdateUahBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        Result<UahBankDetailsDto> result = await handler.Handle(
            new UpdateUahBankDetailsCommand(_updateDto, 1),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_uahBankDetailsDto.Name, result.Value.Name);
        Assert.Equal(_uahBankDetailsDto.Iban, result.Value.Iban);
    }

    private void SetupDependencies(int saveResult = 1, bool entityExists = true)
    {
        SetUpAutomapper(_uahBankDetailsEntity, _uahBankDetailsDto);
        SetUpRepositoryWrapper(saveResult, entityExists);
    }

    private void SetUpAutomapper(Entities.UahBankDetails outputEntity, UahBankDetailsDto outputDto)
    {
        _mockMapper.Setup(m => m.Map<UahBankDetailsDto>(It.IsAny<Entities.UahBankDetails>()))
            .Returns(outputDto);
        _mockMapper.Setup(m => m.Map(
                It.IsAny<UpdateUahBankDetailsDto>(),
                It.IsAny<Entities.UahBankDetails>()))
            .Returns(outputEntity);
    }

    private void SetUpRepositoryWrapper(int saveResult, bool entityExists)
    {
        _repositoryWrapperMock.Setup(repo => repo.UahBankDetailsRepository
            .Update(It.IsAny<Entities.UahBankDetails>()));
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
        _repositoryWrapperMock.Setup(repo => repo.UahBankDetailsRepository
                .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Entities.UahBankDetails>>()))
                .ReturnsAsync(entityExists ? _uahBankDetailsEntity : null);
    }
}
