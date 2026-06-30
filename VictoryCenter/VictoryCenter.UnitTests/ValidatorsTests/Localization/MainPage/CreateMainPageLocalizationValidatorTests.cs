using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Validators.Localization.MainPage;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.MainPage;

public class CreateMainPageLocalizationValidatorTests
{
    private readonly CreateMainPageLocalizationCommandValidator _validator;

    public CreateMainPageLocalizationValidatorTests()
    {
        var baseValidator = new BaseMainPageLocalizationDtoValidator();
        var dtoValidator = new CreateMainPageLocalizationDtoValidator(
            baseValidator,
            new CreateMainAboutUsLocalizationDtoValidator(baseValidator),
            new CreateMainPartnersLocalizationDtoValidator(baseValidator),
            new CreateMainDonationsLocalizationDtoValidator(baseValidator));

        _validator = new CreateMainPageLocalizationCommandValidator(dtoValidator);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllFieldsAreValid()
    {
        _validator.TestValidate(BuildCommand(GetValidDto())).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenEntityIdIsNotPositive(long entityId)
    {
        var dto = GetValidDto() with { EntityId = entityId };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.EntityId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ILocalizationIdentity.EntityId)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenLanguageIdIsNotPositive(long languageId)
    {
        var dto = GetValidDto() with { LanguageId = languageId };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.LanguageId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ILocalizationIdentity.LanguageId)));
    }

    [Fact]
    public void Validate_ShouldHaveRequiredError_WhenTitleBlockTitleIsEmpty()
    {
        var dto = GetValidDto() with { Title = " " };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateMainPageLocalizationDto.Title)));
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
                nameof(CreateMainPageLocalizationDto.Title), MainPageConstants.Localization.ValidationTitleRules.MinLen));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleBlockTitleIsTooLong()
    {
        var dto = GetValidDto() with
        {
            Title = new string('a', MainPageConstants.Localization.ValidationTitleRules.MaxLen + 1)
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateMainPageLocalizationDto.Title), MainPageConstants.Localization.ValidationTitleRules.MaxLen));
    }

    [Fact]
    public void Validate_ShouldHaveRequiredError_WhenTitleBlockDescriptionIsEmpty()
    {
        var dto = GetValidDto() with { Description = " " };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateMainPageLocalizationDto.Description)));
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
                nameof(CreateMainPageLocalizationDto.Description),
                MainPageConstants.Localization.ValidationTitleBlockDescriptionRules.MaxLen));
    }

    [Theory]
    [InlineData(nameof(CreateMainPageLocalizationDto.MainAboutUs))]
    [InlineData(nameof(CreateMainPageLocalizationDto.MainPartners))]
    [InlineData(nameof(CreateMainPageLocalizationDto.MainDonations))]
    public void Validate_ShouldHaveError_WhenSectionTitleIsTooLong(string sectionName)
    {
        var dto = GetValidDtoWithInvalidSection(sectionName, title: new string('a', MainPageConstants.Localization.ValidationTitleRules.MaxLen + 1));

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor($"Dto.{sectionName}.Title");
    }

    [Theory]
    [InlineData(nameof(CreateMainPageLocalizationDto.MainAboutUs))]
    [InlineData(nameof(CreateMainPageLocalizationDto.MainPartners))]
    [InlineData(nameof(CreateMainPageLocalizationDto.MainDonations))]
    public void Validate_ShouldHaveError_WhenSectionDescriptionIsTooLong(string sectionName)
    {
        var dto = GetValidDtoWithInvalidSection(
            sectionName,
            description: new string('a', MainPageConstants.Localization.ValidationSectionDescriptionRules.MaxLen + 1));

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor($"Dto.{sectionName}.Description");
    }

    private static CreateMainPageLocalizationDto GetValidDto() => new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = new string('a', MainPageConstants.Localization.ValidationTitleRules.MinLen),
        Description = new string('a', MainPageConstants.Localization.ValidationTitleBlockDescriptionRules.MinLen),
        MainAboutUs = GetValidAboutUs(),
        MainPartners = GetValidPartners(),
        MainDonations = GetValidDonations()
    };

    private static CreateMainPageLocalizationDto GetValidDtoWithInvalidSection(
        string sectionName,
        string? title = null,
        string? description = null)
    {
        var dto = GetValidDto();

        return sectionName switch
        {
            nameof(CreateMainPageLocalizationDto.MainAboutUs) => dto with
            {
                MainAboutUs = GetValidAboutUs() with
                {
                    Title = title ?? GetValidAboutUs().Title,
                    Description = description ?? GetValidAboutUs().Description
                }
            },
            nameof(CreateMainPageLocalizationDto.MainPartners) => dto with
            {
                MainPartners = GetValidPartners() with
                {
                    Title = title ?? GetValidPartners().Title,
                    Description = description ?? GetValidPartners().Description
                }
            },
            nameof(CreateMainPageLocalizationDto.MainDonations) => dto with
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

    private static CreateMainAboutUsLocalizationDto GetValidAboutUs() => new()
    {
        EntityId = 2,
        Title = new string('a', MainPageConstants.Localization.ValidationTitleRules.MinLen),
        Description = new string('a', MainPageConstants.Localization.ValidationSectionDescriptionRules.MinLen)
    };

    private static CreateMainPartnersLocalizationDto GetValidPartners() => new()
    {
        EntityId = 3,
        Title = new string('a', MainPageConstants.Localization.ValidationTitleRules.MinLen),
        Description = new string('a', MainPageConstants.Localization.ValidationSectionDescriptionRules.MinLen)
    };

    private static CreateMainDonationsLocalizationDto GetValidDonations() => new()
    {
        EntityId = 4,
        Title = new string('a', MainPageConstants.Localization.ValidationTitleRules.MinLen),
        Description = new string('a', MainPageConstants.Localization.ValidationSectionDescriptionRules.MinLen)
    };

    private static CreateMainPageLocalizationCommand BuildCommand(CreateMainPageLocalizationDto dto) => new(dto);
}
