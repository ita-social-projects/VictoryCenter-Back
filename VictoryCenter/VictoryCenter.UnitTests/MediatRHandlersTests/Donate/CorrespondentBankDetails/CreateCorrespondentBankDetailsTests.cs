using AutoMapper;
using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.BLL.Validators.Donate.CorrespondentBankDetails;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donate.CorrespondentBankDetails;

public class CreateCorrespondentBankDetailsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<CreateCorrespondentBankDetailsCommand> _validator;

    private readonly Entities.CorrespondentBankDetails _correspondentBankDetails = new()
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
        Name = "Correspondent Bank",
        Swift = "CORRSWIFT01",
        Account = "ACC1234567890",
        Iban = "UA123456789012345678901234567",
        ForeignBankDetailsId = 1
    };

    private readonly Entities.ForeignBankDetails _foreignBankDetails = new()
    {
        Id = 1,
        Name = "Foreign Bank",
        Receiver = "Receiver Name",
        Iban = "123456789012345678901234567",
        Swift = "12345678901",
        Address = "Bank Street 123"
    };

    public CreateCorrespondentBankDetailsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validator = new CreateCorrespondentBankDetailsCommandValidator();
    }

    [Fact]
    public async Task Handle_ShouldCreateCorrespondentBankDetails()
    {
        SetupDependencies();
        var handler = new CreateCorrespondentBankDetailsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);

        Result<CorrespondentBankDetailsDto> result = await handler
            .Handle(
                new CreateCorrespondentBankDetailsCommand(new CreateCorrespondentBankDetailsDto
                {
                    Name = "Correspondent Bank",
                    Swift = "12345678901",
                    Account = "ACC1234567890",
                    Iban = "123456789012345678901234567",
                    ForeignBankDetailsId = 1
                }),
                CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_correspondentBankDetailsDto.Name, result.Value.Name);
        Assert.Equal(_correspondentBankDetailsDto.Swift, result.Value.Swift);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_InvalidName(string? name)
    {
        _correspondentBankDetails.Name = name!;
        _correspondentBankDetailsDto.Name = name!;
        SetupDependencies();

        var handler = new CreateCorrespondentBankDetailsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);

        Result<CorrespondentBankDetailsDto> result = await handler
            .Handle(
                new CreateCorrespondentBankDetailsCommand(new CreateCorrespondentBankDetailsDto
                {
                    Name = name!,
                    Swift = "345345",
                    Account = "ACC1234567890",
                    Iban = "34543535",
                    ForeignBankDetailsId = 1
                }),
                CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldFail_ForeignBankDetailsNotFound()
    {
        SetupDependencies(foreignBankDetailsExists: false);
        var handler = new CreateCorrespondentBankDetailsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);

        Result<CorrespondentBankDetailsDto> result = await handler
            .Handle(
                new CreateCorrespondentBankDetailsCommand(new CreateCorrespondentBankDetailsDto
                {
                    Name = "Correspondent Bank",
                    Swift = "12345678901",
                    Account = "ACC1234567890",
                    Iban = "123456789012345678901234567",
                    ForeignBankDetailsId = 999
                }),
                CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(999, typeof(Entities.ForeignBankDetails)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFails()
    {
        SetupDependencies(-1);
        var handler = new CreateCorrespondentBankDetailsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validator);

        Result<CorrespondentBankDetailsDto> result = await handler
            .Handle(
                new CreateCorrespondentBankDetailsCommand(new CreateCorrespondentBankDetailsDto
                {
                    Name = "Correspondent Bank",
                    Swift = "12345678901",
                    Account = "ACC1234567890",
                    Iban = "123456789012345678901234567",
                    ForeignBankDetailsId = 1
                }),
                CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(Entities.CorrespondentBankDetails)), result.Errors[0].Message);
    }

    private void SetupDependencies(int saveResult = 1, bool foreignBankDetailsExists = true)
    {
        SetUpAutomapper(_correspondentBankDetails, _correspondentBankDetailsDto);
        SetupRepositoryWrapper(saveResult, foreignBankDetailsExists);
    }

    private void SetUpAutomapper(Entities.CorrespondentBankDetails outputEntity, CorrespondentBankDetailsDto outputDto)
    {
        _mapperMock.Setup(m => m.Map<Entities.CorrespondentBankDetails>(It.IsAny<CreateCorrespondentBankDetailsDto>()))
            .Returns(outputEntity);
        _mapperMock.Setup(m => m.Map<CorrespondentBankDetailsDto>(It.IsAny<Entities.CorrespondentBankDetails>()))
            .Returns(outputDto);
    }

    private void SetupRepositoryWrapper(int saveResult, bool foreignBankDetailsExists)
    {
        _repositoryWrapperMock.Setup(repo => repo.ForeignBankDetailsRepository
            .GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Entities.ForeignBankDetails>>()))
            .ReturnsAsync(foreignBankDetailsExists ? _foreignBankDetails : null);

        _repositoryWrapperMock.Setup(repo => repo.CorrespondentBankDetailsRepository
            .CreateAsync(It.IsAny<Entities.CorrespondentBankDetails>(), new CancellationToken()));

        _repositoryWrapperMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
