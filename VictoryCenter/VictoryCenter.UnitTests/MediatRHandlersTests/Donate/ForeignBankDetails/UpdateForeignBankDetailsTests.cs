using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.BLL.Validators.Donate.ForeignBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.ForeignBankDetails;

public class UpdateForeignBankDetailsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<UpdateForeignBankDetailsCommand> _validator;

    private readonly UpdateForeignBankDetailsDto _updateDto = new()
    {
        Name = "Updated Foreign Bank",
        Receiver = "Updated Receiver",
        Iban = "UA123456789012345678901234567",
        Swift = "12345678901",
        Address = "Updated Address",
    };

    private readonly Entities.ForeignBankDetails _foreignBankDetailsEntity = new()
    {
        Id = 1,
        Name = "Foreign Bank",
        Receiver = "Receiver Name",
        Iban = "UA123456789012345678901234567",
        Swift = "12345678901",
        Address = "Old Address",
        CorrespondentBanks = []
    };

    private readonly ForeignBankDetailsDto _foreignBankDetailsDto = new()
    {
        Id = 1,
        Name = "Updated Foreign Bank",
        Receiver = "Updated Receiver",
        Iban = "UA123456789012345678901234567",
        Swift = "12345678901",
        Address = "Updated Address",
        CorrespondentBanks = []
    };

    public UpdateForeignBankDetailsTests()
    {
        _mockMapper = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new UpdateForeignBankDetailsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        // Arrange
        _foreignBankDetailsEntity.Name = name!;
        _foreignBankDetailsDto.Name = name!;
        SetupDependencies();

        var handler = new UpdateForeignBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        var dtoWithInvalidName = new UpdateForeignBankDetailsDto
        {
            Name = name!,
            Receiver = _updateDto.Receiver,
            Iban = _updateDto.Iban,
            Swift = _updateDto.Swift,
            Address = _updateDto.Address,
        };

        // Act
        Result<ForeignBankDetailsDto> result = await handler.Handle(
            new UpdateForeignBankDetailsCommand(dtoWithInvalidName, 1),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldFail_EntityNotFound()
    {
        // Arrange
        SetupDependencies(entityExists: false);
        var handler = new UpdateForeignBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        // Act
        Result<ForeignBankDetailsDto> result = await handler.Handle(
            new UpdateForeignBankDetailsCommand(_updateDto, 99),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(99, typeof(Entities.ForeignBankDetails)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        // Arrange
        SetupDependencies(saveResult: -1);
        var handler = new UpdateForeignBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        // Act
        Result<ForeignBankDetailsDto> result = await handler.Handle(
            new UpdateForeignBankDetailsCommand(_updateDto, 1),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Entities.ForeignBankDetails)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        SetupDependencies();
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = new UpdateForeignBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        // Act
        Result<ForeignBankDetailsDto> result = await handler.Handle(
            new UpdateForeignBankDetailsCommand(_updateDto, 1),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(Entities.ForeignBankDetails)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateForeignBankDetails()
    {
        // Arrange
        SetupDependencies();
        var handler = new UpdateForeignBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        // Act
        Result<ForeignBankDetailsDto> result = await handler.Handle(
            new UpdateForeignBankDetailsCommand(_updateDto, 1),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_foreignBankDetailsDto.Name, result.Value.Name);
        Assert.Equal(_foreignBankDetailsDto.Iban, result.Value.Iban);
    }

    private void SetupDependencies(int saveResult = 1, bool entityExists = true)
    {
        SetUpAutomapper(_foreignBankDetailsEntity, _foreignBankDetailsDto);
        SetUpRepositoryWrapper(saveResult, entityExists);
    }

    private void SetUpAutomapper(Entities.ForeignBankDetails outputEntity, ForeignBankDetailsDto outputDto)
    {
        _mockMapper.Setup(m => m.Map<ForeignBankDetailsDto>(It.IsAny<Entities.ForeignBankDetails>()))
            .Returns(outputDto);

        _mockMapper.Setup(m => m.Map(
                It.IsAny<UpdateForeignBankDetailsDto>(),
                It.IsAny<Entities.ForeignBankDetails>()))
            .Returns(outputEntity);
    }

    private void SetUpRepositoryWrapper(int saveResult, bool entityExists)
    {
        _repositoryWrapperMock.Setup(repo => repo.ForeignBankDetailsRepository
            .Update(It.IsAny<Entities.ForeignBankDetails>()));

        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);

        _repositoryWrapperMock.Setup(repo => repo.ForeignBankDetailsRepository
                .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Entities.ForeignBankDetails>>()))
                .ReturnsAsync(entityExists ? _foreignBankDetailsEntity : null);

        _repositoryWrapperMock.Setup(repo => repo.CorrespondentBankDetailsRepository
                .GetAllAsync(It.IsAny<QueryOptions<Entities.CorrespondentBankDetails>>()))
                .ReturnsAsync(new List<Entities.CorrespondentBankDetails>());
    }
}
