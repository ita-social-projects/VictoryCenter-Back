using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;
using VictoryCenter.BLL.Queries.Public.CompanyProfile.Get;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.CompanyProfile;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.CompanyProfile;

public class GetCompanyProfileHandlerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<ICompanyProfileRepository> _companyProfileRepositoryMock = new();

    private readonly DAL.Entities.CompanyProfile _entity = new()
    {
        Id = 1,
        Contact = new CompanyProfileContact
        {
            Id = 10,
            Phone = "+380501234567",
            Address = "Kyiv, Str 1",
            Email = "test@test.com",
            CorrespondenceEmail = "corr@test.com",
            Motto = "Motto"
        },
        Requisite = new CompanyProfileRequisite
        {
            Id = 20,
            Recipient = "Recipient",
            Edrpou = "12345678",
            Address = "Requisite Address"
        },
        SocialLinks = []
    };

    private readonly CompanyProfileDto _dto = new()
    {
        Contacts = new CompanyProfileContactsDto
        {
            Phone = "+380501234567",
            Address = "Kyiv, Str 1",
            Email = "test@test.com",
            CorrespondenceEmail = "corr@test.com",
            Motto = "Motto"
        },
        Requisites = new CompanyProfileRequisiteDto
        {
            Recipient = "Recipient",
            Edrpou = "12345678",
            Address = "Requisite Address"
        },
        SocialLinks = []
    };

    public GetCompanyProfileHandlerTests()
    {
        _repositoryWrapperMock
            .SetupGet(x => x.CompanyProfileRepository)
            .Returns(_companyProfileRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProfile_WhenProfileExists()
    {
        // Arrange
        _companyProfileRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.CompanyProfile>?>()))
            .ReturnsAsync(_entity);

        _mapperMock
            .Setup(x => x.Map<CompanyProfileDto>(It.IsAny<DAL.Entities.CompanyProfile>()))
            .Returns(_dto);

        var handler = new GetCompanyProfileHandler(_repositoryWrapperMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(new GetCompanyProfileQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_dto.Contacts.Phone, result.Value.Contacts.Phone);
        Assert.Equal(_dto.Requisites.Recipient, result.Value.Requisites.Recipient);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyDto_WhenProfileNotFound()
    {
        // Arrange
        _companyProfileRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<DAL.Entities.CompanyProfile>?>()))
            .ReturnsAsync((DAL.Entities.CompanyProfile?)null);

        var handler = new GetCompanyProfileHandler(_repositoryWrapperMock.Object, _mapperMock.Object);

        var result = await handler.Handle(new GetCompanyProfileQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.NotNull(result.Value.Contacts);
        Assert.Equal(string.Empty, result.Value.Contacts.Phone);
        Assert.Equal(string.Empty, result.Value.Contacts.Address);
        Assert.Equal(string.Empty, result.Value.Contacts.Email);
        Assert.Equal(string.Empty, result.Value.Contacts.CorrespondenceEmail);
        Assert.Equal(string.Empty, result.Value.Contacts.Motto);
        Assert.Empty(result.Value.Contacts.Localizations);

        Assert.NotNull(result.Value.Requisites);
        Assert.Equal(string.Empty, result.Value.Requisites.Recipient);
        Assert.Equal(string.Empty, result.Value.Requisites.Edrpou);
        Assert.Equal(string.Empty, result.Value.Requisites.Address);
        Assert.Empty(result.Value.Requisites.Localizations);

        Assert.Empty(result.Value.SocialLinks);
    }
}
