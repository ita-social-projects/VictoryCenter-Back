using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.BLL.Validators.Donate.CorrespondentBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.CorrespondentBankDetails;

public class UpdateCorrespondentBankDetailsTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<UpdateCorrespondentBankDetailsCommand> _validator;

    private readonly UpdateCorrespondentBankDetailsDto _updateDto = new()
    {
        Name = "Updated Correspondent Bank",
        Swift = "12345678901",
        Account = "UPDACC123456",
        Iban = "123456789012345678901234567"
    };

    private readonly Entities.CorrespondentBankDetails _correspondentBankDetailsEntity = new()
    {
        Id = 1,
        Name = "Correspondent Bank",
        Swift = "CORRSWIFT01",
        Account = "ACC1234567890",
        Iban = "UA123456789012345678901234567",
        ForeignBankDetailsId = 1
    };

    private readonly CorrespondentBankDetailsDto _correspondentBankDetailsDto = new()
    {
        Id = 1,
        Name = "Updated Correspondent Bank",
        Swift = "UPDSWIFT123",
        Account = "UPDACC123456",
        Iban = "UA987654321098765432109876543",
        ForeignBankDetailsId = 1
    };

    public UpdateCorrespondentBankDetailsTests()
    {
        _mockMapper = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new UpdateCorrespondentBankDetailsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        // Arrange
        _correspondentBankDetailsEntity.Name = name!;
        _correspondentBankDetailsDto.Name = name!;
        SetupDependencies();

        var handler = new UpdateCorrespondentBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        var dto = new UpdateCorrespondentBankDetailsDto
        {
            Name = name!,
            Swift = _updateDto.Swift,
            Account = _updateDto.Account,
            Iban = _updateDto.Iban
        };

        // Act
        Result<CorrespondentBankDetailsDto> result = await handler.Handle(
            new UpdateCorrespondentBankDetailsCommand(dto, 1),
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
        var handler = new UpdateCorrespondentBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        // Act
        Result<CorrespondentBankDetailsDto> result = await handler.Handle(
            new UpdateCorrespondentBankDetailsCommand(_updateDto, 99),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(99, typeof(Entities.CorrespondentBankDetails)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        // Arrange
        SetupDependencies(saveResult: -1);
        var handler = new UpdateCorrespondentBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        // Act
        Result<CorrespondentBankDetailsDto> result = await handler.Handle(
            new UpdateCorrespondentBankDetailsCommand(_updateDto, 1),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(Entities.CorrespondentBankDetails)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        SetupDependencies();
        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = new UpdateCorrespondentBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        // Act
        Result<CorrespondentBankDetailsDto> result = await handler.Handle(
            new UpdateCorrespondentBankDetailsCommand(_updateDto, 1),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(Entities.CorrespondentBankDetails)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateCorrespondentBankDetails()
    {
        // Arrange
        SetupDependencies();
        var handler = new UpdateCorrespondentBankDetailsHandler(_mockMapper.Object, _repositoryWrapperMock.Object, _validator);

        // Act
        Result<CorrespondentBankDetailsDto> result = await handler.Handle(
            new UpdateCorrespondentBankDetailsCommand(_updateDto, 1),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_correspondentBankDetailsDto.Name, result.Value.Name);
        Assert.Equal(_correspondentBankDetailsDto.Swift, result.Value.Swift);
    }

    private void SetupDependencies(int saveResult = 1, bool entityExists = true)
    {
        SetUpAutomapper(_correspondentBankDetailsEntity, _correspondentBankDetailsDto);
        SetUpRepositoryWrapper(saveResult, entityExists);
    }

    private void SetUpAutomapper(Entities.CorrespondentBankDetails outputEntity, CorrespondentBankDetailsDto outputDto)
    {
        _mockMapper.Setup(m => m.Map<CorrespondentBankDetailsDto>(It.IsAny<Entities.CorrespondentBankDetails>()))
            .Returns(outputDto);

        _mockMapper.Setup(m => m.Map(
                It.IsAny<UpdateCorrespondentBankDetailsDto>(),
                It.IsAny<Entities.CorrespondentBankDetails>()))
            .Returns(outputEntity);
    }

    private void SetUpRepositoryWrapper(int saveResult, bool entityExists)
    {
        _repositoryWrapperMock.Setup(repo => repo.CorrespondentBankDetailsRepository
            .Update(It.IsAny<Entities.CorrespondentBankDetails>()));

        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);

        _repositoryWrapperMock.Setup(repo => repo.CorrespondentBankDetailsRepository
                .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Entities.CorrespondentBankDetails>>()))
                .ReturnsAsync(entityExists ? _correspondentBankDetailsEntity : null);
    }
}
