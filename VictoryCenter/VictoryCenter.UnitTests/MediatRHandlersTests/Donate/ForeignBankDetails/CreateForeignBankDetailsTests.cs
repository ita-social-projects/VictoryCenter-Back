using AutoMapper;
using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.BLL.Validators.Donate.ForeignBankDetails;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.ForeignBankDetails;

public class CreateForeignBankDetailsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<CreateForeignBankDetailsCommand> _validator;

    private readonly Entities.ForeignBankDetails _foreignBankDetails = new()
    {
        Id = 1,
        Name = "Foreign Bank",
        Receiver = "Receiver Name",
        Iban = "UA123456789012345678901234567",
        Swift = "12345678901",
        Address = "Bank Street 123"
    };

    private readonly ForeignBankDetailsDto _foreignBankDetailsDto = new()
    {
        Id = 1,
        Name = "Foreign Bank",
        Receiver = "Receiver Name",
        Iban = "UA123456789012345678901234567",
        Swift = "12345678901",
        Address = "Bank Street 123"
    };

    public CreateForeignBankDetailsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new CreateForeignBankDetailsCommandValidator();
    }

    [Fact]
    public async Task Handle_ShouldCreateForeignBankDetails()
    {
        // Arrange
        SetupDependencies();
        var handler = new CreateForeignBankDetailsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);

        // Act
        Result<ForeignBankDetailsDto> result = await handler
            .Handle(
                new CreateForeignBankDetailsCommand(new CreateForeignBankDetailsDto
                {
                    Name = "Foreign Bank",
                    Receiver = "Receiver Name",
                    Iban = "UA123456789012345678901234567",
                    Swift = "12345678901",
                    Address = "Bank Street 123",
                    Currency = BankCurrency.Usd,
                }),
                CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_foreignBankDetailsDto.Name, result.Value.Name);
        Assert.Equal(_foreignBankDetailsDto.Swift, result.Value.Swift);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        // Arrange
        _foreignBankDetails.Name = name!;
        _foreignBankDetailsDto.Name = name!;
        SetupDependencies();

        var handler = new CreateForeignBankDetailsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);

        // Act
        Result<ForeignBankDetailsDto> result = await handler
            .Handle(
                new CreateForeignBankDetailsCommand(new CreateForeignBankDetailsDto
                {
                    Name = name!,
                    Receiver = "Receiver Name",
                    Iban = "UA123456789012345678901234567",
                    Swift = "12345678901",
                    Address = "Bank Street 123"
                }),
                CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        // Arrange
        SetupDependencies(-1);
        var handler = new CreateForeignBankDetailsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);

        // Act
        Result<ForeignBankDetailsDto> result = await handler
            .Handle(
                new CreateForeignBankDetailsCommand(new CreateForeignBankDetailsDto
                {
                    Name = "Foreign Bank",
                    Receiver = "Receiver Name",
                    Iban = "UA123456789012345678901234567",
                    Swift = "12345678901",
                    Address = "Bank Street 123",
                    Currency = BankCurrency.Usd,
                }),
                CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(Entities.ForeignBankDetails)), result.Errors[0].Message);
    }

    private void SetupDependencies(int saveResult = 1)
    {
        SetUpAutomapper(_foreignBankDetails, _foreignBankDetailsDto);
        SetupRepositoryWrapper(saveResult);
    }

    private void SetUpAutomapper(Entities.ForeignBankDetails outputEntity, ForeignBankDetailsDto outputDto)
    {
        _mapperMock.Setup(m => m.Map<Entities.ForeignBankDetails>(It.IsAny<CreateForeignBankDetailsDto>()))
            .Returns(outputEntity);
        _mapperMock.Setup(m => m.Map<ForeignBankDetailsDto>(It.IsAny<Entities.ForeignBankDetails>()))
            .Returns(outputDto);
    }

    private void SetupRepositoryWrapper(int saveResult)
    {
        _repositoryWrapperMock.Setup(repo => repo.ForeignBankDetailsRepository
            .CreateAsync(It.IsAny<Entities.ForeignBankDetails>()));
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
