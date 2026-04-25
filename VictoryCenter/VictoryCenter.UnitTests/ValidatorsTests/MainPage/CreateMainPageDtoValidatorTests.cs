using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
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
        var dto = GetValidDto() with { Title = new string('a', MainPageConstants.Title.MinLength - 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPageDto.Title), MainPageConstants.Title.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooLong()
    {
        var dto = GetValidDto() with { Title = new string('a', MainPageConstants.Title.MaxLength + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPageDto.Title), MainPageConstants.Title.MaxLength));
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
        var dto = GetValidDto() with { Description = new string('a', MainPageConstants.Description.MinLength - 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPageDto.Description), MainPageConstants.Description.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var dto = GetValidDto() with { Description = new string('a', MainPageConstants.Description.MaxLength + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPageDto.Description), MainPageConstants.Description.MaxLength));
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
                    new CreateMetricDto { Value = 30, Name = "c", Type = MetricType.Raiced },
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
            ImpactStatistics = null,
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
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
            new CreateMetricDto { Value = 300, Name = "Raised", Type = MetricType.Raiced },
            new CreateMetricDto { Value = 400, Name = "Therapy", Type = MetricType.TherapyHours },
        ],
    };
}
