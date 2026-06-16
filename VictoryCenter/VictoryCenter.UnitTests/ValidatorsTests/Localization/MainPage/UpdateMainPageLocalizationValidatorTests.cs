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
        var mainAboutUsValidator = new UpdateMainAboutUsLocalizationDtoValidator(baseValidator);
        var mainPartnersValidator = new UpdateMainPartnersLocalizationDtoValidator(baseValidator);
        var mainDonationsValidator = new UpdateMainDonationsLocalizationDtoValidator(baseValidator);
        var dtoValidator = new UpdateMainPageLocalizationDtoValidator(
            baseValidator,
            mainAboutUsValidator,
            mainPartnersValidator,
            mainDonationsValidator);
        _validator = new UpdateMainPageLocalizationCommandValidator(dtoValidator);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllFieldsAreValid()
    {
        var command = BuildCommand(GetValidDto());

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenOptionalFieldsAreNull()
    {
        var command = BuildCommand(new UpdateMainPageLocalizationDto
        {
            Title = null,
            Description = null,
            MainAboutUs = null,
            MainPartners = null,
            MainDonations = null
        });

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooShort()
    {
        var dto = GetValidDto() with
        {
            Title = new string('a', MainPageConstants.Title.MinLength - 1)
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateMainPageLocalizationDto.Title), MainPageConstants.Title.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var dto = GetValidDto() with
        {
            Description = new string('a', MainPageConstants.Description.MaxLength + 1)
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor(x => x.Dto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateMainPageLocalizationDto.Description), MainPageConstants.Description.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainAboutUsIsInvalid()
    {
        var dto = GetValidDto() with
        {
            MainAboutUs = new UpdateMainAboutUsLocalizationDto
            {
                Title = new string('a', MainPageConstants.Title.MinLength - 1)
            }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainAboutUs.Title");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainPartnersIsInvalid()
    {
        var dto = GetValidDto() with
        {
            MainPartners = new UpdateMainPartnersLocalizationDto
            {
                Title = new string('a', MainPageConstants.Title.MinLength - 1)
            }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainPartners.Title");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMainDonationsIsInvalid()
    {
        var dto = GetValidDto() with
        {
            MainDonations = new UpdateMainDonationsLocalizationDto
            {
                Title = new string('a', MainPageConstants.Title.MinLength - 1)
            }
        };

        _validator.TestValidate(BuildCommand(dto))
            .ShouldHaveValidationErrorFor("Dto.MainDonations.Title");
    }

    private static UpdateMainPageLocalizationDto GetValidDto() => new()
    {
        Title = new string('a', MainPageConstants.Title.MinLength),
        Description = new string('a', MainPageConstants.Description.MinLength),
        MainAboutUs = new UpdateMainAboutUsLocalizationDto
        {
            Title = new string('a', MainPageConstants.Title.MinLength),
            Description = new string('a', MainPageConstants.Description.MinLength)
        },
        MainPartners = new UpdateMainPartnersLocalizationDto
        {
            Title = new string('a', MainPageConstants.Title.MinLength),
            Description = new string('a', MainPageConstants.Description.MinLength)
        },
        MainDonations = new UpdateMainDonationsLocalizationDto
        {
            Title = new string('a', MainPageConstants.Title.MinLength),
            Description = new string('a', MainPageConstants.Description.MinLength)
        }
    };

    private static UpdateMainPageLocalizationCommand BuildCommand(UpdateMainPageLocalizationDto dto)
        => new(dto, 1, 1);
}
