using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Errors;
using VictoryCenter.BLL.Queries.Admin.Localization.MainPage.GetByLanguageId;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Models;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.MainPage;

public class GetMainPageLocalizationByLanguageIdHandlerTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Handle_MapsReadRowWithoutAdditionalRepositoryCalls(bool includeBlocks)
    {
        using var cancellation = new CancellationTokenSource();
        var row = new MainPageTranslationRow
        {
            EntityId = 10, LanguageId = 2, LanguageCode = "en", Title = null, Description = "Translation", TranslationStatus = TranslationStatus.Outdated
        };
        var wrapper = new Mock<IRepositoryWrapper>(MockBehavior.Strict);
        wrapper.Setup(w => w.MainPageRepository.GetLocalizationAsync(1, 2, cancellation.Token))
            .ReturnsAsync(new MainPageLocalizationRow
            {
                Translation = row, AboutUs = includeBlocks ? row : null,
                Partners = includeBlocks ? row : null, Donations = includeBlocks ? row : null
            });

        var result = await new GetMainPageLocalizationByLanguageIdHandler(wrapper.Object)
            .Handle(new GetMainPageLocalizationByLanguageIdQuery(1, 2), cancellation.Token);

        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value.EntityId);
        Assert.Null(result.Value.Title);
        Assert.Equal("Translation", result.Value.Description);
        Assert.Equal(TranslationStatus.Outdated, result.Value.TranslationStatus);
        Assert.Equal(2, result.Value.LocalizationInfoDto.Id);
        Assert.Equal("en", result.Value.LocalizationInfoDto.Code);
        if (includeBlocks)
        {
            Assert.Equal(row.EntityId, result.Value.MainAboutUs!.EntityId);
            Assert.Equal(row.Description, result.Value.MainPartners!.Description);
            Assert.Equal(row.TranslationStatus, result.Value.MainDonations!.TranslationStatus);
        }
        else
        {
            Assert.Null(result.Value.MainAboutUs);
            Assert.Null(result.Value.MainPartners);
            Assert.Null(result.Value.MainDonations);
        }

        wrapper.Verify(w => w.MainPageRepository.GetLocalizationAsync(1, 2, cancellation.Token), Times.Once);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Handle_DistinguishesMissingPageFromMissingTranslation(bool pageExists)
    {
        var wrapper = new Mock<IRepositoryWrapper>();
        wrapper.Setup(w => w.MainPageRepository.GetLocalizationAsync(1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pageExists ? new MainPageLocalizationRow() : null);

        var result = await new GetMainPageLocalizationByLanguageIdHandler(wrapper.Object)
            .Handle(new GetMainPageLocalizationByLanguageIdQuery(1, 2), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.IsType<NotFoundError>(Assert.Single(result.Errors));
        Assert.Equal(
            pageExists
            ? ErrorMessagesConstants.NotFound((1L, 2L), typeof(MainPageLocalization))
            : ErrorMessagesConstants.NotFound(1L, typeof(DAL.Entities.MainPage)), result.Errors[0].Message);
    }
}
