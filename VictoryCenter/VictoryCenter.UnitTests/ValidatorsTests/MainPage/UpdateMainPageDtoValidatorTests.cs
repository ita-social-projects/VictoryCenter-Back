using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.BLL.Validators.MainPage.Dto;

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
    public void Validate_ShouldHaveError_WhenImpactStatisticsIsNull()
    {
        var dto = GetValidDto() with { ImpactStatistics = null! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.ImpactStatistics)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateMainPageDto.ImpactStatistics)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenImpactStatisticsCountIsTooLarge()
    {
        var dto = GetValidDto() with
        {
            ImpactStatistics = Enumerable
                .Range(1, MainPageConstants.ImpactStatistic.MaxCount + 1)
                .Select(_ => GetValidImpactStatisticDto())
                .ToList(),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.ImpactStatistics)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(UpdateMainPageDto.ImpactStatistics), MainPageConstants.ImpactStatistic.MaxCount));
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
    public void Validate_ShouldHaveError_WhenNestedImpactStatisticIsInvalid()
    {
        var dto = GetValidDto() with
        {
            ImpactStatistics =
            [
                new UpdateImpactStatisticDto
                {
                    Description = string.Empty,
                    Metrics = [new UpdateMetricDto { Value = "10", Signature = "kids" }],
                },
            ],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor("ImpactStatistics[0].Description");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenImpactStatisticsContainsNullElement()
    {
        var dto = GetValidDto() with
        {
            ImpactStatistics = [null!],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor("ImpactStatistics[0]");
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenOptionalSectionsAreNull()
    {
        var dto = GetValidDto() with
        {
            MainAboutUs = null,
            MainPartners = null,
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenImpactStatisticsContainDuplicateIds()
    {
        var dto = GetValidDto() with
        {
            ImpactStatistics =
            [
                new UpdateImpactStatisticDto
                {
                    Id = 10,
                    Description = "Desc 1",
                    Metrics = [new UpdateMetricDto { Id = 1, Value = "10", Signature = "kids" }],
                },
                new UpdateImpactStatisticDto
                {
                    Id = 10,
                    Description = "Desc 2",
                    Metrics = [new UpdateMetricDto { Id = 2, Value = "20", Signature = "families" }],
                },
            ],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.ImpactStatistics)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdateMainPageDto.ImpactStatistics)));
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
        ImpactStatistics = [GetValidImpactStatisticDto()],
    };

    private static UpdateImpactStatisticDto GetValidImpactStatisticDto() => new()
    {
        Id = 1,
        Description = "Impact statistic description",
        ImageId = 2,
        Metrics = [new UpdateMetricDto { Id = 1, Value = "100", Signature = "children" }],
    };
}