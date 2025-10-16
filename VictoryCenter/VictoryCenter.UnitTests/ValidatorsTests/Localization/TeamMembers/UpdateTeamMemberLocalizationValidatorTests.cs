using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.Validators.Localization.TeamMembers;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.TeamMembers;

public class UpdateTeamMemberLocalizationValidatorTests
{
    private readonly UpdateTeamMemberLocalizationValidator _validator;

    public UpdateTeamMemberLocalizationValidatorTests()
    {
        _validator = new UpdateTeamMemberLocalizationValidator(new BaseTeamMemberLocalizationValidator());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_When_LanguageIdIsNotPositive(long invalidLanguageId)
    {
        var command = new UpdateTeamMemberLocalizationCommand(
            new UpdateTeamMemberLocalizationDto
                {
                    LanguageId = invalidLanguageId,
                    FullName = "Valid Name",
                    Description = "Valid description"
                });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateTeamMemberLocalizationDto.LanguageId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateTeamMemberDto.CategoryId)));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_ShouldHaveError_When_FullNameIsEmpty(string? invalidFullName)
    {
        var command = new UpdateTeamMemberLocalizationCommand(
            new UpdateTeamMemberLocalizationDto
                {
                    LanguageId = 1,
                    FullName = invalidFullName!,
                    Description = "Valid description"
                });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateTeamMemberLocalizationDto.FullName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamMemberDto.FullName)));
    }

    [Theory]
    [InlineData("A")]
    public void Validate_ShouldHaveError_When_FullNameTooShort(string invalidFullName)
    {
        var command = new UpdateTeamMemberLocalizationCommand(
            new UpdateTeamMemberLocalizationDto
                {
                    LanguageId = 1,
                    FullName = invalidFullName,
                    Description = "Valid description"
                });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateTeamMemberLocalizationDto.FullName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.FullName),
                BaseTeamMemberLocalizationValidator.FullNameMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_FullNameTooLong()
    {
        var longName = new string('A', BaseTeamMemberLocalizationValidator.FullNameMaxLength + 1);

        var command = new UpdateTeamMemberLocalizationCommand(
            new UpdateTeamMemberLocalizationDto
                {
                    LanguageId = 1,
                    FullName = longName,
                    Description = "Valid description"
                });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateTeamMemberLocalizationDto.FullName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.FullName),
                BaseTeamMemberLocalizationValidator.FullNameMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_DescriptionTooShort()
    {
        var shortDescription = new string('A', BaseTeamMemberLocalizationValidator.DescriptionNameMinLength - 1);

        var command = new UpdateTeamMemberLocalizationCommand(
            new UpdateTeamMemberLocalizationDto
                {
                    LanguageId = 1,
                    FullName = "Valid Name",
                    Description = shortDescription
                });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateTeamMemberLocalizationDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.Description),
                BaseTeamMemberLocalizationValidator.DescriptionNameMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_When_DescriptionTooLong()
    {
        var longDescription = new string('A', BaseTeamMemberLocalizationValidator.DescriptionNameMaxLength + 1);

        var command = new UpdateTeamMemberLocalizationCommand(
            new UpdateTeamMemberLocalizationDto
                {
                    LanguageId = 1,
                    FullName = "Valid Name",
                    Description = longDescription
                });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateTeamMemberLocalizationDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.Description),
                BaseTeamMemberLocalizationValidator.DescriptionNameMaxLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_When_DtoIsValid()
    {
        var command = new UpdateTeamMemberLocalizationCommand(
            new UpdateTeamMemberLocalizationDto
                {
                    LanguageId = 1,
                    FullName = "Valid FullName",
                    Description = "This is a valid description of sufficient length."
                });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
