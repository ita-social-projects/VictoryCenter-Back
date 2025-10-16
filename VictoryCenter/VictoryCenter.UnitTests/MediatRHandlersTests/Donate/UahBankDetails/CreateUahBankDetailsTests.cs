using AutoMapper;
using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.BLL.Validators.Donate.UahBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.UahBankDetails;
public class CreateUahBankDetailsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<CreateUahBankDetailsCommand> _validatorMock;

    private readonly Entities.UahBankDetails _uahBankDetails = new()
    {
        Id = 1,
        Name = "Bank Name",
        Receiver = "Receiver Name",
        Edrpou = "12345678",
        Iban = "UA123456789012345678901234567",
        PaymentPurpose = "Payment for services"
    };

    private readonly UahBankDetailsDto _uahBankDetailsDto = new()
    {
        Id = 1,
        Name = "Bank Name",
        Receiver = "Receiver Name",
        Edrpou = "12345678",
        Iban = "UA123456789012345678901234567",
        PaymentPurpose = "Payment for services"
    };

    public CreateUahBankDetailsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new CreateUahBankDetailsCommandValidator();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldCreateUahBankDetails()
    {
        SetupDependencies();
        var handler = new CreateUahBankDetailsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock);

        Result<UahBankDetailsDto> result = await handler
            .Handle(
            new CreateUahBankDetailsCommand(new CreateUahBankDetailsDto
            {
                Name = "Bank Name",
                Receiver = "Receiver Name",
                Edrpou = "12345678",
                Iban = "123456789012345678901234567",
                PaymentPurpose = "Payment for services"
            }), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_uahBankDetailsDto.Name, result.Value.Name);
        Assert.Equal(_uahBankDetailsDto.Iban, result.Value.Iban);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        _uahBankDetails.Name = name!;
        _uahBankDetailsDto.Name = name!;
        SetupDependencies();

        var handler = new CreateUahBankDetailsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock);

        Result<UahBankDetailsDto> result = await handler
            .Handle(
            new CreateUahBankDetailsCommand(new CreateUahBankDetailsDto
            {
                Name = name!,
                Receiver = "Receiver",
                Edrpou = "12345678",
                Iban = "UA12345678901234567890123456712",
                PaymentPurpose = "Purpose"
            }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        SetupDependencies(-1);
        var handler = new CreateUahBankDetailsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock);

        Result<UahBankDetailsDto> result = await handler
            .Handle(
            new CreateUahBankDetailsCommand(new CreateUahBankDetailsDto
            {
                Name = "Bank Name",
                Receiver = "Receiver",
                Edrpou = "12345678",
                Iban = "123456789012345678901234567",
                PaymentPurpose = "Purpose"
            }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(Entities.UahBankDetails)), result.Errors[0].Message);
    }

    private void SetupDependencies(int saveResult = 1)
    {
        SetUpAutomapper(_uahBankDetails, _uahBankDetailsDto);
        SetupRepositoryWrapper(saveResult);
    }

    private void SetUpAutomapper(Entities.UahBankDetails outputEntity, UahBankDetailsDto outputDto)
    {
        _mapperMock.Setup(m => m.Map<Entities.UahBankDetails>(It.IsAny<CreateUahBankDetailsDto>()))
            .Returns(outputEntity);
        _mapperMock.Setup(m => m.Map<UahBankDetailsDto>(It.IsAny<Entities.UahBankDetails>()))
            .Returns(outputDto);
    }

    private void SetupRepositoryWrapper(int saveResult)
    {
        _repositoryWrapperMock.Setup(repo => repo.UahBankDetailsRepository
            .CreateAsync(It.IsAny<Entities.UahBankDetails>()));
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
