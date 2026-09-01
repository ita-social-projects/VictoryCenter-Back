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
    public void Validate_NameIsNull_ShouldNotThrow_ShouldHaveRequiredError()
    {
        var dto = new CreateTeamCategoryDto { Name = null!, Description = _validDescription };

        var exception = Record.Exception(() => _validator.TestValidate(dto));
        Assert.Null(exception);

        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamCategoryDto.Name)));
    }

    [Theory]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\u00A0")]
    public void Validate_NameIsNonSpaceWhitespace_ShouldHaveRequiredError(string name)
    {
        var dto = new CreateTeamCategoryDto { Name = name, Description = _validDescription };

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
    public void Validate_DescriptionIsNull_ShouldNotThrow_ShouldHaveRequiredError()
    {
        var dto = new CreateTeamCategoryDto { Name = _validName, Description = null! };

        var exception = Record.Exception(() => _validator.TestValidate(dto));
        Assert.Null(exception);

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

    public static IEnumerable<object[]> NameWithSpaces()
    {
        var valid = new string('N', TeamCategoryConstants.MinNameLength + 1);
        yield return new object[] { $" {valid}" };
        yield return new object[] { $"{valid} " };
        yield return new object[] { $" {valid} " };
    }

    [Theory]
    [MemberData(nameof(NameWithSpaces))]
    public void Validate_NameHasLeadingOrTrailingSpaces_ShouldHaveError(string name)
    {
        var dto = new CreateTeamCategoryDto { Name = name, Description = _validDescription };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustNotHaveLeadingOrTrailingSpaces(
                nameof(CreateTeamCategoryDto.Name)));
    }

    [Fact]
    public void Validate_NameAtMaxLengthWithTrailingSpace_ShouldHaveSpacesError_NotLengthError()
    {
        var maxLengthName = new string('N', TeamCategoryConstants.MaxNameLength);
        var dto = new CreateTeamCategoryDto { Name = maxLengthName + " ", Description = _validDescription };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustNotHaveLeadingOrTrailingSpaces(
                nameof(CreateTeamCategoryDto.Name)))
            .Only();
    }

    [Fact]
    public void Validate_NamePaddedWithTrimmedLengthTooShort_ShouldHaveSpacesError_NotLengthError()
    {
        var paddedShortName = $"  {_tooShortName}  ";
        var dto = new CreateTeamCategoryDto { Name = paddedShortName, Description = _validDescription };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustNotHaveLeadingOrTrailingSpaces(
                nameof(CreateTeamCategoryDto.Name)));
    }

    public static IEnumerable<object[]> DescriptionWithSpaces()
    {
        var valid = new string('D', TeamCategoryConstants.MinDescriptionLength + 1);
        yield return new object[] { $" {valid}" };
        yield return new object[] { $"{valid} " };
        yield return new object[] { $" {valid} " };
    }

    [Theory]
    [MemberData(nameof(DescriptionWithSpaces))]
    public void Validate_DescriptionHasLeadingOrTrailingSpaces_ShouldHaveError(string description)
    {
        var dto = new CreateTeamCategoryDto { Name = _validName, Description = description };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustNotHaveLeadingOrTrailingSpaces(
                nameof(CreateTeamCategoryDto.Description)));
    }

    [Fact]
    public void Validate_DescriptionAtMaxLengthWithTrailingSpace_ShouldHaveSpacesError_NotLengthError()
    {
        var maxLengthDescription = new string('D', TeamCategoryConstants.MaxDescriptionLength);
        var dto = new CreateTeamCategoryDto { Name = _validName, Description = maxLengthDescription + " " };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustNotHaveLeadingOrTrailingSpaces(
                nameof(CreateTeamCategoryDto.Description)))
            .Only();
    }

    [Fact]
    public void Validate_NameHasNoLeadingOrTrailingSpaces_ShouldNotHaveError()
    {
        var dto = new CreateTeamCategoryDto { Name = _validName, Description = _validDescription };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_DescriptionHasNoLeadingOrTrailingSpaces_ShouldNotHaveError()
    {
        var dto = new CreateTeamCategoryDto { Name = _validName, Description = _validDescription };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    public static IEnumerable<object[]> NameWithMultipleInternalSpaces()
    {
        var valid = new string('N', TeamCategoryConstants.MinNameLength);
        yield return new object[] { $"{valid}  {valid}" };
        yield return new object[] { $"{valid}   {valid}" };
        yield return new object[] { $"{valid} {valid}  {valid}" };
    }

    [Theory]
    [MemberData(nameof(NameWithMultipleInternalSpaces))]
    public void Validate_NameHasMultipleConsecutiveInternalSpaces_ShouldHaveError(string name)
    {
        var dto = new CreateTeamCategoryDto { Name = name, Description = _validDescription };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustNotHaveMultipleConsecutiveSpaces(
                nameof(CreateTeamCategoryDto.Name)));
    }

    [Fact]
    public void Validate_NameHasSingleInternalSpace_ShouldNotHaveMultipleSpacesError()
    {
        var part = new string('N', TeamCategoryConstants.MinNameLength);
        var dto = new CreateTeamCategoryDto { Name = $"{part} {part}", Description = _validDescription };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameHasTabBetweenWords_ShouldNotHaveMultipleSpacesError()
    {
        var part = new string('N', TeamCategoryConstants.MinNameLength);
        var dto = new CreateTeamCategoryDto { Name = $"{part}\t{part}", Description = _validDescription };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    public static IEnumerable<object[]> DescriptionWithMultipleInternalSpaces()
    {
        var valid = new string('D', TeamCategoryConstants.MinDescriptionLength);
        yield return new object[] { $"{valid}  {valid}" };
        yield return new object[] { $"{valid}   {valid}" };
        yield return new object[] { $"{valid} {valid}  {valid}" };
    }

    [Theory]
    [MemberData(nameof(DescriptionWithMultipleInternalSpaces))]
    public void Validate_DescriptionHasMultipleConsecutiveInternalSpaces_ShouldHaveError(string description)
    {
        var dto = new CreateTeamCategoryDto { Name = _validName, Description = description };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustNotHaveMultipleConsecutiveSpaces(
                nameof(CreateTeamCategoryDto.Description)));
    }

    [Fact]
    public void Validate_DescriptionHasSingleInternalSpace_ShouldNotHaveMultipleSpacesError()
    {
        var part = new string('D', TeamCategoryConstants.MinDescriptionLength);
        var dto = new CreateTeamCategoryDto { Name = _validName, Description = $"{part} {part}" };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
}
