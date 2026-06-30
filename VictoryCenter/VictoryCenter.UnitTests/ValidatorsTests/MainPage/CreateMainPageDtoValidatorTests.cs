using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainDonations;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.BLL.Validators.MainPage.Dto;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.MainPage;

public class CreateMainPageDtoValidatorTests
{
    private readonly CreateMainPageDtoValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        var result = _validator.TestValidate(GetValidDto());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenTitleIsEmpty(string? title)
    {
        var dto = GetValidDto();
        dto = dto with { Title = title! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainPageDto.Title)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooShort()
    {
        var dto = GetValidDto() with { Title = new string('a', MainPageConstants.ValidationTitleRules.MinLen - 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPageDto.Title), MainPageConstants.ValidationTitleRules.MinLen));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooLong()
    {
        var dto = GetValidDto() with { Title = new string('a', MainPageConstants.ValidationTitleRules.MaxLen + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPageDto.Title), MainPageConstants.ValidationTitleRules.MaxLen));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenDescriptionIsEmpty(string? description)
    {
        var dto = GetValidDto() with { Description = description! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainPageDto.Description)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort()
    {
        var dto = GetValidDto() with { Description = new string('a', MainPageConstants.ValidationDescriptionRules.MinLen - 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPageDto.Description), MainPageConstants.ValidationDescriptionRules.MinLen));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var dto = GetValidDto() with { Description = new string('a', MainPageConstants.ValidationDescriptionRules.MaxLen + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPageDto.Description), MainPageConstants.ValidationDescriptionRules.MaxLen));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainAboutUsIsInvalid()
    {
        var dto = GetValidDto() with
        {
            MainAboutUs = new CreateMainAboutUsDto
            {
                Title = string.Empty,
                Description = "Valid description",
            },
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor("MainAboutUs.Title");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainPartnersIsInvalid()
    {
        var dto = GetValidDto() with
        {
            MainPartners = new CreateMainPartnersDto
            {
                Title = string.Empty,
                Description = "Valid description",
            },
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor("MainPartners.Title");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainDonationsIsInvalid()
    {
        var dto = GetValidDto() with
        {
            MainDonations = new CreateMainDonationsDto
            {
                Title = string.Empty,
                Description = "Valid description",
            },
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor("MainDonations.Title");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenImpactStatisticsIsInvalid()
    {
        var dto = GetValidDto() with
        {
            ImpactStatistics = new CreateImpactStatisticDto
            {
                Title = string.Empty,
                Metrics =
                [
                    new CreateMetricDto { Value = 10, Name = "a", Type = MetricType.Partners },
                    new CreateMetricDto { Value = 20, Name = "b", Type = MetricType.Programs },
                    new CreateMetricDto { Value = 30, Name = "c", Type = MetricType.Raised },
                    new CreateMetricDto { Value = 40, Name = "d", Type = MetricType.TherapyHours },
                ],
            },
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor("ImpactStatistics.Title");
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenOptionalSectionsAreNull()
    {
        var dto = GetValidDto() with
        {
            MainAboutUs = null,
            MainPartners = null,
            MainDonations = null,
            ImpactStatistics = null,
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0, 7)]
    [InlineData(1, 32)]
    [InlineData(50, 1257)]
    [InlineData(150, 3757)]
    [InlineData(1000, 25007)]
    public void CalculateHighestCharactersLimitForRichInput_ShouldReturnExpectedValue(int rawLimit, int expected)
    {
        var result = MainPageConstants.CalculateHighestCharactersLimitForRichInput(rawLimit);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void MainPageRichTextLimits_ShouldAllowFormattingPayloadOverRawTextLimit()
    {
        var visibleText = new string('a', 34);
        var formattedTitle = $"<p><strong>{visibleText}</strong></p>";

        Assert.True(formattedTitle.Length > 50);
        Assert.True(formattedTitle.Length <= MainPageConstants.Localization.ValidationTitleRules.MaxLen);
    }

    private static CreateMainPageDto GetValidDto() => new()
    {
        Title = "Main page title",
        Description = "Main page description",
        ImageId = 1,
        MainAboutUs = new CreateMainAboutUsDto
        {
            Title = "About us title",
            Description = "About us description",
        },
        MainPartners = new CreateMainPartnersDto
        {
            Title = "Partners title",
            Description = "Partners description",
        },
        MainDonations = new CreateMainDonationsDto
        {
            Title = "Donations title",
            Description = "Donations description",
        },
        ImpactStatistics = GetValidImpactStatisticDto(),
    };

    private static CreateImpactStatisticDto GetValidImpactStatisticDto() => new()
    {
        Title = "Impact statistic title",
        ImageId = 2,
        Metrics =
        [
            new CreateMetricDto { Value = 100, Name = "Partners", Type = MetricType.Partners },
            new CreateMetricDto { Value = 200, Name = "Programs", Type = MetricType.Programs },
            new CreateMetricDto { Value = 300, Name = "Raised", Type = MetricType.Raised },
            new CreateMetricDto { Value = 400, Name = "Therapy", Type = MetricType.TherapyHours },
        ],
    };
}
