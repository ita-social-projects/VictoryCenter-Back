using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.Localization.FaqQuestions.GetByLanguageId;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.FaqQuestions;

public class GetFaqQuestionLocalizationsByLanguageIdTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly IEnumerable<FaqQuestionLocalization> _localizationsEntities;
    private readonly IEnumerable<FaqQuestionLocalizationDto> _localizationsDtos;

    public GetFaqQuestionLocalizationsByLanguageIdTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();

        var language = new LocalizationLanguage
        {
            Id = 1,
            Code = "en",
            CreatedAt = DateTime.UtcNow
        };

        _localizationsEntities = new List<FaqQuestionLocalization>
        {
            new()
            {
                EntityId = 1,
                LanguageId = 1,
                Language = language,
                QuestionText = "111 Super mega question text that has enough symbols",
                AnswerText = "111 Ultra detailed and long enought answer text, that will definetely pass validation",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                EntityId = 2,
                LanguageId = 1,
                Language = language,
                QuestionText = "222 Super mega question text that has enough symbols",
                AnswerText = "222 Ultra detailed and long enought answer text, that will definetely pass validation",
                CreatedAt = DateTime.UtcNow
            }
        };

        _localizationsDtos = new List<FaqQuestionLocalizationDto>
        {
            new()
            {
                EntityId = 1,
                LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" },
                QuestionText = "111 Super mega question text that has enough symbols",
                AnswerText = "111 Ultra detailed and long enought answer text, that will definetely pass validation",
            },
            new()
            {
                EntityId = 2,
                LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" },
                QuestionText = "222 Super mega question text that has enough symbols",
                AnswerText = "222 Ultra detailed and long enought answer text, that will definetely pass validation",
            }
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnLocalizations_WhenLanguageIdExists()
    {
        // Arrange
        SetupRepositoryWrapper(_localizationsEntities);
        SetupMapper(_localizationsDtos);
        var handler = new GetByLanguageIdHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);
        const string expectedQuestionText1 = "111 Super mega question text that has enough symbols";
        const string expectedQuestionText2 = "222 Super mega question text that has enough symbols";

        // Act
        var result = await handler.Handle(new GetByLanguageIdQuery(1), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
        Assert.Collection(
            result.Value,
            first => Assert.Equal(expectedQuestionText1, first.QuestionText),
            second => Assert.Equal(expectedQuestionText2, second.QuestionText));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoLocalizationsFound()
    {
        // Arrange
        SetupRepositoryWrapper(new List<FaqQuestionLocalization>());
        SetupMapper(new List<FaqQuestionLocalizationDto>());
        var handler = new GetByLanguageIdHandler(_mockMapper.Object, _mockRepositoryWrapper.Object);

        // Act
        var result = await handler.Handle(new GetByLanguageIdQuery(99), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private void SetupRepositoryWrapper(IEnumerable<FaqQuestionLocalization> entitiesToReturn)
    {
        _mockRepositoryWrapper.Setup(repo =>
            repo.FaqQuestionLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<FaqQuestionLocalization>>()))
            .ReturnsAsync(entitiesToReturn);
    }

    private void SetupMapper(IEnumerable<FaqQuestionLocalizationDto> dtosToReturn)
    {
        _mockMapper.Setup(mapper =>
            mapper.Map<List<FaqQuestionLocalizationDto>>(It.IsAny<IEnumerable<FaqQuestionLocalization>>()))
            .Returns(dtosToReturn.ToList());
    }
}
