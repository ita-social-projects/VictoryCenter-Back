using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.CompanyProfile.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;
using VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;
using VictoryCenter.BLL.DTOs.Admin.SocialLinks;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.CompanyProfile.Commands;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.CompanyProfile;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.CompanyProfile;

public class CreateCompanyProfileHandlerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<ICompanyProfileRepository> _companyProfileRepositoryMock = new();
    private readonly Mock<ILocalizationService<CompanyProfileContact, CompanyProfileContactLocalization>> _localizationContactServiceMock = new();
    private readonly Mock<ILocalizationService<CompanyProfileRequisite, CompanyProfileRequisiteLocalization>> _localizationRequisiteServiceMock = new();
    private readonly IValidator<CreateCompanyProfileCommand> _validator = new CreateCompanyProfileCommandValidator();

    private readonly DAL.Entities.CompanyProfile _entity = new()
    {
        Id = 1,
        Contact = new CompanyProfileContact { Id = 10, Phone = "+380501234567", Address = "Kyiv, Str 1", Email = "test@test.com", CorrespondenceEmail = "corr@test.com", Motto = "Motto" },
        Requisite = new CompanyProfileRequisite { Id = 20, Recipient = "Recipient", Edrpou = "12345678", Address = "Requisite Address" },
        SocialLinks = []
    };

    private readonly CompanyProfileDto _dto = new()
    {
        Contacts = new CompanyProfileContactsDto { Phone = "+380501234567" },
        Requisites = new CompanyProfileRequisiteDto { Recipient = "Recipient" },
        SocialLinks = []
    };

    public CreateCompanyProfileHandlerTests()
    {
        _repositoryWrapperMock
            .SetupGet(x => x.CompanyProfileRepository)
            .Returns(_companyProfileRepositoryMock.Object);

        _repositoryWrapperMock
            .Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }

    [Fact]
    public async Task Handle_ShouldCreateProfile_WhenRequestIsValid()
    {
        // Arrange
        SetupSuccessfulCreate();
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateCompanyProfileCommand(GetValidCreateDto()),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _companyProfileRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<DAL.Entities.CompanyProfile>()), Times.Once);
        _localizationContactServiceMock.Verify(
            x => x.TrackEntityLocalizationAsync(It.IsAny<CompanyProfileContactLocalization>()), Times.Once);
        _localizationRequisiteServiceMock.Verify(
            x => x.TrackEntityLocalizationAsync(It.IsAny<CompanyProfileRequisiteLocalization>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = new CreateCompanyProfileDto
        {
            Contacts = null!,
            Requisites = null!,
            SocialLinks = []
        };
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateCompanyProfileCommand(invalidDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenProfileAlreadyExists()
    {
        // Arrange
        _companyProfileRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.CompanyProfile>?>()))
            .ReturnsAsync(_entity);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateCompanyProfileCommand(GetValidCreateDto()),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.OnlyOneEntityOfTypeIsAllowed(nameof(DAL.Entities.CompanyProfile)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenFirstSaveChangesFails()
    {
        // Arrange
        _companyProfileRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.CompanyProfile>?>()))
            .ReturnsAsync((DAL.Entities.CompanyProfile?)null);

        _companyProfileRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<DAL.Entities.CompanyProfile>()))
            .ReturnsAsync(_entity);

        _mapperMock
            .Setup(x => x.Map<DAL.Entities.CompanyProfile>(It.IsAny<CreateCompanyProfileDto>()))
            .Returns(_entity);

        _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateCompanyProfileCommand(GetValidCreateDto()),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(DAL.Entities.CompanyProfile)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        _companyProfileRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.CompanyProfile>?>()))
            .ReturnsAsync((DAL.Entities.CompanyProfile?)null);

        _companyProfileRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<DAL.Entities.CompanyProfile>()))
            .ReturnsAsync(_entity);

        _mapperMock
            .Setup(x => x.Map<DAL.Entities.CompanyProfile>(It.IsAny<CreateCompanyProfileDto>()))
            .Returns(_entity);

        _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateCompanyProfileCommand(GetValidCreateDto()),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(DAL.Entities.CompanyProfile)),
            result.Errors[0].Message);
    }

    private void SetupSuccessfulCreate()
    {
        _companyProfileRepositoryMock
            .SetupSequence(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.CompanyProfile>?>()))
            .ReturnsAsync((DAL.Entities.CompanyProfile?)null)
            .ReturnsAsync(_entity);

        _companyProfileRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<DAL.Entities.CompanyProfile>()))
            .ReturnsAsync(_entity);

        _mapperMock
            .Setup(x => x.Map<DAL.Entities.CompanyProfile>(It.IsAny<CreateCompanyProfileDto>()))
            .Returns(_entity);

        _mapperMock
            .Setup(x => x.Map<CompanyProfileContactLocalization>(It.IsAny<CreateCompanyProfileContactLocalizationDto>()))
            .Returns(new CompanyProfileContactLocalization { LanguageId = 1 });

        _mapperMock
            .Setup(x => x.Map<CompanyProfileRequisiteLocalization>(It.IsAny<CreateCompanyProfileRequisiteLocalizationDto>()))
            .Returns(new CompanyProfileRequisiteLocalization { LanguageId = 1 });

        _mapperMock
            .Setup(x => x.Map<CompanyProfileDto>(It.IsAny<DAL.Entities.CompanyProfile>()))
            .Returns(_dto);

        _localizationContactServiceMock
            .Setup(x => x.TrackEntityLocalizationAsync(It.IsAny<CompanyProfileContactLocalization>()))
            .ReturnsAsync(new CompanyProfileContactLocalization());

        _localizationRequisiteServiceMock
            .Setup(x => x.TrackEntityLocalizationAsync(It.IsAny<CompanyProfileRequisiteLocalization>()))
            .ReturnsAsync(new CompanyProfileRequisiteLocalization());

        _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
    }

    private CreateCompanyProfileHandler CreateHandler() => new(
        _repositoryWrapperMock.Object,
        _mapperMock.Object,
        _validator,
        _localizationContactServiceMock.Object,
        _localizationRequisiteServiceMock.Object);

    private static CreateCompanyProfileDto GetValidCreateDto() => new()
    {
        Contacts = new CreateCompanyProfileContactsDto
        {
            Phone = "+380501234567",
            Address = "Kyiv, Str 1",
            Email = "test@test.com",
            CorrespondenceEmail = "corr@test.com",
            Motto = "Motto",
            Localization =
            [
                new CreateCompanyProfileContactLocalizationDto { EntityId = 0, LanguageId = 1, Address = "Addr UA" }
            ]
        },
        Requisites = new CreateCompanyProfileRequisiteDto
        {
            Recipient = "Recipient",
            Edrpou = "12345678",
            Address = "Requisite Address",
            Localization =
            [
                new CreateCompanyProfileRequisiteLocalizationDto { EntityId = 0, LanguageId = 1, Recipient = "Recipient UA" }
            ]
        },
        SocialLinks =
        [
            new CreateSocialLinkDto { SocialPlatform = SocialPlatform.Facebook, Url = "https://facebook.com/test" }
        ]
    };
}
