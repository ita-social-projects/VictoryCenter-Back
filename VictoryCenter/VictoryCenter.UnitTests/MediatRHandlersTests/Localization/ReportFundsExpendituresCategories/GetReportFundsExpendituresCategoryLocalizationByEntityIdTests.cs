using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.ReportFundsExpendituresCategories.GetByEntityId;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.ReportFundsExpendituresCategories;

public class GetReportFundsExpendituresCategoryLocalizationByEntityIdTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly GetReportFundsExpendituresCategoryLocalizationByEntityIdHandler _handler;

    private readonly List<ReportFundsExpendituresCategoryLocalization> _entities = new()
    {
        new ReportFundsExpendituresCategoryLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "English Name",
            Language = new LocalizationLanguage { Id = 2, Code = "en", CreatedAt = DateTimeOffset.UtcNow },
            CreatedAt = DateTimeOffset.UtcNow
        },
        new ReportFundsExpendituresCategoryLocalization
        {
            EntityId = 1,
            LanguageId = 3,
            Name = "German Name",
            Language = new LocalizationLanguage { Id = 3, Code = "de", CreatedAt = DateTimeOffset.UtcNow },
            CreatedAt = DateTimeOffset.UtcNow
        }
    };

    private readonly List<ReportFundsExpendituresCategoryLocalizationDto> _dtos = new()
    {
        new ReportFundsExpendituresCategoryLocalizationDto
        {
            EntityId = 1,
            LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
            Name = "English Name"
        },
        new ReportFundsExpendituresCategoryLocalizationDto
        {
            EntityId = 1,
            LocalizationInfoDto = new LocalizationInfoDto { Id = 3, Code = "de" },
            Name = "German Name"
        }
    };

    public GetReportFundsExpendituresCategoryLocalizationByEntityIdTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _handler = new GetReportFundsExpendituresCategoryLocalizationByEntityIdHandler(
            _mockMapper.Object, _mockRepositoryWrapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizations_WhenEntityIdExists()
    {
        SetupRepositoryWrapper(_entities);
        SetupMapper(_dtos);

        var query = new GetReportFundsExpendituresCategoryLocalizationByEntityIdQuery(1);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenEntityIdDoesNotExist()
    {
        SetupRepositoryWrapper(new List<ReportFundsExpendituresCategoryLocalization>());
        SetupMapper(new List<ReportFundsExpendituresCategoryLocalizationDto>());

        var query = new GetReportFundsExpendituresCategoryLocalizationByEntityIdQuery(999);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private void SetupRepositoryWrapper(IEnumerable<ReportFundsExpendituresCategoryLocalization> entitiesToReturn)
    {
        _mockRepositoryWrapper
            .Setup(repo => repo.ReportFundsExpendituresCategoryLocalizationsRepository
                .GetAllAsync(It.IsAny<QueryOptions<ReportFundsExpendituresCategoryLocalization>>()))
            .ReturnsAsync(entitiesToReturn);
    }

    private void SetupMapper(IEnumerable<ReportFundsExpendituresCategoryLocalizationDto> dtosToReturn)
    {
        _mockMapper
            .Setup(m => m.Map<List<ReportFundsExpendituresCategoryLocalizationDto>>(
                It.IsAny<IEnumerable<ReportFundsExpendituresCategoryLocalization>>()))
            .Returns(dtosToReturn.ToList());
    }
}
