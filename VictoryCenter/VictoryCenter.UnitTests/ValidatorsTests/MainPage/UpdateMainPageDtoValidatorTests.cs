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

public class UpdateMainPageDtoValidatorTests
{
    private readonly UpdateMainPageDtoValidator _validator = new();

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
        var dto = GetValidDto() with { Title = title! };

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
            MainAboutUs = new UpdateMainAboutUsDto
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
            MainPartners = new UpdateMainPartnersDto
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
            MainDonations = new UpdateMainDonationsDto
            {
                Title = string.Empty,
                Description = "Valid description",
            },
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor("MainDonations.Title");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNestedImpactStatisticIsInvalid()
    {
        var dto = GetValidDto() with
        {
            ImpactStatistics = new UpdateImpactStatisticDto
            {
                Title = string.Empty,
                Metrics = [new UpdateMetricDto { Value = 10, Name = "kids", Type = MetricType.Raised }],
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

    private static UpdateMainPageDto GetValidDto() => new()
    {
        Title = "Main page title",
        Description = "Main page description",
        ImageId = 1,
        MainAboutUs = new UpdateMainAboutUsDto
        {
            Title = "About us title",
            Description = "About us description",
        },
        MainPartners = new UpdateMainPartnersDto
        {
            Title = "Partners title",
            Description = "Partners description",
        },
        MainDonations = new UpdateMainDonationsDto
        {
            Title = "Donations title",
            Description = "Donations description",
        },
        ImpactStatistics = GetValidImpactStatisticDto(),
    };

    private static UpdateImpactStatisticDto GetValidImpactStatisticDto() => new()
    {
        Id = 1,
        Title = "Impact statistic title",
        ImageId = 2,
        Metrics = [new UpdateMetricDto { Id = 1, Value = 100, Name = "children", Type = MetricType.Raised }],
    };
}
