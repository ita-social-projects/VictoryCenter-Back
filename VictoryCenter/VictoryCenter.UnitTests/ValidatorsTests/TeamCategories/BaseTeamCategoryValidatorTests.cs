using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.BLL.Validators.TeamCategories;

namespace VictoryCenter.UnitTests.ValidatorsTests.TeamCategories;

public class BaseTeamCategoryValidatorTests
{
    private readonly string _validName = new('N', TeamCategoryConstants.MinNameLength + 1);
    private readonly string _validDescription = new('D', TeamCategoryConstants.MinDescriptionLength + 1);

    private readonly string _tooShortName = new('N', TeamCategoryConstants.MinNameLength - 1);
    private readonly string _tooLongName = new('N', TeamCategoryConstants.MaxNameLength + 1);

    private readonly string _tooShortDescription = new('D', TeamCategoryConstants.MinDescriptionLength - 1);
    private readonly string _tooLongDescription = new('D', TeamCategoryConstants.MaxDescriptionLength + 1);
    private readonly BaseTeamCategoryValidator _validator;

    public BaseTeamCategoryValidatorTests()
    {
        _validator = new BaseTeamCategoryValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_NameIsEmpty_ShouldHaveError(string? name)
    {
        var dto = new CreateTeamCategoryDto { Name = name!, Description = _validDescription };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamCategoryDto.Name)));
    }

    [Fact]
    public void Validate_NameIsTooShort_ShouldHaveError()
    {
        var dto = new CreateTeamCategoryDto { Name = _tooShortName, Description = _validDescription };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateTeamCategoryDto.Name), TeamCategoryConstants.MinNameLength));
    }

    [Fact]
    public void Validate_NameIsTooLong_ShouldHaveError()
    {
        var dto = new CreateTeamCategoryDto { Name = _tooLongName, Description = _validDescription };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateTeamCategoryDto.Name), TeamCategoryConstants.MaxNameLength));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_DescriptionIsEmpty_ShouldHaveError(string? description)
    {
        var dto = new CreateTeamCategoryDto { Name = _validName, Description = description };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamCategoryDto.Description)));
    }

    [Fact]
    public void Validate_DescriptionIsTooShort_ShouldHaveError()
    {
        var dto = new CreateTeamCategoryDto { Name = _validName, Description = _tooShortDescription };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateTeamCategoryDto.Description), TeamCategoryConstants.MinDescriptionLength));
    }

    [Fact]
    public void Validate_DescriptionIsTooLong_ShouldHaveError()
    {
        var dto = new CreateTeamCategoryDto { Name = _validName, Description = _tooLongDescription };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateTeamCategoryDto.Description), TeamCategoryConstants.MaxDescriptionLength));
    }

    [Fact]
    public void Validate_NameAtMinimumLength_ShouldNotHaveError()
    {
        var minLengthName = new string('N', TeamCategoryConstants.MinNameLength);

        var dto = new CreateTeamCategoryDto { Name = minLengthName, Description = _validDescription };

        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameAtMaximumLength_ShouldNotHaveError()
    {
        var maxLengthName = new string('N', TeamCategoryConstants.MaxNameLength);

        var dto = new CreateTeamCategoryDto { Name = maxLengthName, Description = _validDescription };

        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_DescriptionAtMinimumLength_ShouldNotHaveError()
    {
        var minLengthDescription = new string('D', TeamCategoryConstants.MinDescriptionLength);

        var dto = new CreateTeamCategoryDto { Name = _validName, Description = minLengthDescription };

        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_DescriptionAtMaximumLength_ShouldNotHaveError()
    {
        var maxLengthDescription = new string('D', TeamCategoryConstants.MaxDescriptionLength);

        var dto = new CreateTeamCategoryDto { Name = _validName, Description = maxLengthDescription };

        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ValidDto_ShouldNotHaveErrors()
    {
        var dto = new CreateTeamCategoryDto
        {
            Name = _validName,
            Description = _validDescription
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
