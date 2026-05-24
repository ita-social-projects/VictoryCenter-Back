using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.ReportFundsExpendituresSettings.GetByEntityId;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.ReportFundsExpendituresSettings;

public class GetReportFundsExpendituresSettingsLocalizationByEntityIdTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly GetReportFundsExpendituresSettingsLocalizationByEntityIdHandler _handler;

    private readonly List<ReportFundsExpendituresSettingsLocalization> _entities = new()
    {
        new ReportFundsExpendituresSettingsLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            DisclaimerTitle = "English disclaimer",
            Language = new LocalizationLanguage { Id = 2, Code = "en", CreatedAt = DateTimeOffset.UtcNow },
            CreatedAt = DateTimeOffset.UtcNow
        },
        new ReportFundsExpendituresSettingsLocalization
        {
            EntityId = 1,
            LanguageId = 3,
            DisclaimerTitle = "Deutscher Haftungsausschluss",
            Language = new LocalizationLanguage { Id = 3, Code = "de", CreatedAt = DateTimeOffset.UtcNow },
            CreatedAt = DateTimeOffset.UtcNow
        }
    };

    private readonly List<ReportFundsExpendituresSettingsLocalizationDto> _dtos = new()
    {
        new ReportFundsExpendituresSettingsLocalizationDto
        {
            EntityId = 1,
            LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
            DisclaimerTitle = "English disclaimer"
        },
        new ReportFundsExpendituresSettingsLocalizationDto
        {
            EntityId = 1,
            LocalizationInfoDto = new LocalizationInfoDto { Id = 3, Code = "de" },
            DisclaimerTitle = "Deutscher Haftungsausschluss"
        }
    };

    public GetReportFundsExpendituresSettingsLocalizationByEntityIdTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _handler = new GetReportFundsExpendituresSettingsLocalizationByEntityIdHandler(
            _mockMapper.Object, _mockRepositoryWrapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizations_WhenEntityIdExists()
    {
        SetupRepositoryWrapper(_entities);
        SetupMapper(_dtos);

        var query = new GetReportFundsExpendituresSettingsLocalizationByEntityIdQuery(1);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenEntityIdDoesNotExist()
    {
        SetupRepositoryWrapper(new List<ReportFundsExpendituresSettingsLocalization>());
        SetupMapper(new List<ReportFundsExpendituresSettingsLocalizationDto>());

        var query = new GetReportFundsExpendituresSettingsLocalizationByEntityIdQuery(999);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private void SetupRepositoryWrapper(IEnumerable<ReportFundsExpendituresSettingsLocalization> entitiesToReturn)
    {
        _mockRepositoryWrapper
            .Setup(repo => repo.ReportFundsExpendituresSettingsLocalizationsRepository
                .GetAllAsync(It.IsAny<QueryOptions<ReportFundsExpendituresSettingsLocalization>>()))
            .ReturnsAsync(entitiesToReturn);
    }

    private void SetupMapper(IEnumerable<ReportFundsExpendituresSettingsLocalizationDto> dtosToReturn)
    {
        _mockMapper
            .Setup(m => m.Map<List<ReportFundsExpendituresSettingsLocalizationDto>>(
                It.IsAny<IEnumerable<ReportFundsExpendituresSettingsLocalization>>()))
            .Returns(dtosToReturn.ToList());
    }
}
