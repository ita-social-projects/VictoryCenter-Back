using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Validators.Localization.MainPage;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.MainPage;

public class UpdateMainPageLocalizationValidatorTests
{
    private readonly UpdateMainPageLocalizationCommandValidator _validator;

    public UpdateMainPageLocalizationValidatorTests()
    {
        var baseValidator = new BaseMainPageLocalizationDtoValidator();
        var dtoValidator = new UpdateMainPageLocalizationDtoValidator(
            new UpdateMainAboutUsLocalizationDtoValidator(baseValidator),
            new UpdateMainPartnersLocalizationDtoValidator(baseValidator),
            new UpdateMainDonationsLocalizationDtoValidator(baseValidator));

        _validator = new UpdateMainPageLocalizationCommandValidator(dtoValidator);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllFieldsAreValid()
    {
        _validator.TestValidate(BuildCommand(GetValidDto())).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveRequiredError_WhenTitleBlockTitleIsEmpty()
    {
        var dto = GetValidDto() with { Title = " " };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateMainPageLocalizationDto.Title)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleBlockTitleIsTooShort()
    {
        var dto = GetValidDto() with
        {
            Title = new string('a', MainPageConstants.Localization.ValidationTitleRules.MinLen - 1)
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateMainPageLocalizationDto.Title), MainPageConstants.Localization.ValidationTitleRules.MinLen));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleBlockDescriptionIsTooLong()
    {
        var dto = GetValidDto() with
        {
            Description = new string('a', MainPageConstants.Localization.ValidationTitleBlockDescriptionRules.MaxLen + 1)
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateMainPageLocalizationDto.Description),
                MainPageConstants.Localization.ValidationTitleBlockDescriptionRules.MaxLen));
    }

    [Theory]
    [InlineData(nameof(UpdateMainPageLocalizationDto.MainAboutUs))]
    [InlineData(nameof(UpdateMainPageLocalizationDto.MainPartners))]
    [InlineData(nameof(UpdateMainPageLocalizationDto.MainDonations))]
    public void Validate_ShouldHaveError_WhenSectionTitleIsTooShort(string sectionName)
    {
        var dto = GetValidDtoWithInvalidSection(sectionName, title: new string('a', MainPageConstants.Localization.ValidationTitleRules.MinLen - 1));

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor($"Dto.{sectionName}.Title");
    }

    [Theory]
    [InlineData(nameof(UpdateMainPageLocalizationDto.MainAboutUs))]
    [InlineData(nameof(UpdateMainPageLocalizationDto.MainPartners))]
    [InlineData(nameof(UpdateMainPageLocalizationDto.MainDonations))]
    public void Validate_ShouldHaveError_WhenSectionDescriptionIsTooLong(string sectionName)
    {
        var dto = GetValidDtoWithInvalidSection(
            sectionName,
            description: new string('a', MainPageConstants.Localization.ValidationSectionDescriptionRules.MaxLen + 1));

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor($"Dto.{sectionName}.Description");
    }

    private static UpdateMainPageLocalizationDto GetValidDto() => new()
    {
        Title = new string('a', MainPageConstants.Localization.ValidationTitleRules.MinLen),
        Description = new string('a', MainPageConstants.Localization.ValidationTitleBlockDescriptionRules.MinLen),
        MainAboutUs = GetValidAboutUs(),
        MainPartners = GetValidPartners(),
        MainDonations = GetValidDonations()
    };

    private static UpdateMainPageLocalizationDto GetValidDtoWithInvalidSection(
        string sectionName,
        string? title = null,
        string? description = null)
    {
        var dto = GetValidDto();

        return sectionName switch
        {
            nameof(UpdateMainPageLocalizationDto.MainAboutUs) => dto with
            {
                MainAboutUs = GetValidAboutUs() with
                {
                    Title = title ?? GetValidAboutUs().Title,
                    Description = description ?? GetValidAboutUs().Description
                }
            },
            nameof(UpdateMainPageLocalizationDto.MainPartners) => dto with
            {
                MainPartners = GetValidPartners() with
                {
                    Title = title ?? GetValidPartners().Title,
                    Description = description ?? GetValidPartners().Description
                }
            },
            nameof(UpdateMainPageLocalizationDto.MainDonations) => dto with
            {
                MainDonations = GetValidDonations() with
                {
                    Title = title ?? GetValidDonations().Title,
                    Description = description ?? GetValidDonations().Description
                }
            },
            _ => dto
        };
    }

    private static UpdateMainAboutUsLocalizationDto GetValidAboutUs() => new()
    {
        Title = new string('a', MainPageConstants.Localization.ValidationTitleRules.MinLen),
        Description = new string('a', MainPageConstants.Localization.ValidationSectionDescriptionRules.MinLen)
    };

    private static UpdateMainPartnersLocalizationDto GetValidPartners() => new()
    {
        Title = new string('a', MainPageConstants.Localization.ValidationTitleRules.MinLen),
        Description = new string('a', MainPageConstants.Localization.ValidationSectionDescriptionRules.MinLen)
    };

    private static UpdateMainDonationsLocalizationDto GetValidDonations() => new()
    {
        Title = new string('a', MainPageConstants.Localization.ValidationTitleRules.MinLen),
        Description = new string('a', MainPageConstants.Localization.ValidationSectionDescriptionRules.MinLen)
    };

    private static UpdateMainPageLocalizationCommand BuildCommand(UpdateMainPageLocalizationDto dto)
        => new(dto, 1, 1);
}
