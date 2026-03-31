using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.CompanyProfile.Update;
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

public class UpdateCompanyProfileHandlerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<ICompanyProfileRepository> _companyProfileRepositoryMock = new();
    private readonly Mock<ILocalizationService<CompanyProfileContact, CompanyProfileContactLocalization>> _localizationContactServiceMock = new();
    private readonly Mock<ILocalizationService<CompanyProfileRequisite, CompanyProfileRequisiteLocalization>> _localizationRequisiteServiceMock = new();
    private readonly IValidator<UpdateCompanyProfileCommand> _validator = new UpdateCompanyProfileCommandValidator();

    private readonly CompanyProfileDto _dto = new()
    {
        Contacts = new CompanyProfileContactsDto { Phone = "+380501234567" },
        Requisites = new CompanyProfileRequisiteDto { Recipient = "Recipient" },
        SocialLinks = []
    };

    public UpdateCompanyProfileHandlerTests()
    {
        _repositoryWrapperMock
            .SetupGet(x => x.CompanyProfileRepository)
            .Returns(_companyProfileRepositoryMock.Object);

        _repositoryWrapperMock
            .Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProfile_WhenExistingLocalizationsAndLinksAreUpdated()
    {
        // Arrange
        var existingLocalization = new CompanyProfileContactLocalization { EntityId = 10, LanguageId = 1 };
        var existingRequisiteLocalization = new CompanyProfileRequisiteLocalization { EntityId = 20, LanguageId = 1 };
        var existingLink = new CompanyProfileSocialLink { Id = 5, SocialPlatform = SocialPlatform.Facebook, Url = "https://old.com" };

        var entity = BuildEntity(
            contactLocalizations: [existingLocalization],
            requisiteLocalizations: [existingRequisiteLocalization],
            socialLinks: [existingLink]);

        SetupRepositorySequence(entity);

        _mapperMock
            .Setup(x => x.Map<CompanyProfileDto>(It.IsAny<DAL.Entities.CompanyProfile>()))
            .Returns(_dto);

        var request = new UpdateCompanyProfileCommand(BuildValidUpdateDto(
            languageId: 1,
            platform: SocialPlatform.Facebook));

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mapperMock.Verify(
            x => x.Map(It.IsAny<UpdateCompanyProfileContactLocalizationDto>(), It.IsAny<CompanyProfileContactLocalization>()),
            Times.Once);
        _mapperMock.Verify(
            x => x.Map(It.IsAny<UpdateCompanyProfileRequisiteLocalizationDto>(), It.IsAny<CompanyProfileRequisiteLocalization>()),
            Times.Once);
        _mapperMock.Verify(
            x => x.Map(It.IsAny<UpdateSocialLinkDto>(), It.IsAny<CompanyProfileSocialLink>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProfile_WhenNewLocalizationsAndLinksAreAdded()
    {
        // Arrange
        var entity = BuildEntity(
            contactLocalizations: [],
            requisiteLocalizations: [],
            socialLinks: []);

        SetupRepositorySequence(entity);

        _mapperMock
            .Setup(x => x.Map<CompanyProfileSocialLink>(It.IsAny<UpdateSocialLinkDto>()))
            .Returns(new CompanyProfileSocialLink { SocialPlatform = SocialPlatform.Facebook, Url = "https://facebook.com/new" });

        _mapperMock
            .Setup(x => x.Map<CompanyProfileContactLocalization>(It.IsAny<UpdateCompanyProfileContactLocalizationDto>()))
            .Returns(new CompanyProfileContactLocalization { LanguageId = 1 });

        _mapperMock
            .Setup(x => x.Map<CompanyProfileRequisiteLocalization>(It.IsAny<UpdateCompanyProfileRequisiteLocalizationDto>()))
            .Returns(new CompanyProfileRequisiteLocalization { LanguageId = 1 });

        _mapperMock
            .Setup(x => x.Map<CompanyProfileDto>(It.IsAny<DAL.Entities.CompanyProfile>()))
            .Returns(_dto);

        _localizationContactServiceMock
            .Setup(x => x.TrackEntityLocalizationForUpdateAsync(It.IsAny<CompanyProfileContactLocalization>()))
            .Returns(Task.CompletedTask);

        _localizationRequisiteServiceMock
            .Setup(x => x.TrackEntityLocalizationForUpdateAsync(It.IsAny<CompanyProfileRequisiteLocalization>()))
            .Returns(Task.CompletedTask);

        var request = new UpdateCompanyProfileCommand(BuildValidUpdateDto(
            languageId: 1,
            platform: SocialPlatform.Facebook));

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _localizationContactServiceMock.Verify(
            x => x.TrackEntityLocalizationForUpdateAsync(It.IsAny<CompanyProfileContactLocalization>()),
            Times.Once);
        _localizationRequisiteServiceMock.Verify(
            x => x.TrackEntityLocalizationForUpdateAsync(It.IsAny<CompanyProfileRequisiteLocalization>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = new UpdateCompanyProfileDto
        {
            Contacts = null!,
            Requisites = null!,
            SocialLinks = []
        };
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UpdateCompanyProfileCommand(invalidDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenProfileNotFound()
    {
        // Arrange
        _companyProfileRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.CompanyProfile>?>()))
            .ReturnsAsync((DAL.Entities.CompanyProfile?)null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UpdateCompanyProfileCommand(BuildValidUpdateDto()),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        var entity = BuildEntity([], [], []);
        SetupRepositorySequence(entity);

        _mapperMock
            .Setup(x => x.Map<CompanyProfileSocialLink>(It.IsAny<UpdateSocialLinkDto>()))
            .Returns(new CompanyProfileSocialLink { SocialPlatform = SocialPlatform.Instagram, Url = "https://instagram.com/test" });

        _mapperMock
            .Setup(x => x.Map<CompanyProfileContactLocalization>(It.IsAny<UpdateCompanyProfileContactLocalizationDto>()))
            .Returns(new CompanyProfileContactLocalization { LanguageId = 1 });

        _mapperMock
            .Setup(x => x.Map<CompanyProfileRequisiteLocalization>(It.IsAny<UpdateCompanyProfileRequisiteLocalizationDto>()))
            .Returns(new CompanyProfileRequisiteLocalization { LanguageId = 1 });

        _localizationContactServiceMock
            .Setup(x => x.TrackEntityLocalizationForUpdateAsync(It.IsAny<CompanyProfileContactLocalization>()))
            .Returns(Task.CompletedTask);

        _localizationRequisiteServiceMock
            .Setup(x => x.TrackEntityLocalizationForUpdateAsync(It.IsAny<CompanyProfileRequisiteLocalization>()))
            .Returns(Task.CompletedTask);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UpdateCompanyProfileCommand(BuildValidUpdateDto()),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(DAL.Entities.CompanyProfile)),
            result.Errors[0].Message);
    }

    private void SetupRepositorySequence(DAL.Entities.CompanyProfile entity)
    {
        _companyProfileRepositoryMock
            .SetupSequence(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.CompanyProfile>?>()))
            .ReturnsAsync(entity)
            .ReturnsAsync(entity);
    }

    private UpdateCompanyProfileHandler CreateHandler() => new(
        _repositoryWrapperMock.Object,
        _mapperMock.Object,
        _localizationContactServiceMock.Object,
        _localizationRequisiteServiceMock.Object,
        _validator);

    private static DAL.Entities.CompanyProfile BuildEntity(
        ICollection<CompanyProfileContactLocalization> contactLocalizations,
        ICollection<CompanyProfileRequisiteLocalization> requisiteLocalizations,
        ICollection<CompanyProfileSocialLink> socialLinks) => new()
    {
        Id = 1,
        Contact = new CompanyProfileContact
        {
            Id = 10,
            Phone = "+380501234567",
            Address = "Kyiv, Str 1",
            Email = "test@test.com",
            CorrespondenceEmail = "corr@test.com",
            Motto = "Old Motto",
            Localizations = contactLocalizations
        },
        Requisite = new CompanyProfileRequisite
        {
            Id = 20,
            Recipient = "Old Recipient",
            Edrpou = "12345678",
            Address = "Old Address",
            Localizations = requisiteLocalizations
        },
        SocialLinks = socialLinks
    };

    private static UpdateCompanyProfileDto BuildValidUpdateDto(
        long languageId = 1,
        SocialPlatform platform = SocialPlatform.Instagram) => new()
    {
        Contacts = new UpdateCompanyProfileContactDto
        {
            Phone = "+380501234567",
            Address = "Kyiv, Str 1",
            Email = "test@test.com",
            CorrespondenceEmail = "corr@test.com",
            Motto = "New Motto",
            Localizations =
            [
                new UpdateCompanyProfileContactLocalizationDto { LanguageId = languageId, Address = "New Addr" }
            ]
        },
        Requisites = new UpdateCompanyProfileRequisiteDto
        {
            Recipient = "New Recipient",
            Edrpou = "12345678",
            Address = "New Address",
            Localizations =
            [
                new UpdateCompanyProfileRequisiteLocalizationDto { LanguageId = languageId, Recipient = "New Recipient UA" }
            ]
        },
        SocialLinks =
        [
            new UpdateSocialLinkDto { SocialPlatform = platform, Url = "https://instagram.com/test" }
        ]
    };
}
